using Microsoft.AspNetCore.Identity;

namespace DiplomWebShopVN.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? Prizvyshe { get; set; }
        public string Name { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public int Discont { get; set; } = 0;
    }
}
