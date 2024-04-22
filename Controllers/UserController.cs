using DiplomWebShopVN.Models;
using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net.Mime;

namespace DiplomWebShopVN.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment hostEnvironment;


        public UserController(IWebHostEnvironment hostEnvironment, DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
        this.hostEnvironment = hostEnvironment;
        this.context = context;
        this.userManager = userManager;
        }   

        [HttpPost]
        public IActionResult OrderDetails(int id)
        {
            OrderDetailsView UserOrderDetails = new OrderDetailsView();
            UserOrderDetails.CurrentOrder = context.Orders.Include(u => u.User)
                .Where(o => o.Id == id).First();
            UserOrderDetails.OrderProducts = context.BasketElements.Include(p => p.ProductInBasket)
                .Where(o => o.OrderId == id).ToList();
            return View(UserOrderDetails);
        }
        // Order Status : new / active / in progress / completed / canceled .
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            Order OrderToCancel = context.Orders.Where(o => o.Id == id).First();
            if(OrderToCancel.Status == "new")
            {
                context.Remove(OrderToCancel);
                List<BasketElement> BasketProductToBack = context.BasketElements.Include(p=>p.ProductInBasket)
                    .Where(e=>e.OrderId == id).ToList();
                for(int i = 0; i < BasketProductToBack.Count; i++)
                {  
                    BasketProductToBack[i].ProductInBasket.Count += BasketProductToBack[i].Count;
                    context.Update(BasketProductToBack[i].ProductInBasket);
                    context.Remove(BasketProductToBack[i]);
                }
                context.SaveChanges();
                Console.WriteLine("!!!INFO!!! Order Deleted!");
                TempData["Status"] = "Замовлення номер "+id+", видалено.";
            }
            else
            {
                TempData["Status"] = "Замовлення номер " + id + ", не видалено. Замовлення вже взято в роботу.";
                Console.WriteLine("!!!ERROR!!! Order Status != 'NEW' ");
                return RedirectToAction(nameof(ListOrders));
            }          
            return RedirectToAction(nameof(ListOrders));
        }

        [HttpGet]
        public IActionResult ListOrders()
        {
            //[count of Basket add to Session if registred
            string Userid = userManager.GetUserId(User);
            int countbasket = context.BasketElements.Where(u => u.UserId == Userid && u.OrderId == 0).ToList().Count;
            Url.ActionContext.HttpContext.Session.SetInt32("basketcount", countbasket);
            //count of Basket add to Session if registred]
            List<Order> Orders = context.Orders.Where(o => o.UserId == userManager.GetUserId(User)).ToList();
            return View(Orders);
        }

        public int GetDiscontProcent(string UserId)
        {
            List<Order> AllOrders = context.Orders.Where(u => u.UserId == UserId && u.Status.ToLower() == "complited").ToList();
            int allOrdersPrice = 0;
            int UserDiscont = 1;
            if (AllOrders.Count > 0)
            {
                foreach (var order in AllOrders)
                {
                    allOrdersPrice += (int)order.OrderPrice;
                }
            }
            if (allOrdersPrice >= 1000)
            {
                UserDiscont = 2;
            }
            if (allOrdersPrice >= 3000)
            {
                    UserDiscont = 3;
            }
            if (allOrdersPrice >= 5000)
            {
                UserDiscont = 5;
            }
            //Console.WriteLine("!!!!USER DISCONT = " + UserDiscont);
            return UserDiscont;
        }

        [HttpGet]
        public async Task<IActionResult> UserPanel()
        {
            ApplicationUser currentUser = await userManager.GetUserAsync(User);
            currentUser.Discont = GetDiscontProcent(currentUser.Id);
            return View(currentUser);  
        }

        [HttpGet]
        public async Task<IActionResult> EditUserProfile()
        {
            ApplicationUser currentUser = await userManager.GetUserAsync(User);
            return View(currentUser);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(ApplicationUser model, IFormFileCollection Photo)
        {
            ApplicationUser EditedUser = await userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if(EditedUser.Id == model.Id)
                {
                    if(Photo != null && Photo.Count > 0)
                    {               
                        string path = "/Images/" + Photo[0].FileName;
                        using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await Photo[0].CopyToAsync(fs);
                        }
                        EditedUser.ProfilePicture = Photo[0].FileName;
                    }       
                    EditedUser.Prizvyshe = model.Prizvyshe;
                    EditedUser.Name = model.Name;                
                    EditedUser.Email = model.Email;
                    EditedUser.PhoneNumber = model.PhoneNumber;
                    context.Update(EditedUser);
                    context.SaveChanges();
                    TempData["Status"] = "Дані користувача оновлено.";
                        //Update Profile Image In Session
                        if (!string.IsNullOrEmpty(EditedUser.ProfilePicture))
                        {
                            Url.ActionContext.HttpContext.Session.SetString("userimg", EditedUser.ProfilePicture);
                        }
                        else
                        {
                            Url.ActionContext.HttpContext.Session.Remove("userimg");
                        }                    
                }
                else 
                {
                    Console.WriteLine("!!!ERROR ERROR FIND USER!!!! SENDED USER ID != CURENT USER ID!!!");
                    return RedirectToAction(nameof(UserPanel));
                }
            }
            else
            {
                TempData["Status"] = "Model state is Invalid!";
                return View(model); 
            }
            return RedirectToAction(nameof(UserPanel));
        }


    }
  
}
