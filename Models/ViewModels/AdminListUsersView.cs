using DiplomWebShopVN.Models.Domain;

namespace DiplomWebShopVN.Models.ViewModels
{
    public class AdminListUsersView
    {
        public List<ApplicationUser> ListUsers { get; set; } = null!;
        public List<string> ListRoles { get; set; } = null!;
        public int object_number { get; set; }
    }
}
