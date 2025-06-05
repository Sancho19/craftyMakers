namespace TiaPlatform.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ResourceId { get; set; }
        public DateTime PurchaseDate { get; set; }

        public ApplicationUser User { get; set; }
        public Resource Resource { get; set; }
    }

}
