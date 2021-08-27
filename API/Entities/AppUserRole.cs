using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // Table for mapping many to many realation
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole AppRole { get; set; }
    }
}