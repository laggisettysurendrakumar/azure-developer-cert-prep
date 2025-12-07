namespace Commerce.OrderApi.Models
{
    public class OrderEvent
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string orderId { get; set; }
        public string eventType { get; set; }   // Created, Paid, Shipped, etc.
        public DateTime eventTime { get; set; } = DateTime.UtcNow;
        public string description { get; set; }
    }

}
