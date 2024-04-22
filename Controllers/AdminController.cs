using DiplomWebShopVN.Models;
using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Models.ViewModels;
using DiplomWebShopVN.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomWebShopVN.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IUserAuthenticationService _service;
        private readonly DatabaseContext context;
        //private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController (IUserAuthenticationService service, DatabaseContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._service = service;
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult DeleteUser(string id)
        {
            try
            {
                if (id == null)
                {
                    Console.WriteLine("INFO MESSAGE : ID = NULL");
                    TempData["msg"] = "Error (Null) user id.";
                    TempData["statuscode"] = 0;
                    return RedirectToAction(nameof(AdminPanel));
                }
                var targetUser = userManager.Users.Where(u => u.Id == id).First();
                userManager.DeleteAsync(targetUser);
                TempData["Status"] = "Користувача: "+ targetUser.UserName + ", видалено.";
                TempData["statuscode"] = 1;
            }
            catch(Exception ex){
                TempData["Status"] = ex.Message;
            }
            return Redirect("/Admin/AdminPanel");                    
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(AdminListUsersView model)
        {
            
            if (model.ListRoles[0] == null || model.ListUsers[0].Id == null)
            {
                Console.WriteLine("INFO MESSAGE : MODEL STATE IS INVALID");
                TempData["msg"] = "Error change user role.";
                TempData["statuscode"] = 0;
                return RedirectToAction(nameof(AdminPanel));
            }
            var result = await _service.EditSelectedUserRoleAsync(model);
            TempData["Status"] = result.Message;
            return RedirectToAction(nameof(AdminPanel));
        }
        
        
        [HttpGet]
        public IActionResult AdminPanel()
        {
            var listusers = userManager.Users.ToList();//get all users to list

            List<string> lroles = new List<string>();// list roles from all users
            
            foreach (var userr in listusers)//get list roles from all users
            {
                try
                {
                string Role = userManager.GetRolesAsync(userr).Result.ElementAt(0);
                    if (!String.IsNullOrEmpty(Role))
                    {
                        lroles.Add(Role);
                    }
                }
                catch (Exception ex)
                {
                  lroles.Add("");
                }                                
            }
            AdminListUsersView adminListUsers = new AdminListUsersView
            {
                ListUsers = listusers, ListRoles = lroles
            };

            return View(adminListUsers);
        }
    }
}
