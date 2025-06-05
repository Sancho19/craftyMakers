namespace TiaPlatform.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get; set; }

        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }

}
