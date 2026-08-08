using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace GuitarStore.Web.Data;

/// <summary>
/// Creates the DynamoDB tables if they're missing. Safe to run on every startup — it checks
/// first and does nothing when the tables already exist. Intended for local development and
/// first-run provisioning; production tables are created by infra/deploy.sh.
/// </summary>
public class DynamoDbInitializer
{
    private readonly IAmazonDynamoDB _client;
    private readonly ILogger<DynamoDbInitializer> _logger;

    public DynamoDbInitializer(IAmazonDynamoDB client, ILogger<DynamoDbInitializer> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task EnsureTablesAsync(CancellationToken ct = default)
    {
        var existing = (await _client.ListTablesAsync(ct)).TableNames;

        await CreateIfMissingAsync(existing, new CreateTableRequest
        {
            TableName = "GuitarStore-Products",
            BillingMode = BillingMode.PAY_PER_REQUEST,
            AttributeDefinitions =
            [
                new AttributeDefinition("Id", ScalarAttributeType.N),
                new AttributeDefinition("ProductTypeName", ScalarAttributeType.S)
            ],
            KeySchema = [new KeySchemaElement("Id", KeyType.HASH)],
            GlobalSecondaryIndexes =
            [
                new GlobalSecondaryIndex
                {
                    IndexName = "ProductTypeName-index",
                    KeySchema = [new KeySchemaElement("ProductTypeName", KeyType.HASH)],
                    Projection = new Projection { ProjectionType = ProjectionType.ALL }
                }
            ]
        }, ct);

        // Cart items live alongside their cart record: SK "META" is the cart itself,
        // "ITEM#<productId>" is each line. One Query by CartId returns the whole cart.
        await CreateIfMissingAsync(existing, new CreateTableRequest
        {
            TableName = "GuitarStore-Carts",
            BillingMode = BillingMode.PAY_PER_REQUEST,
            AttributeDefinitions =
            [
                new AttributeDefinition("CartId", ScalarAttributeType.S),
                new AttributeDefinition("SortKey", ScalarAttributeType.S)
            ],
            KeySchema =
            [
                new KeySchemaElement("CartId", KeyType.HASH),
                new KeySchemaElement("SortKey", KeyType.RANGE)
            ]
        }, ct);

        await CreateIfMissingAsync(existing, new CreateTableRequest
        {
            TableName = "GuitarStore-Orders",
            BillingMode = BillingMode.PAY_PER_REQUEST,
            AttributeDefinitions = [new AttributeDefinition("TrackingNumber", ScalarAttributeType.S)],
            KeySchema = [new KeySchemaElement("TrackingNumber", KeyType.HASH)]
        }, ct);

        await CreateIfMissingAsync(existing, new CreateTableRequest
        {
            TableName = "GuitarStore-Employees",
            BillingMode = BillingMode.PAY_PER_REQUEST,
            AttributeDefinitions = [new AttributeDefinition("EmpId", ScalarAttributeType.N)],
            KeySchema = [new KeySchemaElement("EmpId", KeyType.HASH)]
        }, ct);
    }

    private async Task CreateIfMissingAsync(List<string> existing, CreateTableRequest request, CancellationToken ct)
    {
        if (existing.Contains(request.TableName))
        {
            return;
        }

        _logger.LogInformation("Creating DynamoDB table {TableName}", request.TableName);
        await _client.CreateTableAsync(request, ct);
        await WaitUntilActiveAsync(request.TableName, ct);
    }

    private async Task WaitUntilActiveAsync(string tableName, CancellationToken ct)
    {
        for (var attempt = 0; attempt < 30; attempt++)
        {
            var description = await _client.DescribeTableAsync(tableName, ct);
            if (description.Table.TableStatus == TableStatus.ACTIVE)
            {
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(1), ct);
        }

        throw new TimeoutException($"Table {tableName} did not become ACTIVE in time.");
    }
}
