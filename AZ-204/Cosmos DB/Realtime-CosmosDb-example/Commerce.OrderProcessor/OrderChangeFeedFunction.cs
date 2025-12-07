using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Commerce.OrderProcessor.Functions;

public class OrderDocument
{
    public string id { get; set; } = default!;
    public string customerId { get; set; } = default!;
    public string status { get; set; } = default!;
}

public class OrderChangeFeedFunction
{
    private readonly ILogger _logger;

    public OrderChangeFeedFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<OrderChangeFeedFunction>();
    }

    [Function("OrderChangeFeedFunction")]
    public void Run(
        [CosmosDBTrigger(
            databaseName: "CommerceDb",
            containerName: "Orders",
            Connection = "CosmosDbConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true
            )]
        IReadOnlyList<OrderDocument> input)
    {
        if (input == null || input.Count == 0)
        {
            return;
        }

        _logger.LogInformation($"Order change feed triggered with {input.Count} documents");

        foreach (var doc in input)
        {
            _logger.LogInformation(
                $"Order {doc.id} for customer {doc.customerId} changed. Status: {doc.status}");
        }
    }
}
