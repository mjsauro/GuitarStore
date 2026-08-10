#!/usr/bin/env bash
# Deploys GuitarStore.Web to AWS Lambda behind an API Gateway HTTP API.
#
# Why API Gateway and not a Lambda function URL: this account (like all accounts
# created after ~2024) blocks public access to function URLs by default. Fronting the
# URL with CloudFront + OAC works for GETs but breaks every form post — OAC signs
# requests with SigV4 and Lambda rejects unsigned payloads, so a browser POST without
# an x-amz-content-sha256 header gets a 403. API Gateway invokes Lambda directly and
# has no such constraint, and the function stays private either way.
#
# Safe to re-run: creates what's missing, updates what exists.
#
#   ./infra/deploy.sh          # build, package, create/update, print the URL
#   ./infra/deploy.sh --code   # skip provisioning, just push new code
#
# Environment:
#   AWS_REGION      deployment region (default us-east-2)
#   SES_IDENTITY    verified sender address for order receipts. When unset, the SES
#                   policy step is skipped and the app logs receipts instead of sending.
#
# The account id is read from the caller's identity rather than committed, and the IAM
# policies are rendered from the .template.json files at deploy time.
set -euo pipefail

REGION="${AWS_REGION:-us-east-2}"
SES_IDENTITY="${SES_IDENTITY:-}"
FUNCTION_NAME="guitarstore-web"
ROLE_NAME="guitarstore-lambda-role"
POLICY_NAME="GuitarStoreDynamoDbAccess"
API_NAME="guitarstore-api"
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT="$REPO_ROOT/GuitarStore.Web"
ARTIFACT="/tmp/guitarstore-lambda.zip"

CODE_ONLY=false
[[ "${1:-}" == "--code" ]] && CODE_ONLY=true

ACCOUNT_ID="$(aws sts get-caller-identity --query Account --output text)"

RENDERED="$(mktemp -d)"
trap 'rm -rf "$RENDERED"' EXIT

# Substitutes ${ACCOUNT_ID}, ${REGION}, and ${SES_IDENTITY} into a policy template.
render_policy() {
  local template="$REPO_ROOT/infra/$1.template.json"
  local output="$RENDERED/$1.json"

  sed -e "s|\${ACCOUNT_ID}|${ACCOUNT_ID}|g" \
      -e "s|\${REGION}|${REGION}|g" \
      -e "s|\${SES_IDENTITY}|${SES_IDENTITY}|g" \
      "$template" > "$output"

  echo "$output"
}

# ---------------------------------------------------------------- IAM role
if [[ "$CODE_ONLY" == false ]]; then
  if ! aws iam get-role --role-name "$ROLE_NAME" >/dev/null 2>&1; then
    echo "Creating IAM role $ROLE_NAME"
    aws iam create-role \
      --role-name "$ROLE_NAME" \
      --assume-role-policy-document "file://$REPO_ROOT/infra/lambda-trust-policy.json" \
      --description "Execution role for the GuitarStore demo app" >/dev/null

    aws iam attach-role-policy \
      --role-name "$ROLE_NAME" \
      --policy-arn arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole

    # Wait for the role to propagate before Lambda will accept it.
    sleep 10
  fi

  echo "Applying DynamoDB access policy"
  aws iam put-role-policy \
    --role-name "$ROLE_NAME" \
    --policy-name "$POLICY_NAME" \
    --policy-document "file://$(render_policy dynamodb-access-policy)"

  echo "Applying data protection key policy"
  aws iam put-role-policy \
    --role-name "$ROLE_NAME" \
    --policy-name GuitarStoreDataProtection \
    --policy-document "file://$(render_policy dataprotection-policy)"

  if [[ -n "$SES_IDENTITY" ]]; then
    echo "Applying SES send policy for $SES_IDENTITY"
    aws iam put-role-policy \
      --role-name "$ROLE_NAME" \
      --policy-name GuitarStoreSesSend \
      --policy-document "file://$(render_policy ses-send-policy)"
  else
    echo "SES_IDENTITY not set — skipping SES policy (receipts will be logged, not sent)"
  fi
fi

ROLE_ARN="arn:aws:iam::${ACCOUNT_ID}:role/${ROLE_NAME}"

# ---------------------------------------------------------------- package
echo "Publishing $PROJECT"
rm -f "$ARTIFACT"
dotnet publish "$PROJECT" -c Release -r linux-arm64 --self-contained false \
  -o /tmp/guitarstore-publish >/dev/null
(cd /tmp/guitarstore-publish && zip -qr "$ARTIFACT" .)
echo "Packaged $(du -h "$ARTIFACT" | cut -f1)"

# ---------------------------------------------------------------- function
ENV_VARS="Variables={ASPNETCORE_ENVIRONMENT=Production,AWS__Region=${REGION},AWS__AutoProvision=false}"

if aws lambda get-function --function-name "$FUNCTION_NAME" --region "$REGION" >/dev/null 2>&1; then
  echo "Updating function code"
  aws lambda update-function-code \
    --function-name "$FUNCTION_NAME" \
    --zip-file "fileb://$ARTIFACT" \
    --region "$REGION" >/dev/null
  aws lambda wait function-updated --function-name "$FUNCTION_NAME" --region "$REGION"
else
  echo "Creating function $FUNCTION_NAME"
  aws lambda create-function \
    --function-name "$FUNCTION_NAME" \
    --runtime dotnet10 \
    --architectures arm64 \
    --handler GuitarStore.Web \
    --role "$ROLE_ARN" \
    --zip-file "fileb://$ARTIFACT" \
    --timeout 30 \
    --memory-size 1024 \
    --environment "$ENV_VARS" \
    --region "$REGION" >/dev/null
  aws lambda wait function-active --function-name "$FUNCTION_NAME" --region "$REGION"
fi

# Merge the settings this script owns into whatever is already on the function, rather
# than replacing the block wholesale — Cognito and email settings (including the client
# secret) are configured out of band and must survive a deploy.
EXISTING="$(aws lambda get-function-configuration --function-name "$FUNCTION_NAME" --region "$REGION" \
  --query 'Environment.Variables' --output json 2>/dev/null || echo '{}')"

MERGED="$(python3 -c '
import json, sys
existing = json.loads(sys.argv[1] or "{}") or {}
existing.update({
    "ASPNETCORE_ENVIRONMENT": "Production",
    "AWS__Region": sys.argv[2],
    "AWS__AutoProvision": "false",
})
print(json.dumps({"Variables": existing}))
' "$EXISTING" "$REGION")"

aws lambda update-function-configuration \
  --function-name "$FUNCTION_NAME" \
  --environment "$MERGED" \
  --region "$REGION" >/dev/null
aws lambda wait function-updated --function-name "$FUNCTION_NAME" --region "$REGION"

# ---------------------------------------------------------------- API Gateway
FUNCTION_ARN="arn:aws:lambda:${REGION}:${ACCOUNT_ID}:function:${FUNCTION_NAME}"
API_ID="$(aws apigatewayv2 get-apis --region "$REGION" \
  --query "Items[?Name=='${API_NAME}'].ApiId | [0]" --output text)"

if [[ "$API_ID" == "None" || -z "$API_ID" ]]; then
  echo "Creating HTTP API $API_NAME"
  API_ID="$(aws apigatewayv2 create-api \
    --name "$API_NAME" \
    --protocol-type HTTP \
    --target "$FUNCTION_ARN" \
    --region "$REGION" \
    --query ApiId --output text)"

  # Only this API may invoke the function; it is not otherwise reachable.
  aws lambda add-permission \
    --function-name "$FUNCTION_NAME" \
    --statement-id AllowApiGatewayInvoke \
    --action lambda:InvokeFunction \
    --principal apigateway.amazonaws.com \
    --source-arn "arn:aws:execute-api:${REGION}:${ACCOUNT_ID}:${API_ID}/*/*" \
    --region "$REGION" >/dev/null
fi

ENDPOINT="$(aws apigatewayv2 get-api --api-id "$API_ID" --region "$REGION" --query ApiEndpoint --output text)"

# Hand the endpoint to the workflow rather than making it hardcode one. The
# hostname embeds the generated API id, so it changes if this script ever has to
# recreate the API above.
[[ -n "${GITHUB_OUTPUT:-}" ]] && echo "endpoint=$ENDPOINT" >>"$GITHUB_OUTPUT"

echo
echo "Deployed: $ENDPOINT"
