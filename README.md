# Matt's Guitar Store

My bootcamp final project from 2017, and a rebuild of it from 2026 — kept side by side in
one repository so the two can be compared directly.

**[Live demo →](https://ff5r1kiiae.execute-api.us-east-2.amazonaws.com)**
*(The first request after a quiet spell takes a second or two while Lambda cold-starts.)*

Browse the catalog, add a guitar to the cart, and check out with the test card
`4242 4242 4242 4242`. Payments are simulated — nothing is charged and no card data is
stored.

## The two versions

The original was an ASP.NET MVC 5 application on .NET Framework 4.6.1: Entity Framework 6
against SQL Server LocalDB, ASP.NET Identity over OWIN, Braintree for payments, Mailgun for
email. It ran on Windows and IIS, and only there.

The rebuild is the same store — same catalog, same cart, same checkout, same back office —
on a stack that runs anywhere and deploys itself.

| | 2017 | 2026 |
| --- | --- | --- |
| Framework | ASP.NET MVC 5 on .NET Framework 4.6.1 | ASP.NET Core MVC on .NET 10 |
| Data | Entity Framework 6 + SQL Server LocalDB | DynamoDB |
| Auth | ASP.NET Identity + OWIN, hand-written views | Cognito hosted UI over OIDC |
| Payments | Braintree, raw card data posted to the server | `IPaymentService`, card data never stored |
| Email | Mailgun calls copy-pasted into controllers | One `IEmailSender` on SES |
| Hosting | Windows + IIS | Lambda behind API Gateway |
| Deployment | Manual | GitHub Actions on push |

## How it runs

```
API Gateway (HTTP API)  ->  Lambda (.NET 10, arm64)  ->  DynamoDB
                                    |
                                    +-> Cognito  (sign-in, registration, password reset)
                                    +-> SES      (order receipts)
```

The Lambda is private — its resource policy admits exactly one principal, API Gateway,
scoped to this API. GitHub Actions deploys over OIDC, so no AWS credentials are stored
anywhere in GitHub.

## Data modeling

Moving from a relational schema to DynamoDB meant rethinking the shapes rather than
transliterating tables:

- The `ProductTypeProperty` / `ProductTypePropertyValue` pair — an EAV pattern for
  attributes like a guitar's pickup style or an amp's wattage — collapsed into a single map
  attribute on the product item.
- `Cart` and `CartProducts` became one item collection under a shared partition key, so the
  whole cart comes back in a single query instead of a join.
- `Order` and `OrderProducts` became one item with its line items embedded. They were
  already point-in-time snapshots in the old schema, so later catalog edits never rewrite
  someone's receipt.

## What the old code got wrong

Reviewing the original before rebuilding turned up real problems, each fixed by
construction rather than patched:

- **SQL injection** in the sales report, which concatenated a query-string value straight
  into a SQL statement. That page now aggregates the store's own orders and accepts no
  query input at all.
- **No authorization whatsoever** on the product-admin and employee controllers. Anyone who
  knew the URL could create, edit, or delete products — or read and modify staff records
  including dates of birth and wages. Both now require an admin role.
- **Missing CSRF tokens** on several POST actions, including the one that charged a card.
  Antiforgery validation is now global rather than remembered per action.
- **Unhandled exceptions** on missing records, which surfaced as 500s. Now 404s.
- **Raw card numbers and CVVs** flowing through the application's own models. Payments now
  sit behind an interface, and only the last four digits are ever persisted.

## Layout

```
GuitarStore/        the 2017 application, unchanged
GuitarStoreDB/      its SQL Server schema
GuitarStoreTests/   its tests
GuitarStore.Web/    the 2026 rebuild
infra/              deploy scripts and IAM policy templates
```

## Running it locally

Needs the .NET 10 SDK and a JRE. No AWS account required — the app falls back to a local
sign-in stub and logs receipts instead of sending them.

```bash
# DynamoDB Local, once
curl -L -o /tmp/ddb.tar.gz https://d1ni2b6xgvw0s0.cloudfront.net/v2.x/dynamodb_local_latest.tar.gz
mkdir -p ~/.dynamodb-local && tar xzf /tmp/ddb.tar.gz -C ~/.dynamodb-local
cd ~/.dynamodb-local && java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb -port 8000 &

# The app — tables are created and the catalog seeded on first run
cd GuitarStore.Web && dotnet run
```

Then open <http://localhost:5168>. Visit `/DevAuth` to sign in as an administrator; that
endpoint stands in for Cognito and is unreachable outside the Development environment.

See [`GuitarStore.Web/README.md`](GuitarStore.Web/README.md) for configuration and
deployment details.
