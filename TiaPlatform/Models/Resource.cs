using System.ComponentModel.DataAnnotations.Schema;

namespace TiaPlatform.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        // Navigation
        public ICollection<ResourceImage> Images { get; set; }

    }

}
