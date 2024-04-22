using DiplomWebShopVN.Models;
using DiplomWebShopVN.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace DiplomWebShopVN.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderController (DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public int GetDiscontProcent(string UserId) //complited //
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

        [HttpPost] // Status : new / active / in progress / complited / canceled .
        public IActionResult MakeOrder(Order model) // when this method work need stop another methods MakeOrder<
        {
            if (!ModelState.IsValid)
            {   //back order price with no discont.
                int backOrderPrice = 0;
                List<BasketElement> BasketOrder = context.BasketElements.Where(b => b.UserId == model.UserId).Where(c => c.OrderId == 0).ToList();
                foreach(var item in BasketOrder)
                {
                    backOrderPrice += (int) item.CurrentPrice;
                }
                model.OrderPrice = backOrderPrice;
              return View(model);
            }
            model.UserId = userManager.GetUserId(User);
            model.User = userManager.Users.Where(u => u.Id == model.UserId).First();
            model.ManagerId = null;
            model.Status = "new";
            model.OrderDateTime = DateTime.Now;
            model.OrderDetails = "Замовлення створене, статус замовлення ("+ model.Status + "), очікує обробки."+"\n";
            context.Orders.Add(model);
            context.SaveChanges();
            List<BasketElement> BasketElemsToOrder = context.BasketElements
                .Where(a => a.UserId == model.UserId)
                .Where(b => b.OrderId == 0).ToList();
            foreach(var item in BasketElemsToOrder)  // try if count is good and have products.
            {
                Product itemFromDb = context.Products.Where(p => p.Id == item.ProductId).First();
                if (item.Count > itemFromDb.Count)
                {
                    Console.WriteLine("!!!Error!!! Products count not Avalible! Product id:"+item.ProductId);
                    if(itemFromDb.Count > 0)
                    {
                        item.Count = itemFromDb.Count;
                        context.Update(item);
                        context.SaveChanges();
                        return RedirectToAction(nameof(Basket));
                    }
                    else
                    {
                        context.Remove(item);
                        context.Remove(model);
                        context.SaveChanges();
                        return RedirectToAction(nameof(Basket));
                    }                   
                }
            }
            foreach (var item in BasketElemsToOrder) // if all good add to order BasketElements
            {
                Product updateProductCount = context.Products.Where(p => p.Id == item.ProductId).First();
                updateProductCount.Count -= item.Count;
                item.OrderId = model.Id;
                item.CurrentPrice = updateProductCount.Price;
                context.Update(item);
                context.Update(updateProductCount);
                context.SaveChanges();
            /*    TempData["Status"] = "Замовлення створене.";*/
            }
            TempData["Status"] = "Замовлення # " + model.Id + ", створене. Очікуйте дзвінка менеджера.";
            return Redirect("/User/ListOrders");
        }

        [HttpPost]
        public IActionResult OrderPage(string uId)
        {
            Order newOrder = new Order();
            newOrder.UserId = uId;
            newOrder.User = context.Users.Where(u => u.Id == uId).First();
            newOrder.Name = newOrder.User.Name;
            newOrder.Phone = newOrder.User.PhoneNumber;
            newOrder.Prizvyshe = newOrder.User.Prizvyshe;
            newOrder.Email = newOrder.User.Email;
            List<BasketElement> allProductsInBasket = context.BasketElements.Where(u=>u.UserId == uId).Where(e => e.OrderId == 0).ToList();
            foreach(BasketElement elem in allProductsInBasket)
            {
                newOrder.CountProducts += 1 * elem.Count;
                Product elemPrice = context.Products.Where(p => p.Id == elem.ProductId).First();
                if(elemPrice != null)
                {
                  newOrder.OrderPrice += elemPrice.Price * elem.Count;
                  elem.CurrentPrice = elemPrice.Price * elem.Count;
                    context.BasketElements.Update(elem);
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("ERROR: Can't Find Product in Basket Element");
                    return RedirectToAction(nameof(Basket));
                }
            }
            //--------------------Discont---------------------
            newOrder.Discont = GetDiscontProcent(uId);
            //=================
            return View(newOrder);
        }

        public IActionResult AddToBasket(int id, string rUrl)
        {
            Product newProductToBasket = context.Products.Where(p=>p.Id == id).First(); 
            BasketElement newBasketEl = new BasketElement();
            newBasketEl.ProductInBasket = newProductToBasket;
            newBasketEl.UserId = userManager.GetUserId(User);
            newBasketEl.Count = 1;
            newBasketEl.OrderId = 0;
            newBasketEl.ProductId = newProductToBasket.Id;
            List<BasketElement> CurentInBasketElements = context.BasketElements
                .Where(e => e.UserId == newBasketEl.UserId)
                .Where(b => b.OrderId == 0).ToList();
            foreach (var item in CurentInBasketElements)
            {
                if(item.ProductId == id)
                {
                    Console.WriteLine("!!!INFO!!! Product Alllredy Addeded to Basket.");
                    CountPlus((int) item.Id);
                    Console.WriteLine("!!!!INFO COUNT PLUS DONE METHOD!!!");
                    return Redirect(rUrl);
                    //return RedirectToAction(nameof(Basket));    
                }
            }
            Console.WriteLine("!!!!addtobasketmethodwork!!!!");
            context.BasketElements.Add(newBasketEl);
            context.SaveChanges();
            TempData["Status"] = "Товар " + newBasketEl.ProductInBasket.Name + " додано в кошик.";
            //[count of Basket add to Session if registred
            string Userid = userManager.GetUserId(User);
            int countbasket = context.BasketElements.Where(u => u.UserId == Userid && u.OrderId == 0).ToList().Count;
            Url.ActionContext.HttpContext.Session.SetInt32("basketcount", countbasket);
            //count of Basket add to Session if registred]
            return Redirect(rUrl);
        }

        public IActionResult CountPlus(int id) // (NEED FIX) change method for more safe (mayby POST)
        {
            if (id > 0 && id < 9999999) // change this if
            {
               BasketElement currentBasketEl = context.BasketElements
                .Where(e => e.Id == id).First();
                Product tryCountProduct = context.Products.Where(p => p.Id == currentBasketEl.ProductId).First();

                if (currentBasketEl != null)
                {
                    if((currentBasketEl.Count + 1) <= tryCountProduct.Count)
                    {
                         currentBasketEl.Count++;
                         context.Update(currentBasketEl);
                         context.SaveChanges();
                        TempData["Status"] = "Кількість одиниць товару у кошику збільшена.";
                    }
                    else
                    {
                        Console.WriteLine("!!!ERROR!!! Can't Add More Count");
                        if(tryCountProduct.Count > 0)
                        {
                            currentBasketEl.Count = tryCountProduct.Count;
                            context.Update(currentBasketEl);
                            context.SaveChanges();
                            TempData["Status"] = "Кількість одиниць товару оновлена.";
                        }
                        else //if count = 0;
                        {
                            Console.WriteLine("!!!ERROR!!! Don't Have Current Count Product");
                            context.Remove(currentBasketEl);
                            context.SaveChanges();
                            TempData["Status"] = "Вибачте товар закінчився.";
                        }

                    }                               
                }
            }                  
            return RedirectToAction(nameof(Basket));                        
        }

        public IActionResult CountMinus(int id) // (NEED FIX) change method for more safe (mayby POST)
        {
            if (id > 0 && id < 99999999) // change this if
            {               
                BasketElement currentBasketEl = context.BasketElements
                 .Where(e => e.Id == id).First();
                Product tryCountProduct = context.Products.Where(p => p.Id == currentBasketEl.ProductId).First();
                if (currentBasketEl != null)
                {
                    if(currentBasketEl.Count > 1 && tryCountProduct.Count > 0)
                    {
                        if((currentBasketEl.Count-1) <= tryCountProduct.Count)
                        {
                           currentBasketEl.Count--;
                           context.Update(currentBasketEl);
                           context.SaveChanges();
                            TempData["Status"] = "Кількість товару зменшена.";
                        }
                        else if((currentBasketEl.Count - 1) > tryCountProduct.Count)
                        {
                                currentBasketEl.Count = tryCountProduct.Count;
                                context.Update(currentBasketEl);
                                context.SaveChanges();
                               TempData["Status"] = "Кількість товару оновлена.";
                        }
                        
                    }                 
                    else if(tryCountProduct.Count < 1)
                    {
                        Console.WriteLine("!!!ERROR___ Don't Have Current Count Product");
                        context.Remove(currentBasketEl);
                        context.SaveChanges();
                        TempData["Status"] = "Вибачте товар закінчився.";
                    }
                    else if (currentBasketEl.Count == 1)
                    {
                        TempData["Status"] = "Мінімальна кількість товару один.";
                        return RedirectToAction(nameof(Basket));
                    }
                }
            }
            return RedirectToAction(nameof(Basket));
        }

        [HttpGet]
        public IActionResult DeleteFromBasket(int id) // think need use try-catch and try POst method
        {
            Console.WriteLine("!!!!!!ID = " + id);
            if (id > 0 && id < 999999999)
            {
                BasketElement basketElforDel = context.BasketElements.Where(e => e.Id == id).First();
                if(basketElforDel != null)
                {
                    context.BasketElements.Remove(basketElforDel);
                    context.SaveChanges();       
                }             
            }
            return RedirectToAction(nameof(Basket));

        }

        [HttpGet]
        public IActionResult Basket()
        {
            //  var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            // var url = location.AbsoluteUri;

            
            List<BasketElement> listBasket = context.BasketElements
            .Include(p => p.ProductInBasket)
            .Where(u => u.UserId == userManager.GetUserId(User) && u.OrderId == 0).ToList();
            // trying if product or him count exists
            if (listBasket != null && listBasket.Count > 0)
            {
                foreach(var item in listBasket) 
                {
                    Product tryCountProduct = context.Products.Where(p => p.Id == item.ProductId).First();
                    if (tryCountProduct != null)
                    {
                        if(item.Count > tryCountProduct.Count)
                        {
                            if(tryCountProduct.Count > 0)
                            {
                                item.Count = tryCountProduct.Count;
                                context.Update(item);
                                context.SaveChanges();
                                TempData["Status"] = "Оновлена кількість товару по наявності.";
                            }
                            else
                            {
                                context.Remove(item);
                                context.SaveChanges();
                                TempData["Status"] = "Видалено товари які не в наявності.";
                            }
                        }
                    }
                    else
                    {
                        context.Remove(item);
                        context.SaveChanges();
                        TempData["Status"] = "Видалено товари яких немає в наявності.";
                    }

                }
            }
            listBasket = context.BasketElements
            .Include(p => p.ProductInBasket)
            .Where(u => u.UserId == userManager.GetUserId(User) && u.OrderId == 0).ToList();
            //[count of Basket add to Session if registred
            string Userid = userManager.GetUserId(User);
            int countbasket = context.BasketElements.Where(u => u.UserId == Userid && u.OrderId == 0).ToList().Count;
            Url.ActionContext.HttpContext.Session.SetInt32("basketcount", countbasket);
            //count of Basket add to Session if registred]
            return View(listBasket);
        }

 /*       [HttpPost]
        public IActionResult Basket(BasketElement model)
        {
            return RedirectToAction("Basket");
        }*/
    }
}
