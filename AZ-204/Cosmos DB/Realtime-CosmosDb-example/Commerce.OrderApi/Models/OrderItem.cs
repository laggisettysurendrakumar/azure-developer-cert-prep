namespace Commerce.OrderApi.Models
{
    public class OrderItem
    {
        public string skuId { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
