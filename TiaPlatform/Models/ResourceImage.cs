namespace TiaPlatform.Models
{
    public class ResourceImage
    {
        public int Id { get; set; }

        public int ResourceId { get; set; }
        public string ImagePath { get; set; }

        public Resource Resource { get; set; }
    }

}
