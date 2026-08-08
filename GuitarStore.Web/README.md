# GuitarStore.Web

A rebuild of the original 2017 GuitarStore (ASP.NET MVC 5 / .NET Framework 4.6.1 / Entity
Framework 6 / SQL Server LocalDB, still in `../GuitarStore`) on ASP.NET Core MVC and
DynamoDB, targeting AWS App Runner.

## What changed and why

| Original | Now | Why |
| --- | --- | --- |
| ASP.NET MVC 5 on .NET Framework 4.6.1 | ASP.NET Core MVC on .NET 10 | Windows/IIS-only → runs anywhere |
| Entity Framework 6 + SQL Server LocalDB | DynamoDB | Tiny dataset, near-zero traffic; free tier doesn't expire the way an idle RDS instance bills forever |
| `ProductTypeProperty` / `ProductTypePropertyValue` EAV tables | `Properties` map on the product item | Two joins collapse into one attribute |
| `Cart` + `CartProducts` join | One item collection per cart | Whole cart in a single Query |
| `Order` + `OrderProducts` join | Line items embedded on the order | They were already purchase-time snapshots |
| Braintree with raw card data posted to the server | `IPaymentService` with a simulated processor | Demoable with no merchant account; a real provider drops in behind the interface |
| Ad-hoc Mailgun calls copy-pasted into controllers | (pending) single `IEmailSender` on SES | One implementation instead of three |
| ASP.NET Identity + OWIN | (pending) Cognito via OIDC | Less hand-rolled auth to get wrong |

Security gaps found in the original and closed here: SQL injection in the sales report
(that page now aggregates this store's own orders and takes no query input), missing
`[Authorize]` on the product-admin and employee controllers, missing antiforgery tokens
(now applied globally rather than per action), and unhandled exceptions on missing records
(now 404s).

## Running locally

Needs the .NET 10 SDK and a JRE (for DynamoDB Local).

```bash
# 1. Start DynamoDB Local (once)
curl -L -o /tmp/ddb.tar.gz https://d1ni2b6xgvw0s0.cloudfront.net/v2.x/dynamodb_local_latest.tar.gz
mkdir -p ~/.dynamodb-local && tar xzf /tmp/ddb.tar.gz -C ~/.dynamodb-local
cd ~/.dynamodb-local && java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb -port 8000 &

# 2. Run the app — tables are created and the catalog seeded on first start
cd GuitarStore.Web
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

Then open <http://localhost:5199>.

To reach the admin screens locally, visit `/DevAuth` and sign in as an administrator. That
endpoint stands in for Cognito and is unreachable outside the Development environment (the
route isn't mapped, and the controller refuses to act).

Checkout uses a simulated payment processor — **don't enter a real card**. Use
`4242 4242 4242 4242` for an approval and `4000 0000 0000 0002` for a decline.

## Building the container

Docker isn't required; the SDK can build the image itself.

```bash
dotnet publish -c Release /t:PublishContainer \
  /p:ContainerArchiveOutputPath=/tmp/guitarstore-image.tar.gz \
  /p:ContainerRepository=guitarstore-web
```

A `Dockerfile` is included for `docker build` if you'd rather use it.

## Configuration

| Setting | Local | Deployed |
| --- | --- | --- |
| `AWS:DynamoDbServiceUrl` | `http://localhost:8000` | unset — uses the real service |
| `AWS:Region` | `us-east-1` | region of the deployment |
| Credentials | not needed (DynamoDB Local ignores them) | App Runner instance role |
