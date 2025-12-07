using Microsoft.Azure.Cosmos;

namespace Commerce.OrderApi.Repositories
{
    public interface ICosmosRepository<T> where T : class
    {
        Task<T> AddAsync(T item, PartitionKey partitionKey);
        Task<T> GetAsync(string id, PartitionKey partitionKey);
        IAsyncEnumerable<T> QueryAsync(string query);
    }

}
