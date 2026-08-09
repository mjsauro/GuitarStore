#!/usr/bin/env bash
# One-time setup so GitHub Actions can deploy without any stored AWS credentials.
#
# Creates an IAM role that GitHub Actions assumes over OIDC. The role can update this
# one Lambda's code and configuration, and read the API endpoint — nothing else. No
# access keys are created, so there's nothing to rotate or leak.
#
#   ./infra/setup-github-oidc.sh
#
# Environment:
#   AWS_REGION      deployment region (default us-east-2)
#   GITHUB_REPO     owner/repo (default mjsauro/GuitarStore)
#   DEPLOY_BRANCH   branch allowed to deploy, besides master (default rebuild/dotnet10-aws)
set -euo pipefail

REGION="${AWS_REGION:-us-east-2}"
GITHUB_REPO="${GITHUB_REPO:-mjsauro/GuitarStore}"
DEPLOY_BRANCH="${DEPLOY_BRANCH:-rebuild/dotnet10-aws}"
ROLE_NAME="guitarstore-github-actions"
PROVIDER_HOST="token.actions.githubusercontent.com"
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

ACCOUNT_ID="$(aws sts get-caller-identity --query Account --output text)"
PROVIDER_ARN="arn:aws:iam::${ACCOUNT_ID}:oidc-provider/${PROVIDER_HOST}"

RENDERED="$(mktemp -d)"
trap 'rm -rf "$RENDERED"' EXIT

render() {
  local output="$RENDERED/$1.json"
  sed -e "s|\${ACCOUNT_ID}|${ACCOUNT_ID}|g" \
      -e "s|\${REGION}|${REGION}|g" \
      -e "s|\${GITHUB_REPO}|${GITHUB_REPO}|g" \
      -e "s|\${DEPLOY_BRANCH}|${DEPLOY_BRANCH}|g" \
      "$REPO_ROOT/infra/$1.template.json" > "$output"
  echo "$output"
}

# ---------------------------------------------------------------- OIDC provider
if ! aws iam get-open-id-connect-provider --open-id-connect-provider-arn "$PROVIDER_ARN" >/dev/null 2>&1; then
  echo "Creating GitHub OIDC provider"
  aws iam create-open-id-connect-provider \
    --url "https://${PROVIDER_HOST}" \
    --client-id-list sts.amazonaws.com >/dev/null
else
  echo "GitHub OIDC provider already present"
fi

# ---------------------------------------------------------------- role
TRUST="$(render github-actions-trust-policy)"

if aws iam get-role --role-name "$ROLE_NAME" >/dev/null 2>&1; then
  echo "Updating trust policy on $ROLE_NAME"
  aws iam update-assume-role-policy \
    --role-name "$ROLE_NAME" \
    --policy-document "file://$TRUST"
else
  echo "Creating role $ROLE_NAME"
  aws iam create-role \
    --role-name "$ROLE_NAME" \
    --assume-role-policy-document "file://$TRUST" \
    --description "Lets GitHub Actions deploy GuitarStore.Web" >/dev/null
fi

echo "Applying deploy policy"
aws iam put-role-policy \
  --role-name "$ROLE_NAME" \
  --policy-name GuitarStoreDeploy \
  --policy-document "file://$(render github-actions-deploy-policy)"

cat <<EOF

Done. Add this repository secret in GitHub so the workflow can find the role
(Settings > Secrets and variables > Actions > New repository secret):

  Name:  AWS_ACCOUNT_ID
  Value: ${ACCOUNT_ID}

Deploys are allowed from: master and ${DEPLOY_BRANCH}
EOF
