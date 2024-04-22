using DiplomWebShopVN.Models.DTO;
using DiplomWebShopVN.Models.ViewModels;

namespace DiplomWebShopVN.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task<Status> RegistrationAsync(RegistrationModel model);
        Task<Status> EditSelectedUserRoleAsync(AdminListUsersView model);
        Task LogoutAsync();
    }
}
