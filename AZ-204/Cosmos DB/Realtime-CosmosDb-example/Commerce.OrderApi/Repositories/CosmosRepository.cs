using Microsoft.Azure.Cosmos;

namespace Commerce.OrderApi.Repositories;

public class CosmosRepository<T> : ICosmosRepository<T> where T : class
{
    private readonly Container _container;

    public CosmosRepository(CosmosClient client, string databaseId, string containerId)
    {
        _container = client.GetContainer(databaseId, containerId);
    }

    public async Task<T> AddAsync(T item, PartitionKey partitionKey)
    {
        var response = await _container.CreateItemAsync(item, partitionKey);
        return response.Resource;
    }

    public async Task<T> GetAsync(string id, PartitionKey partitionKey)
    {
        var response = await _container.ReadItemAsync<T>(id, partitionKey);
        return response.Resource;
    }

    public async IAsyncEnumerable<T> QueryAsync(string query)
    {
        var iterator = _container.GetItemQueryIterator<T>(
            new QueryDefinition(query));

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            foreach (var item in response)
            {
                yield return item;
            }
        }
    }
}
