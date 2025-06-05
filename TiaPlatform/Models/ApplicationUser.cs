using Microsoft.AspNetCore.Identity;
namespace TiaPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
