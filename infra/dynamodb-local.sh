#!/usr/bin/env bash
# Starts DynamoDB Local for development, downloading it on first use.
#
# Idempotent: if it's already listening on the port, this does nothing and exits 0, so
# it's safe to run before every debug session.
#
#   ./infra/dynamodb-local.sh          # start (or confirm already running)
#   ./infra/dynamodb-local.sh status   # report without starting
#   ./infra/dynamodb-local.sh stop     # stop it
set -euo pipefail

PORT="${DYNAMODB_LOCAL_PORT:-8000}"
INSTALL_DIR="${DYNAMODB_LOCAL_HOME:-$HOME/.dynamodb-local}"
DOWNLOAD_URL="https://d1ni2b6xgvw0s0.cloudfront.net/v2.x/dynamodb_local_latest.tar.gz"

is_running() {
  curl -s -o /dev/null --max-time 3 "http://localhost:${PORT}" 2>/dev/null
}

case "${1:-start}" in
  status)
    if is_running; then
      echo "DynamoDB Local is running on port ${PORT}"
    else
      echo "DynamoDB Local is not running"
      exit 1
    fi
    exit 0
    ;;
  stop)
    if pkill -f "DynamoDBLocal.jar.*-port ${PORT}"; then
      echo "Stopped DynamoDB Local"
    else
      echo "DynamoDB Local was not running"
    fi
    exit 0
    ;;
esac

if is_running; then
  echo "DynamoDB Local already running on port ${PORT}"
  exit 0
fi

if ! command -v java >/dev/null 2>&1; then
  echo "Java is required to run DynamoDB Local. Install a JRE (e.g. 'brew install openjdk') and retry." >&2
  exit 1
fi

if [[ ! -f "$INSTALL_DIR/DynamoDBLocal.jar" ]]; then
  echo "Downloading DynamoDB Local to $INSTALL_DIR"
  mkdir -p "$INSTALL_DIR"
  curl -fsSL -o "$INSTALL_DIR/dynamodb_local_latest.tar.gz" "$DOWNLOAD_URL"
  tar xzf "$INSTALL_DIR/dynamodb_local_latest.tar.gz" -C "$INSTALL_DIR"
fi

echo "Starting DynamoDB Local on port ${PORT}"
cd "$INSTALL_DIR"
nohup java -Djava.library.path=./DynamoDBLocal_lib \
  -jar DynamoDBLocal.jar -sharedDb -port "${PORT}" \
  > "$INSTALL_DIR/dynamodb-local.log" 2>&1 &

for _ in $(seq 1 20); do
  if is_running; then
    echo "DynamoDB Local is up (log: $INSTALL_DIR/dynamodb-local.log)"
    exit 0
  fi
  sleep 0.5
done

echo "DynamoDB Local did not come up. Check $INSTALL_DIR/dynamodb-local.log" >&2
exit 1
