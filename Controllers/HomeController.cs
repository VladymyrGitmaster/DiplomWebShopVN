using DiplomWebShopVN.Models;
using DiplomWebShopVN.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace DiplomWebShopVN.Controllers
{
    //[AllowAnonymous]
    public class HomeController : Controller
    {
        private static bool infoRegister = false;
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext context;
        private List<Product> Lp = null!;
        private List<Category> Lc = null!;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        /*private readonly IHttpContextAccessor accessor;*/

        public HomeController(RoleManager<IdentityRole> roleManager, ILogger<HomeController> logger, DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            _logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        public IActionResult CountPlusNotRegistered(int id)// <<<here come ProductID 
        {
            if (id > 0 && id < 9999999) // change this if
            {
                BasketElement currentBasketEl = new BasketElement();
                List<BasketElement> BasketInSession = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");
                foreach (var elem in BasketInSession)
                {
                    if (elem.ProductId == id)
                    {
                        currentBasketEl = elem;
                    }
                }
                Product tryCountProduct = context.Products.Where(p => p.Id == currentBasketEl.ProductId).First();

                if (currentBasketEl != null)
                {
                    if ((currentBasketEl.Count + 1) <= tryCountProduct.Count)
                    {
                        currentBasketEl.Count++;
                        foreach (var item in BasketInSession)
                        {
                            if (item.ProductId == currentBasketEl.ProductId)
                            {
                                item.Count = currentBasketEl.Count;
                            }
                        }
                        SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
                        TempData["Status"] = "Кількість одиниць товару у кошику збільшена.";
                    }
                    else
                    {
                        Console.WriteLine("!!!ERROR!!! Can't Add More Count");
                        if (tryCountProduct.Count > 0)
                        {
                            foreach (var item in BasketInSession)
                            {
                                if (item.ProductId == currentBasketEl.ProductId)
                                {
                                    item.Count = tryCountProduct.Count;
                                }
                            }
                            SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
                            TempData["Status"] = "Кількість одиниць товару оновлена.";
                        }
                        else //if count = 0;
                        {
                            int indexfordelete = -1;
                            int i = 0;
                            foreach (var item in BasketInSession)
                            {
                                if (item.ProductId == currentBasketEl.ProductId)
                                {
                                    indexfordelete = i;
                                    break;
                                }
                                i++;
                            }
                            BasketInSession.RemoveAt(indexfordelete);
                            Console.WriteLine("!!!ERROR!!! Don't Have Current Count Product");
                            SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
                            TempData["Status"] = "Вибачте товар закінчився.";
                        }

                    }
                }
            }
            return Redirect("/Home/BasketNotRegistered");
        }

        public IActionResult CountMinusNotRegistered(int id) // <<<here come ProductID
        {
            if (id > 0 && id < 99999999) // change this if
            {
                BasketElement currentBasketEl = new BasketElement();
                List<BasketElement> BasketInSession = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");
                foreach (var elem in BasketInSession)
                {
                    if (elem.ProductId == id)
                    {
                        currentBasketEl = elem;
                    }
                }
                Product tryCountProduct = context.Products.Where(p => p.Id == currentBasketEl.ProductId).First();
                if (currentBasketEl != null)
                {
                    if (currentBasketEl.Count > 1 && tryCountProduct.Count > 0)
                    {
                        if ((currentBasketEl.Count - 1) <= tryCountProduct.Count)
                        {
                            currentBasketEl.Count--;
                            foreach (var item in BasketInSession)
                            {
                                if (item.ProductId == currentBasketEl.ProductId)
                                {
                                    item.Count = currentBasketEl.Count;
                                }
                            }
                            SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
                            TempData["Status"] = "Кількість товару зменшена.";
                        }
                        else if ((currentBasketEl.Count - 1) > tryCountProduct.Count)
                        {
                            currentBasketEl.Count = tryCountProduct.Count;
                            foreach (var item in BasketInSession)
                            {
                                if (item.ProductId == currentBasketEl.ProductId)
                                {
                                    item.Count = tryCountProduct.Count;
                                }
                            }
                            SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
                            TempData["Status"] = "Кількість товару оновлена.";
                        }

                    }
                    else if (tryCountProduct.Count < 1)
                    {
                        int indexfordelete = -1;
                        int i = 0;
                        foreach (var item in BasketInSession)
                        {
                            if (item.ProductId == currentBasketEl.ProductId)
                            {
                                indexfordelete = i;
                                break;
                            }
                            i++;
                        }
                        BasketInSession.RemoveAt(indexfordelete);
                        Console.WriteLine("!!!ERROR!!! Don't Have Current Count Product");
                        SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
                        TempData["Status"] = "Вибачте товар закінчився.";
                    }
                    else if (currentBasketEl.Count == 1)
                    {
                        TempData["Status"] = "Мінімальна кількість товару один.";
                        return Redirect("/Home/BasketNotRegistered");
                    }
                }
            }
            return Redirect("/Home/BasketNotRegistered");
        }

        public IActionResult RemoveBasketElNotRegistered(int id)// <<<here come ProductID
        {
            List<BasketElement> BasketInSession = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");
            int indexfordelete = -1;
            int i = 0;
            foreach (var item in BasketInSession)
            {
                if (item.ProductId == id)
                {
                    indexfordelete = i;
                    break;
                }
                i++;
            }
            BasketInSession.RemoveAt(indexfordelete);
            SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
            TempData["Status"] = "Товар видалено з кошика.";
            return Redirect("/Home/BasketNotRegistered");
        }

        [HttpGet] //Need make (method or code) for can't add in basket more than 50~ prducts.
        public IActionResult AddToBasketNotregistered(int id, string? rUrl)
        {
            List<BasketElement> BasketInSession = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");
            if (BasketInSession == null || (BasketInSession != null && BasketInSession.Count < 1))
            {
                List<BasketElement> BasketElements = new List<BasketElement>();
                BasketElement basketelement = new BasketElement();
                basketelement.ProductId = id;
                basketelement.ProductInBasket = context.Products.Where(p => p.Id == id).First();
                basketelement.Count = 1;
                basketelement.CurrentPrice = basketelement.ProductInBasket.Price * basketelement.Count;
                basketelement.OrderId = 0;
                basketelement.UserId = Url.ActionContext.HttpContext.Session.Id;
                BasketElements.Add(basketelement);
                SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketElements);
                TempData["Status"] = "Товар " + basketelement.ProductInBasket.Name + " додано в кошик.";
                //[count of Basket add to Session if NOTregistred
                Url.ActionContext.HttpContext.Session.SetInt32("basketcountNotRegistered", 1);
                //count of Basket add to Session if NOTregistred]
                return Redirect(rUrl);
            }
            else if (BasketInSession != null && BasketInSession.Count > 0)
            {
                BasketElement basketelement = new BasketElement();
                basketelement.ProductInBasket = context.Products.Where(p => p.Id == id).First();
                foreach (var item in BasketInSession)
                {
                    if (item.ProductInBasket.Id == id)
                    {
                        TempData["Status"] = "Товар " + basketelement.ProductInBasket.Name + " вже в кошику.";
                        return Redirect(rUrl);
                    }
                }
                basketelement.ProductId = id;
                basketelement.Count = 1;
                basketelement.CurrentPrice = basketelement.ProductInBasket.Price * basketelement.Count;
                basketelement.OrderId = 0;
                basketelement.UserId = Url.ActionContext.HttpContext.Session.Id;
                BasketInSession.Add(basketelement);
                SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketInSession);
                TempData["Status"] = "Товар " + basketelement.ProductInBasket.Name + " додано в кошик.";
                List<BasketElement> elementscount = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");
                //[count of Basket add to Session if NOTregistred
                Url.ActionContext.HttpContext.Session.SetInt32("basketcountNotRegistered", elementscount.Count);
                //count of Basket add to Session if NOTregistred]
                return Redirect(rUrl);
            }
            return Redirect(rUrl);
            // Url.ActionContext.HttpContext.Session.Clear();
        }

        [HttpGet]
        public IActionResult BasketNotRegistered()
        {
            List<BasketElement> BasketInSession = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");

            if (BasketInSession != null && BasketInSession.Count > 0)
            {
                //[count of Basket add to Session if NOTregistred
                Url.ActionContext.HttpContext.Session.SetInt32("basketcountNotRegistered", (int)BasketInSession.Count);
                //count of Basket add to Session if NOTregistred]
                return (View(BasketInSession));
            }
            else
            {
                //[count of Basket add to Session if NOTregistred
                Url.ActionContext.HttpContext.Session.SetInt32("basketcountNotRegistered", 0);
                //count of Basket add to Session if NOTregistred]
                TempData["Status"] = "Кошик порожній.";
                return Redirect("/Home/Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MakeOrderNotRegistered(Order model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.User.Name = model.Name;
            model.User.Prizvyshe = model.Prizvyshe;
            model.User.PhoneNumber = model.Phone;
            model.User.Email = model.Email;
            model.ManagerId = null;
            model.Status = "new";
            model.OrderDateTime = DateTime.Now;
            model.OrderDetails = "Замовлення створене, статус замовлення (" + model.Status + "), очікує обробки." + "\n";
            context.Orders.Add(model);
            context.SaveChanges();
            ApplicationUser newNotRegistered = context.Users.Where(u => u.Id == model.User.Id).First();
            //role managment
            var rolename = "guest";
            if (!await roleManager.RoleExistsAsync(rolename))
            {
                await roleManager.CreateAsync(new IdentityRole(rolename));
            }
            await userManager.CreateAsync(newNotRegistered);
            if (await roleManager.RoleExistsAsync(rolename))
            {
                await userManager.AddToRoleAsync(newNotRegistered, rolename);
            }
            List<BasketElement> BasketElemsToOrder = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");
            int i = 0;
            foreach (var item in BasketElemsToOrder)  // try if count is good and have products.
            {
                Product itemFromDb = context.Products.Where(p => p.Id == item.ProductId).First();
                if (item.Count > itemFromDb.Count)
                {
                    Console.WriteLine("!!!Error!!! Products count not Avalible! Product id:" + item.ProductId);
                    if (itemFromDb.Count > 0)
                    {
                        item.Count = itemFromDb.Count;
                        SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketElemsToOrder);
                        TempData["Status"] = "Кількість товару оновлена по наявності.";
                        return Redirect("/Home/BasketNotRegistered");
                    }
                    else
                    {
                        BasketElemsToOrder.RemoveAt(i);
                        SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", BasketElemsToOrder);
                        context.Remove(model);
                        context.SaveChanges();
                        TempData["Status"] = "Товару вже немаэ в наявності.";
                        return Redirect("/Home/BasketNotRegistered");
                    }
                }
                i++;
            }

            SessionHelper.SetObjectInSession(Url.ActionContext.HttpContext.Session, "Basket", null);
            Url.ActionContext.HttpContext.Session.SetInt32("basketcountNotRegistered", 0);
            foreach (var item in BasketElemsToOrder) // if all good add to order BasketElements
            {
                BasketElement newBasketElem = new BasketElement();
                newBasketElem.ProductId = item.ProductId;
                newBasketElem.Count = item.Count;
                newBasketElem.UserId = item.UserId;
                context.BasketElements.Add(newBasketElem);
                context.SaveChanges();
                BasketElement curentItem = context.BasketElements.Where(e => e.Id == newBasketElem.Id).First();
                Product updateProductCount = context.Products.Where(p => p.Id == item.ProductId).First();
                updateProductCount.Count -= item.Count;
                curentItem.OrderId = model.Id;
                curentItem.CurrentPrice = updateProductCount.Price;
                context.Update(curentItem);
                context.Update(updateProductCount);
                context.SaveChanges();
            }
            /*   if (!await roleManager.RoleExistsAsync("guest"))
               {
                   await roleManager.CreateAsync(new IdentityRole("guest"));
               }
               if (await roleManager.RoleExistsAsync("guest"))
               {
                   await userManager.AddToRoleAsync(await userManager.GetUserAsync(User), "guest");
               }*/

            TempData["Status"] = "Замовлення # " + model.Id + ", створене. Очікуйте дзвінка менеджера.";
            return Redirect("/Home/Index");
        }

        [HttpPost]
        public IActionResult OrderPageNotRegistered(string? SessionId)
        {
            Order newOrder = new Order();
            newOrder.UserId = SessionId;
            newOrder.isNotRegitered = true;
            List<BasketElement> allProductsInBasket = SessionHelper.GetCustomObjectFromSession<List<BasketElement>>(Url.ActionContext.HttpContext.Session, "Basket");
            foreach (var elem in allProductsInBasket)
            {
                newOrder.CountProducts += 1 * elem.Count;
                Product elemPrice = context.Products.Where(p => p.Id == elem.ProductId).First();
                if (elemPrice != null)
                {
                    newOrder.OrderPrice += elemPrice.Price * elem.Count;
                }
                else
                {
                    TempData["Status"] = "ERROR: Can't Find Product in Basket Element";
                    Console.WriteLine("ERROR: Can't Find Product in Basket Element");
                    return Redirect("/Home/BasketNotRegistered");
                }
            }
            //--------------------Discont---------------------
            newOrder.Discont = 0;
            //=================
            return View(newOrder);
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult AddComent(string incoment, int prodid)
        {
            if (incoment != null && prodid > 0)
            {
                string CurrentUserID = userManager.GetUserId(User);
                ProductComent newComent = new ProductComent();
                newComent.DateComent = DateTime.Now;
                newComent.ProductId = prodid;
                newComent.ProductUserDetail = incoment;
                newComent.UserId = CurrentUserID;
                var listProdComents = context.ProductComents.Where(p => p.ProductId == prodid).ToList();

                if (listProdComents.Count < 1)
                {
                    context.Add(newComent);
                    context.SaveChanges();
                    TempData["Status"] = "Дякуэмо за відгук.";
                    return Redirect("/Home/Details/" + prodid);
                }
                else
                {
                    foreach (var item in listProdComents)
                    {
                        if (item.UserId == newComent.UserId)
                        {
                            item.ProductUserDetail = incoment;
                            context.Update(item);
                            context.SaveChanges();
                            TempData["Status"] = "Дякуэмо, відгук оновлено.";
                            return Redirect("/Home/Details/" + prodid);
                        }
                    }
                    TempData["Status"] = "Дякуэмо за відгук.";
                    context.Add(newComent);
                    context.SaveChanges();
                    return Redirect("/Home/Details/" + prodid);
                }
            }
            TempData["Status"] = "Поле вводу порожнє, будь ласка введіть свій коментар.";
            return Redirect("/Home/Details/" + prodid);
        }

        [Authorize(Roles = "user")]
        [Route("Home/{id}/ChangeRating/{stars}")]
        [HttpGet]
        public IActionResult ChangeRating(int id, int stars)
        {
            Product findProduct = context.Products.FirstOrDefault(p => p.Id == id);
            if (findProduct != null && stars >= 1 && stars <= 5)
            {
                string currentUserId = userManager.GetUserId(User);
                int votesCount = 0; int sumaryStars = 0;
                List<RatingProduct> RatingTable = context.RatingProducts.ToList();
                if (RatingTable.Count == 0)
                {
                    RatingProduct addVotetoRating = new RatingProduct();
                    addVotetoRating.ProductId = id;
                    addVotetoRating.UserId = currentUserId;
                    addVotetoRating.Rating = stars;
                    context.Add(addVotetoRating); // add changes to product and save
                    findProduct.Rating = stars;
                    findProduct.Votes = 1;
                    context.Update(findProduct);
                    context.SaveChanges();
                    return Redirect("/Home/Details/" + id);
                }
                else
                {
                    foreach (var item in RatingTable)
                    {
                        if (item.ProductId == id && item.UserId == currentUserId)
                        {
                            if (item.Rating == stars)
                            {
                                foreach (var rProd in RatingTable)
                                {
                                    if (rProd.ProductId == id)
                                    {
                                        sumaryStars += rProd.Rating;
                                        votesCount++;
                                    }
                                }
                                sumaryStars -= stars;
                                votesCount--;
                                if (votesCount > 1)
                                {
                                    findProduct.Rating = sumaryStars / votesCount;
                                    findProduct.Votes = votesCount;
                                }
                                else
                                {
                                    findProduct.Rating = sumaryStars;
                                    findProduct.Votes = votesCount;
                                }
                                context.Update(findProduct);
                                item.ProductId = 0;
                                context.Update(item);
                                context.Remove(item);
                                context.SaveChanges();
                                return Redirect("/Home/Details/" + id);
                            }
                            else
                            {
                                item.Rating = stars;
                                context.Update(item);
                                context.SaveChanges();
                                RatingTable = context.RatingProducts.ToList();
                                foreach (var rProd in RatingTable)
                                {
                                    if (rProd.ProductId == id)
                                    {
                                        sumaryStars += rProd.Rating;
                                        votesCount++;
                                    }
                                }
                                findProduct.Rating = sumaryStars / votesCount;
                                findProduct.Votes = votesCount;
                                context.Update(findProduct);
                                context.SaveChanges();
                                return Redirect("/Home/Details/" + id);
                            }

                        }
                    }
                    RatingProduct addVotetoRating = new RatingProduct();
                    addVotetoRating.ProductId = id;
                    addVotetoRating.UserId = currentUserId;
                    addVotetoRating.Rating = stars;
                    context.Add(addVotetoRating); // add changes to product and save
                    context.SaveChanges();
                    RatingTable = context.RatingProducts.ToList();
                    foreach (var rProd in RatingTable)
                    {
                        if (rProd.ProductId == id)
                        {
                            sumaryStars += rProd.Rating;
                            votesCount++;
                        }
                    }
                    findProduct.Rating = sumaryStars / votesCount;
                    findProduct.Votes = votesCount;
                    context.Update(findProduct);
                    context.SaveChanges();
                    return Redirect("/Home/Details/" + id);
                }
            }
            else
            {
                Console.WriteLine("!!!ERROR Cant Add STARS TO RATING PRODUCT!!!");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string seach, string category, int? page)
        {
            //[count of Basket add to Session if registered
            if (User.Identity.IsAuthenticated)
            {
                string Userid = userManager.GetUserId(User);
                ApplicationUser _curentUser = await userManager.GetUserAsync(User);
                int countbasket = context.BasketElements.Where(u => u.UserId == Userid && u.OrderId == 0).ToList().Count;
                Url.ActionContext.HttpContext.Session.SetInt32("basketcount", countbasket);
                if (!string.IsNullOrEmpty(_curentUser.ProfilePicture))
                {
                    Url.ActionContext.HttpContext.Session.SetString("userimg", _curentUser.ProfilePicture);
                }
                else
                {
                    Url.ActionContext.HttpContext.Session.Remove("userimg");
                }
            }
            //count of Basket add to Session if registered]
            int pageNumber = page ?? 1;
            int pageSize = 8;
            if (!String.IsNullOrEmpty(seach)) // Пошук товарів
            {
                List<Product> Lp2;
                Lp = context.Products.Include(c => c.ProductCategory).Include(d => d.Photos).ToList();
                Lp2 = new List<Product>();
                foreach (var item in Lp)
                {
                    if (item.Name.ToLower().Contains(seach.ToLower())
                        || item.ShortDescription.ToLower().Contains(seach.ToLower())
                        || item.ProductCategory.CategoryName.ToLower().Contains(seach.ToLower())
                        || item.ProductCategory.Description.ToLower().Contains(seach.ToLower()))
                    {
                        Lp2.Add(item);
                    }
                }
                Lc = context.Categories.ToList();
                ViewData["ListCategories"] = Lc;
                return View(Lp2.ToPagedList(pageNumber, pageSize));
            }
            else if (!String.IsNullOrEmpty(category))
            {
                Lp = context.Products.Include(p => p.ProductCategory)
                    .Include(d => d.Photos)
                    .Where(c => c.ProductCategory.CategoryName.ToLower() == category.ToLower()).ToList();
                Lc = context.Categories.ToList();
                ViewData["ListCategories"] = Lc;
                return View(Lp.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var products = context.Products.Include(p => p.ProductCategory).Include(d => d.Photos);
                var OnePageProducts = products.ToPagedList(pageNumber, pageSize);
                Lc = context.Categories.ToList();
                ViewData["ListCategories"] = Lc;
                return View(OnePageProducts);
            }

        }
        public IActionResult Privacy()
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (!infoRegister)
                {
                    TempData["Status"] = "Зареєструйтесь і отримайте миттєву знижку в подарунок.";
                    infoRegister = true;
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            List<ProductComent> listComents = context.ProductComents.Include(u => u.User).Where(c => c.ProductId == id).ToList();
            ViewData["coments"] = listComents;
            Product product = context.Products.Include(p => p.ProductCategory)
            .Include(d => d.Photos)
            .Where(i => i.Id == id).First();
            return View(product);
        }

    }
}