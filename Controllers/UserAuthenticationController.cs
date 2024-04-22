using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Models.DTO;
using DiplomWebShopVN.Repositories.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace DiplomWebShopVN.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _service;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthenticationController (RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, IUserAuthenticationService service)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._service = service;
        }
       
        public IActionResult Registration()
        {
            return View(new RegistrationModel());
        }

        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "UserAuthentication");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                TempData["Status"] = "Error from external provider!";
                return Redirect("/UserAuthentication/login");
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");
                TempData["Status"] = "Error loading external login information.";
                return Redirect("/UserAuthentication/login");
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                            Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                            Prizvyshe = info.Principal.FindFirstValue(ClaimTypes.Surname),   
                            
                        }; 
                                               
                          //role managment
                          var rolename = "user";
                          if (!await roleManager.RoleExistsAsync(rolename))
                          {
                              await roleManager.CreateAsync(new IdentityRole(rolename));
                          }
                          await userManager.CreateAsync(user);
                          if (await roleManager.RoleExistsAsync(rolename))
                          {
                            await userManager.AddToRoleAsync(user, rolename);
                          }

                    }
                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                TempData["Status"] = $"Email claim not received from: {info.LoginProvider}";
                return Redirect("/UserAuthentication/Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model, int? notused)
        {
            if (!ModelState.IsValid)
            {
              return View(model);
            }
            model.Role = "user"; 
                var result = await _service.RegistrationAsync(model);
            TempData["msg"] = result.Message;
            TempData["statuscode"] = result.StatusCode;
            //return RedirectToAction(nameof(Registration));
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _service.LoginAsync(model); 
            if(result.StatusCode == 1) {
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["Status"] = result.Message;
                return RedirectToAction(nameof(Login));  
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

/*        public async Task<IActionResult> Reg()
        {
            var model = new RegistrationModel
            {
                UserName = "Admin",
                Prizvyshe = "Vlasnyk",
                Name = "Administrator",
                Email = "Admin@gmail.com",
                Phone = "0971231212",
                Password = "Admin@12345#",
                PasswordConfirm = "Admin@12345#",
                Role = "admin"
            };
            var result = await _service.RegistrationAsync(model);
            return Ok(result);
        }*/

    }
}
