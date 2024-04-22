using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Models.DTO;
using DiplomWebShopVN.Models.ViewModels;
using DiplomWebShopVN.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DiplomWebShopVN.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthenticationService(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                status.StatusCode = 0;
                status.Message = "Невірне ім'я користувача.";
                return status; 
            }
            // match password
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Невірний пароль.";
                return status;
            }
            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName)
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Ласкаво просимо, "+ user.UserName+".";
                return status;
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User Locked Out.";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on Login In";
                return status;
            }
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync(); 
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExists =  await userManager.FindByNameAsync(model.UserName);
            if(userExists != null) {
                status.StatusCode = 0;
                status.Message = "Користувач з Ім'ям користувача: " + model.UserName + ", вже зареєстрований.";
                return status;
            }
            ApplicationUser user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Prizvyshe = model.Prizvyshe,
                PhoneNumber = model.Phone,
                Name = model.Name,
                Email = model.Email,
                UserName = model.UserName,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) {
                status.StatusCode = 0;
                status.Message = "Помилка реєстрації.";
                return status;
            }
            //role managment
            if(!await roleManager.RoleExistsAsync(model.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            if(await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }
            status.StatusCode = 1;
            status.Message = "Користувач успішно зареєструвався.";
            return status;
        }

        public async Task<Status> EditSelectedUserRoleAsync(AdminListUsersView model)
        {
            var user = userManager.Users.Where(u => u.Id == model.ListUsers[0].Id).First();
            var status = new Status();

            if (!await roleManager.RoleExistsAsync(model.ListRoles[0]))
            {
                await roleManager.CreateAsync(new IdentityRole(model.ListRoles[0]));
            }
            if (await roleManager.RoleExistsAsync(model.ListRoles[0]))
            {
                var userroles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, userroles);
                await userManager.AddToRoleAsync(user, model.ListRoles[0]);
                status.StatusCode = 1;
                status.Message = "User role changed successfullly.";
                return status;
            }
            status.StatusCode = 0;
            status.Message = "User role change error.";
            return status;

        }

    }
}
