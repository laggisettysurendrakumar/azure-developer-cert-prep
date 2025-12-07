namespace Commerce.OrderApi.Models
{
    public class Order
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string customerId { get; set; }
        public DateTime orderDate { get; set; } = DateTime.UtcNow;
        public string status { get; set; } = "Created"; // Created, Paid, Shipped, Cancelled
        public decimal totalAmount { get; set; }
        public List<OrderItem> items { get; set; } = new();
    }

}
