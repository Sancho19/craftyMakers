namespace TiaPlatform.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get; set; }

        public decimal Price { get; set; }
    }

}
