using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbConsoleDemo;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;

internal class Program
{
    // TODO: Replace these with your values from Azure Portal
    private static readonly string endpointUri = "<Copy endpointUri from Azure portal>";
    private static readonly string primaryKey = "<Copy primaryKey from Azure portal>";
    private const string DatabaseId = "DemoDatabase";
    private const string ContainerId = "TodoItems";
    private const string PartitionKeyPath = "/Category";

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Azure Cosmos DB .NET Console Demo");
        Console.WriteLine("Connecting...");

        // Create the Cosmos client
        using CosmosClient client = new CosmosClient(endpointUri, primaryKey);

        // 1. Create database (if it doesn’t exist)
        Database database = await client.CreateDatabaseIfNotExistsAsync(DatabaseId);
        Console.WriteLine($"✅ Database ready: {database.Id}");

        // 2. Create container (if it doesn’t exist)
        Container container = await database.CreateContainerIfNotExistsAsync(
            id: ContainerId,
            partitionKeyPath: PartitionKeyPath,
            throughput: 400   // RU/s
        );
        Console.WriteLine($"✅ Container ready: {container.Id}");

        // 3. Insert sample data
        Console.WriteLine("Inserting sample items...");
        await InsertSampleItemsAsync(container);

        // 4. Read all items and show count
        Console.WriteLine("Reading items back from container...");
        List<TodoItem> items = await ReadAllItemsAsync(container);

        Console.WriteLine("📦 Items in container:");
        foreach (var item in items)
        {
            Console.WriteLine($"- {item.id} | {item.Title} | {item.Status} | {item.Category}");
        }

        Console.WriteLine($"\n✅ Total records in container: {items.Count}");
    }

    private static async Task InsertSampleItemsAsync(Container container)
    {
        // Simple sample data
        var todos = new List<TodoItem>
            {
                new TodoItem { Title = "Learn Azure Fundamentals", Status = "New",        Category = "Learning" },
                new TodoItem { Title = "Build Cosmos DB console app", Status = "InProgress", Category = "Coding" },
                new TodoItem { Title = "Prepare for AZ-900",          Status = "New",        Category = "Learning" },
                new TodoItem { Title = "Prepare for AZ-204",          Status = "New",        Category = "Learning" },
            };

        foreach (var todo in todos)
        {
            ItemResponse<TodoItem> response = await container.CreateItemAsync(
                item: todo,
                partitionKey: new PartitionKey(todo.Category)
            );

            Console.WriteLine($"   Inserted item with id: {response.Resource.id}, RU charge: {response.RequestCharge}");
        }
    }

    private static async Task<List<TodoItem>> ReadAllItemsAsync(Container container)
    {
        var sqlQueryText = "SELECT * FROM c";

        QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
        FeedIterator<TodoItem> queryResultSetIterator = container.GetItemQueryIterator<TodoItem>(queryDefinition);

        List<TodoItem> results = new List<TodoItem>();

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<TodoItem> currentResultSet = await queryResultSetIterator.ReadNextAsync();
            results.AddRange(currentResultSet);
        }

        return results;
    }
}
