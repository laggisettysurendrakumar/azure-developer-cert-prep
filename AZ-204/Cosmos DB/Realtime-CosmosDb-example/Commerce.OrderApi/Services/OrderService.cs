using Commerce.OrderApi.Config;
using Commerce.OrderApi.Models;
using Commerce.OrderApi.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Commerce.OrderApi.Services;

public class OrderService
{
    private readonly Container _ordersContainer;
    private readonly Container _inventoryContainer;

    public OrderService(CosmosClient client, IOptions<CosmosDbConfig> options)
    {
        var config = options.Value;

        _ordersContainer = client.GetContainer(config.DatabaseId, config.OrdersContainerId);
        _inventoryContainer = client.GetContainer(config.DatabaseId, config.InventoryContainerId);
    }

    public async Task<Order> PlaceOrderAsync(Order order)
    {
        if (order == null || order.items == null || !order.items.Any())
            throw new ArgumentException("Order must contain at least one item.");

        // Group items by SKU
        var itemsBySku = order.items.GroupBy(i => i.skuId);

        // 1. Validate and update inventory per SKU using ETag + TransactionalBatch
        foreach (var skuGroup in itemsBySku)
        {
            string skuId = skuGroup.Key;
            int totalQtyRequested = skuGroup.Sum(i => i.quantity);

            ItemResponse<InventoryItem> inventoryResponse =
                await _inventoryContainer.ReadItemAsync<InventoryItem>(
                    id: skuId,
                    partitionKey: new PartitionKey(skuId));

            InventoryItem inventory = inventoryResponse.Resource;
            string etag = inventoryResponse.ETag;

            if (inventory.availableQuantity < totalQtyRequested)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for SKU {skuId}. Requested {totalQtyRequested}, available {inventory.availableQuantity}.");
            }

            inventory.availableQuantity -= totalQtyRequested;

            var batch = _inventoryContainer.CreateTransactionalBatch(new PartitionKey(skuId));

            batch.ReplaceItem(
                inventory.id,
                inventory,
                new TransactionalBatchItemRequestOptions
                {
                    IfMatchEtag = etag // optimistic concurrency
                });

            var batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                // In a real system: retry with backoff or surface proper error
                throw new InvalidOperationException(
                    $"Failed to update inventory for SKU {skuId}. Status: {batchResponse.StatusCode}");
            }
        }

        // 2. All inventory updates OK, now create order
        order.orderDate = DateTime.UtcNow;
        order.status = "Created";
        order.totalAmount = order.items.Sum(i => i.quantity * i.price);

        var orderResponse = await _ordersContainer.CreateItemAsync(
            order,
            new PartitionKey(order.customerId));

        return orderResponse.Resource;
    }
}
