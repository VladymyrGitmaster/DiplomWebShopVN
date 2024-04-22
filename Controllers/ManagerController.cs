
using DiplomWebShopVN.Models;
using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Models.ViewModels;
using DiplomWebShopVN.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace DiplomWebShopVN.Controllers
{
    [Authorize(Roles = "manager")]
    public class ManagerController : Controller
    {
        private readonly IUserAuthenticationService _service;
        private readonly DatabaseContext context;
        //private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment hostEnvironment;

        public ManagerController(IWebHostEnvironment hostEnvironment, IUserAuthenticationService service, DatabaseContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.hostEnvironment = hostEnvironment;
            this._service = service;
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        public IActionResult AddProduct(Product model)
        {
            Product prodForUpdate = context.Products.Where(p => p.Id == model.Id).First();
            prodForUpdate.Count = model.Count;
            prodForUpdate.Price = model.Price;
            context.Update(prodForUpdate);
            context.SaveChanges();
            return Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            Product product = context.Products.Include(p => p.ProductCategory)
            .Include(d => d.Photos)
            .Where(i => i.Id == id).First();
            ViewBag.ListCategories = context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct([Bind("Id,Name,ShortDescription,LongDescription,CategoryId")] Product model, IFormFileCollection Photos)
        {
            if (ModelState.IsValid)
            {
                Product PrforUpdate = context.Products.Where(p => p.Id == model.Id).First();
                PrforUpdate.Name = model.Name;
                PrforUpdate.ShortDescription = model.ShortDescription;
                PrforUpdate.LongDescription = model.LongDescription;
                PrforUpdate.CategoryId = model.CategoryId;
                PrforUpdate.ProductCategory = context.Categories.Where(c => c.Id == model.CategoryId).First();
                if (Photos != null && Photos.Count > 0)
                {
                    //[clear old photos
                    context.Photos.RemoveRange(context.Photos.Where(f => f.ProductId == model.Id).ToList());
                    context.SaveChanges();
                    //clear old photos]
                    PrforUpdate.Image = Photos[0].FileName;
                    foreach (IFormFile file in Photos)
                    {
                        string path = "/Images/" + file.FileName;
                        using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await file.CopyToAsync(fs);
                        }
                        Photo photo = new Photo { Filename = file.FileName, PhotoUrl = path, ProductId = (int)model.Id };
                        context.Photos.Add(photo);
                    }
                    context.Update(PrforUpdate);
                    await context.SaveChangesAsync();
                    TempData["Status"] = "Product Updated Successfullly!!!";
                    return Redirect("/Home/Details/" + model.Id);
                }
                else
                {
                    TempData["Status"] = "Product Updated Successfullly With Old Photos!!!";
                    context.Update(PrforUpdate);
                    await context.SaveChangesAsync();
                    return Redirect("/Home/Details/" + model.Id);
                }
            }
            ViewData["Status"] = "Model State is Invalid!!!";
            Console.WriteLine("Model State is Invalid!!!");
            ViewData["ListCategories"] = context.Categories.ToList();
            return View(model);
        }

        /// <summary>
        /// Add Product Count And Price to Model Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddProduct(int id)
        {
            var product = context.Products.Include(p => p.ProductCategory)
            .Where(i => i.Id == id).First();
            return View(product);
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            Product PrforDelete = context.Products.Where(p => p.Id == id).FirstOrDefault();
            if (PrforDelete != null && PrforDelete.Id == id && PrforDelete.Count < 1)
            {
                try
                {
                    context.Remove(PrforDelete);
                    context.SaveChanges();
                    TempData["Status"] = "Product Id = " + id + " = [Deleted].";
                    return Redirect("/Home/Index");
                }
                catch (DbUpdateException ex)
                {
                    TempData["Status"] = ex.Message;
                    return Redirect("/Home/Index");
                }
            }
            else
            {
                if (PrforDelete.Count > 0)
                {
                    TempData["Status"] = "Product is available.";
                }
                else
                {
                    TempData["Status"] = "Error delete product.";
                }
                return Redirect("/Home/Index");
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(OrderDetailsView model)
        {
            Order updateOrderStatus = context.Orders.Where(o => o.Id == model.CurrentOrder.Id).First();
            if (model.CurrentOrder.Status != null && model.CurrentOrder.Status.Length > 2)
            {
                if (updateOrderStatus.Status != model.CurrentOrder.Status)
                {
                    if (updateOrderStatus.Status == "complited")
                    {
                        TempData["Status"] = "Статус замовлення = complited.";
                        return Redirect("/Manager/OrderDetails/" + model.CurrentOrder.Id);
                    }
                    else if (updateOrderStatus.Status == "canceled")
                    {
                        TempData["Status"] = "Статус замовлення = canceled.";
                        return Redirect("/Manager/OrderDetails/" + model.CurrentOrder.Id);
                    }
                    if (updateOrderStatus.Status == "in progress" && model.CurrentOrder.Status == "active")
                    {
                        TempData["Status"] = "Статус замовлення = in progress.";
                        return Redirect("/Manager/OrderDetails/" + model.CurrentOrder.Id);
                    }
                    if (model.CurrentOrder.Status == "canceled")
                    {
                        List<BasketElement> BasketProductToBack = context.BasketElements.Include(p => p.ProductInBasket)
                            .Where(e => e.OrderId == model.CurrentOrder.Id).ToList();
                        for (int i = 0; i < BasketProductToBack.Count; i++)
                        {
                            BasketProductToBack[i].ProductInBasket.Count += BasketProductToBack[i].Count;
                            context.Update(BasketProductToBack[i].ProductInBasket);
                        }
                        context.SaveChanges();
                    }
                    updateOrderStatus.Status = model.CurrentOrder.Status;
                    TempData["Status"] = "Статус замовлення змінено на " + updateOrderStatus.Status;
                    updateOrderStatus.OrderDetails += "Статус замовлення змінено на (" + updateOrderStatus.Status + ") , додано [" + User.Identity.Name + "]: "
                   + DateTime.Now + ". \n";
                    context.Update(updateOrderStatus);
                    context.SaveChanges();
                }
                else
                {
                    TempData["Status"] = "Обрано вже існуючий статус замовлення.";
                }

            }
            else
            {
                TempData["Status"] = "Помилка зміни статусу замовлення.";
                Console.WriteLine("!!!ERROR CHANGE STATUS!!!");
            }

            return Redirect("/Manager/OrderDetails/" + model.CurrentOrder.Id);
        }

        [HttpPost]
        public IActionResult AddOrderDetails(OrderDetailsView model)
        {
            if (model.CurrentOrder.OrderDetails != null)
            {
                if (model.CurrentOrder.OrderDetails.Length > 3)
                {
                    Order AddToOrderDetails = context.Orders.Where(o => o.Id == model.CurrentOrder.Id).First();
                    AddToOrderDetails.OrderDetails += "Коментар: " + model.CurrentOrder.OrderDetails + ", додано [" + User.Identity.Name + "]: "
                       + DateTime.Now + ". \n";
                    context.Update(AddToOrderDetails);
                    context.SaveChanges();
                }
                else
                {
                    TempData["Status"] = "Введіть коментар корректно, більше 3х символів.";
                }
            }
            else
            {
                TempData["Status"] = "Введіть коментар корректно.";
                Console.WriteLine("!!!ERROR!!! ADD COMENT MORE THAN 3 Length!!!");
            }
            return Redirect("/Manager/OrderDetails/" + model.CurrentOrder.Id);
        }
        /// <summary>
        /// Пошук замовлень по номеру замовлення
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult OrderDetails(int? id, int? notused)
        {
            Console.WriteLine("!!!!!!!!!!!!!Working Method OrderDetails HTTPGET!!!!!!!!!!!!");
            OrderDetailsView orderDetails = new OrderDetailsView();
            if (id != null)
            {
                try
                {
                    orderDetails.CurrentOrder = context.Orders.Include(u => u.User)
                        .Where(o => o.Id == id).First();
                    orderDetails.OrderProducts = context.BasketElements.Include(p => p.ProductInBasket)
                        .Where(o => o.OrderId == id).ToList();
                    return View(orderDetails);
                }
                catch (Exception ex)
                {
                    TempData["Status"] = ex.Message.ToString();
                    return RedirectToAction(nameof(ManagerPanel));
                }
            }
            else
            {
                TempData["Status"] = "Введіть айді товару.";
                return RedirectToAction(nameof(ManagerPanel));
            }
        }

        [HttpPost]
        public IActionResult OrderDetails(int id)
        {
            OrderDetailsView orderDetails = new OrderDetailsView();
            orderDetails.CurrentOrder = context.Orders.Include(u => u.User)
                .Where(o => o.Id == id).First();
            orderDetails.OrderProducts = context.BasketElements.Include(p => p.ProductInBasket)
                .Where(o => o.OrderId == id).ToList();
            return View(orderDetails);
        }
        [HttpGet]
        public IActionResult ManagerPanel() // List Orders free and takens
        {
            List<Order> Orders = context.Orders.Include(u => u.User).Where(o => o.ManagerId == null || o.ManagerId == userManager.GetUserId(User)).ToList();
            return View(Orders);
        }
        // Status : new / active / in progress / complited / canceled .
        [HttpPost]
        public IActionResult TakeOrder(int id)
        {
            Order OrderForTake = context.Orders.Where(o => o.Id == id).First();
            if (OrderForTake.ManagerId == null)
            {
                OrderForTake.ManagerId = userManager.GetUserId(User);
                OrderForTake.Status = "active";
                OrderForTake.OrderDetails += "Замовлення прийнято в роботу, статус замовлення (" + OrderForTake.Status + "), ваш менеджер: " +
                User.Identity.Name + ", прийнято: " + DateTime.Now + "." + "\n";
                context.Update(OrderForTake);
                context.SaveChanges();
            }
            else
            {
                // need add error message to page
                Console.WriteLine("!!!ERROR!!! ORDER : ManagerID IS NOT NULL!!!");
                return RedirectToAction(nameof(ManagerPanel));
            }
            return RedirectToAction(nameof(ManagerPanel));
        }
        //CreateCategory
        [HttpGet]
        public IActionResult CreateCategory()
        {
            TempData["ListCategories"] = context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model State is Invelid!!!");
                return View(model);
            }
            context.Categories.Add(model);
            context.SaveChanges();
            TempData["Status"] = "Категорія: " + model.CategoryName + ", створена.";
            return RedirectToAction(nameof(CreateCategory));
        }

        //CreateProduct
        [HttpGet]
        public IActionResult CreateProduct()
        {
            var Categories = context.Categories.ToList();
            if (Categories.Count < 1)
            {
                TempData["Status"] = "Список категорій пустий.";
                return RedirectToAction(nameof(CreateCategory));
            }
            ViewBag.ListCategories = Categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([Bind("Name,ShortDescription,LongDescription,CategoryId")] Product model, IFormFileCollection Photos)
        {
            if (ModelState.IsValid)
            {
                model.Price = 0; // change-delete this code
                model.Rating = 0; // change-delete this code
                model.Count = 0;
                model.ProductCategory = context.Categories.Where(c => c.Id == model.CategoryId).First();
                context.Add(model);
                await context.SaveChangesAsync();
                if (Photos != null && Photos.Count > 0)
                {
                    model.Image = Photos[0].FileName;
                    context.Update(model);
                    await context.SaveChangesAsync();
                    foreach (IFormFile file in Photos)
                    {
                        string path = "/Images/" + file.FileName;
                        using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await file.CopyToAsync(fs);
                        }
                        Photo photo = new Photo { Filename = file.FileName, PhotoUrl = path, ProductId = (int)model.Id };
                        context.Photos.Add(photo);
                    }
                    await context.SaveChangesAsync();
                }
                else
                {
                    ViewData["Status"] = "Please Add Photo!";
                    Console.WriteLine("Please Add Photo!!!");
                    ViewData["ListCategories"] = context.Categories.ToList();
                    context.Remove(model);
                    await context.SaveChangesAsync();
                    return View(model);
                }
                TempData["Status"] = "Product Creadted Successfullly!!!";
                return Redirect("/Home/Details/" + model.Id);
            }
            ViewData["Status"] = "Model State is Invalid!!!";
            Console.WriteLine("Model State is Invalid!!!");
            ViewData["ListCategories"] = context.Categories.ToList();
            return View(model);
        }

        //Delete Category
        [HttpGet]
        public IActionResult DeleteCategory(int Id)
        {
            var findcategory = context.Categories.Where(c => c.Id == Id).First();
            if (findcategory != null)
            {
                Product isProductWithCategory = context.Products.Where(c => c.CategoryId == findcategory.Id).FirstOrDefault();
                if (isProductWithCategory == null)
                {
                    context.Categories.Remove(findcategory);
                    context.SaveChanges();
                    TempData["Status"] = "Категорія: " + findcategory.CategoryName + ", видалена.";
                }
                else
                {
                    Console.WriteLine("!!!ERROR!!! HAVE PRODUCTS WITH THIS CATEGOGY SO CAN'T DELETE!!!");
                }
            }
            return RedirectToAction(nameof(CreateCategory));
        }
    }
}
