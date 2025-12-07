namespace Commerce.OrderApi.Models
{
    public class InventoryItem
    {
        public string id { get; set; }          // same as skuId
        public string skuId { get; set; }
        public int availableQuantity { get; set; }
        public int reorderThreshold { get; set; } = 10;
        public string warehouseLocation { get; set; }
    }
}
