# GuitarStore.Web

A rebuild of the original 2017 GuitarStore (ASP.NET MVC 5 / .NET Framework 4.6.1 / Entity
Framework 6 / SQL Server LocalDB, still in `../GuitarStore`) on ASP.NET Core MVC and
DynamoDB, deployed to AWS Lambda.

## What changed and why

| Original | Now | Why |
| --- | --- | --- |
| ASP.NET MVC 5 on .NET Framework 4.6.1 | ASP.NET Core MVC on .NET 10 | Windows/IIS-only → runs anywhere |
| Entity Framework 6 + SQL Server LocalDB | DynamoDB | Tiny dataset, near-zero traffic; free tier doesn't expire the way an idle RDS instance bills forever |
| `ProductTypeProperty` / `ProductTypePropertyValue` EAV tables | `Properties` map on the product item | Two joins collapse into one attribute |
| `Cart` + `CartProducts` join | One item collection per cart | Whole cart in a single Query |
| `Order` + `OrderProducts` join | Line items embedded on the order | They were already purchase-time snapshots |
| Braintree with raw card data posted to the server | `IPaymentService` with a simulated processor | Demoable with no merchant account; a real provider drops in behind the interface |
| Ad-hoc Mailgun calls copy-pasted into controllers | Single `IEmailSender` on SES | One implementation instead of three |
| ASP.NET Identity + OWIN | Cognito hosted UI via OIDC | Seven hand-written auth views deleted, not ported |
| Password minimum of 4 characters, no complexity | 8+, mixed case and digits | Enforced by Cognito |

Security gaps found in the original and closed here: SQL injection in the sales report
(that page now aggregates this store's own orders and takes no query input), missing
`[Authorize]` on the product-admin and employee controllers, missing antiforgery tokens
(now applied globally rather than per action), and unhandled exceptions on missing records
(now 404s).

## Running locally

Needs the .NET 10 SDK and a JRE (for DynamoDB Local).

In VS Code, press **F5** — that starts the database, builds, and launches with the
debugger attached. From a terminal:

```bash
# 1. Start DynamoDB Local (downloads it on first use; safe to re-run)
./infra/dynamodb-local.sh

# 2. Run the app — tables are created and the catalog seeded on first start
cd GuitarStore.Web
dotnet run
```

Then open <http://localhost:5168>.

To reach the admin screens locally, visit `/DevAuth` and sign in as an administrator. That
endpoint stands in for Cognito and is unreachable outside the Development environment (the
route isn't mapped, and the controller refuses to act).

Checkout uses a simulated payment processor — **don't enter a real card**. Use
`4242 4242 4242 4242` for an approval and `4000 0000 0000 0002` for a decline.

## Deployed

Live at <https://ff5r1kiiae.execute-api.us-east-2.amazonaws.com>.

```
API Gateway (HTTP API)  ->  Lambda (dotnet10, arm64)  ->  DynamoDB (4 tables)
                                    |
                                    +-> Cognito (sign-in, hosted UI)
                                    +-> SES (order receipts)
```

Sign-in, registration, and password reset are Cognito's hosted UI. Membership of the
Cognito `Admin` group arrives as a role claim and gates the catalog, employee, and
report screens.

SES is in the sandbox, so receipts only reach verified addresses — enough for a demo.
Moving out of the sandbox requires a support request.

Redeploy with `./infra/deploy.sh` (full) or `./infra/deploy.sh --code` (code only):

```bash
AWS_REGION=us-east-2 SES_IDENTITY=you@example.com ./infra/deploy.sh
```

The IAM policies are rendered at deploy time from the `.template.json` files — the
account id comes from the caller's own identity and the sender address from
`SES_IDENTITY`, so neither is committed. With `SES_IDENTITY` unset the SES policy is
skipped and receipts are logged rather than sent.

The Lambda is private: its resource policy allows exactly one principal —
`apigateway.amazonaws.com`, restricted to this API's ARN. There's no function URL.

A note on why it isn't a Lambda function URL, since that's the more obvious choice:
accounts created after ~2024 block public access to function URLs by default. Fronting
one with CloudFront + origin access control gets GETs working but breaks every form
post, because OAC signs origin requests with SigV4 and Lambda rejects unsigned
payloads — a browser POST with no `x-amz-content-sha256` header gets a 403. API Gateway
invokes Lambda directly and sidesteps this entirely.

## Configuration

| Setting | Local | Deployed |
| --- | --- | --- |
| `AWS:DynamoDbServiceUrl` | `http://localhost:8000` | unset — uses the real service |
| `AWS:Region` | `us-east-1` | region of the deployment |
| Credentials | not needed (DynamoDB Local ignores them) | Lambda execution role |
| `App:PublicOrigin` | unset | public URL, used to build OIDC redirects |
| `Cognito:*` | unset — falls back to `/DevAuth` | user pool, client, domain; secret set as Lambda config |
| `Email:FromAddress` | unset — receipts are logged, not sent | SES-verified sender |

Nothing secret is committed. The Cognito client secret is set directly as Lambda
environment configuration, and `deploy.sh` merges rather than replaces that block so a
code push can't wipe it.
