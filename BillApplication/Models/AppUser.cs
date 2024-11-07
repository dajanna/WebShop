using Microsoft.AspNetCore.Identity;

namespace BillApplication.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
