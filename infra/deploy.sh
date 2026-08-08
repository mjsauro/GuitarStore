#!/usr/bin/env bash
# Deploys GuitarStore.Web to AWS Lambda behind a public Function URL.
#
# Safe to re-run: creates what's missing, updates what exists.
#
#   ./infra/deploy.sh          # build, package, create/update, print the URL
#   ./infra/deploy.sh --code   # skip provisioning, just push new code
set -euo pipefail

REGION="${AWS_REGION:-us-east-2}"
FUNCTION_NAME="guitarstore-web"
ROLE_NAME="guitarstore-lambda-role"
POLICY_NAME="GuitarStoreDynamoDbAccess"
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT="$REPO_ROOT/GuitarStore.Web"
ARTIFACT="/tmp/guitarstore-lambda.zip"

CODE_ONLY=false
[[ "${1:-}" == "--code" ]] && CODE_ONLY=true

ACCOUNT_ID="$(aws sts get-caller-identity --query Account --output text)"

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
    --policy-document "file://$REPO_ROOT/infra/dynamodb-access-policy.json"
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

  echo "Creating public Function URL"
  aws lambda create-function-url-config \
    --function-name "$FUNCTION_NAME" \
    --auth-type NONE \
    --region "$REGION" >/dev/null

  aws lambda add-permission \
    --function-name "$FUNCTION_NAME" \
    --statement-id FunctionURLAllowPublicAccess \
    --action lambda:InvokeFunctionUrl \
    --principal "*" \
    --function-url-auth-type NONE \
    --region "$REGION" >/dev/null
fi

aws lambda update-function-configuration \
  --function-name "$FUNCTION_NAME" \
  --environment "$ENV_VARS" \
  --region "$REGION" >/dev/null

URL="$(aws lambda get-function-url-config --function-name "$FUNCTION_NAME" --region "$REGION" --query FunctionUrl --output text)"
echo
echo "Deployed: $URL"
