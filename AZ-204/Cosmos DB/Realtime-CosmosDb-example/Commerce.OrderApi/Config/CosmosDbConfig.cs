namespace Commerce.OrderApi.Config
{
    public class CosmosDbConfig
    {
        public string AccountEndpoint { get; set; }
        public string AccountKey { get; set; }
        public string DatabaseId { get; set; }
        public string OrdersContainerId { get; set; }
        public string InventoryContainerId { get; set; }
        public string OrderEventsContainerId { get; set; }
    }
}
