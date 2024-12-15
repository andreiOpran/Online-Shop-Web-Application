using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Security.Claims;
using System.Xml.Schema;
using static OnlineShop.Models.CartProducts;

namespace OnlineShop.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // se afiseaza lista de produse impreuna cu categoria
        // [HttpGet] implicit
        // TODO
        // [Authorize(Roles = "")]
        public IActionResult Index()
        {

            var products = db.Products.Include("Category")
                                      .Include("User");

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            ViewBag.Products = products;


            // motor cautare

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // scoatem spatiile

                // cautare in produs (title, description)
                List<int> productIds = db.Products.Where(
                                       p => p.Title.Contains(search) || p.Description.Contains(search)
                                       ).Select(p => p.ProductId).ToList();

                // probabil este irelevant si aduce prea multe rezultate (omitem deocamdata)
                // cautare in review-uri (content)
                //List<int> reviewIds = db.Reviews.Where(
                //      r => r.Content != null && r.Content.Contains(search)
                //      ).Select(r => (int)r.ProductId).ToList();

                // TODO - nu aduce toate rezultatele cand cauti numele categoriei
                // cautare in categorii (CategoryName)
                List<int> categoryIds = db.Categories.Where(
                      c => c.CategoryName.Contains(search)
                      ).SelectMany(c => c.Products.Select(p => p.ProductId)).ToList();

                // concatenare
                List<int> searchIds = productIds.Union(categoryIds).ToList();

                // filtrare
                products = db.Products.Where(product => searchIds.Contains(product.ProductId))
                                      .Include("Category")
                                      .Include("User");
            }

            ViewBag.SearchString = search;


            // afisare paginata

            int perPage = 12;
            int totalItems = products.Count();

            // /Products/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // pagina 1 -> offset = 0, pagina 2 -> offset = 2, pagina 3 -> offset = 4 ...
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * perPage;
            }

            // preluam produsele pentru pagina curenta
            var paginatedProducts = products.Skip(offset).Take(perPage);

            // numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)perPage);

            ViewBag.CurrentPage = currentPage == 0 ? 1 : currentPage;

            // trimitem produsele catre view
            ViewBag.Products = paginatedProducts;

            // search-ul ramane in url chiar daca schimbam pagina
            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Products/Index?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?page";
            }

            return View();

        }


        // afisare un singur produs in functie de id imprepuna cu categoria
        // se preiau si review-urile produsului
        //[Authorize(Roles = "Admin")]
        public IActionResult Show(int id)
        {
            Product product = db.Products.Include("Category")
                                         .Include("User")
                                         .Include("Reviews")
                                         .Include("Reviews.User")
                                         .Where(p => p.ProductId == id)
                                         .First();

            if (product == null)
            {
                return NotFound();
            }


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // TODO - implementarea functiei SetAcessRights() 
            // SetAccessRights();

            return View(product);
        }


        // adaugare review in DB
        [HttpPost]
        // TODO
        // [Authorize(Roles = "")]
        public IActionResult Show([FromForm] Review review)
        {
            review.CreatedDate = DateTime.Now;

            review.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductId);
            }
            else
            {
                Product product = db.Products.Include("Category")
                                             .Include("Reviews")
                                             .Include("Reviews.User")
                                             .Where(p => p.ProductId == review.ProductId)
                                             .First();

                // TODO - implementarea functiei SetAcessRights()
                // SetAccessRights();

                return View(product);
            }
        }


        // formular pentru adaugare produs + selectare categorie
        // [HttpGet] implicit
        // TODO
        // [Authorize(Roles = "")]
        public IActionResult New()
        {
            Product product = new Product();

            product.Categories = GetAllCategories();

            return View(product);

        }

        // salvare produs in baza de date
        // TODO
        // [Authorize(Roles = "")]
        [HttpPost]
        public async Task<IActionResult> New(Product product, IFormFile Image)
        {
            var sanitizer = new HtmlSanitizer();
            product.CreatedDate = DateTime.Now;
            product.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                product.Description = sanitizer.Sanitize(product.Description);

                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    product.ImagePath = "/images/" + fileName;
                }

                db.Products.Add(product);
                await db.SaveChangesAsync();
                TempData["message"] = "The product has been added successfully.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                product.Categories = GetAllCategories();
                return View(product);
            }
        }


        // edit articol impreuna cu categoria sa (categoria se selecteaza din dropdown)
        // datele existente ale produsului se incarca in fromular
        // [HttpGet] implicit
        // TODO
        // [Authorize(Roles = "")]
        public IActionResult Edit(int id)
        {
            Product product = db.Products.Include("Category")
                                         .Where(p => p.ProductId == id)
                                         .First();

            product.Categories = GetAllCategories();

            // TODO
            //if( /*User-ul are drepturi de editare*/)
            //{
            return View(product);
            //}
            //else
            //{
            //    TempData["message"] = "You do not have the rights to edit this product.";
            //    TempData["messageType"] = "alert-danger";
            //    return RedirectToAction("Index");
            //}                       
        }

        // salvare produs modificat in baza de date
        [HttpPost]
        // TODO
        // [Authorize(Roles = "")]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile? Image)
        {
            var sanitizer = new HtmlSanitizer();

            Product product = db.Products.Find(id);

            if (ModelState.IsValid)
            {
                //if( /*User-ul are drepturi de editare*/ )
                //{
                product.Title = requestProduct.Title;

                requestProduct.Description = sanitizer.Sanitize(requestProduct.Description);
                product.Description = requestProduct.Description;

                product.Price = requestProduct.Price;

                product.Stock = requestProduct.Stock;

                product.CategoryId = requestProduct.CategoryId;
                // product.Category = requestProduct.Category; // TODO - de verificat

                product.SalePercentage = requestProduct.SalePercentage;

                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    product.ImagePath = "/images/" + fileName;
                }               

                db.SaveChanges();

                TempData["message"] = "The product has been modified successfully.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
                //}
                //else
                //{
                //    TempData["message"] = "You do not have the rights to edit this product.";
                //    TempData["messageType"] = "alert-danger";
                //    return RedirectToAction("Index");
                //}

            }
            else
            {
                requestProduct.Categories = GetAllCategories();
                return View(requestProduct);
            }
        }

        // stergere produs        
        [HttpPost]
        // TODO
        // [Authorize(Roles = "")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Include("Reviews")
                                         .Where(p => p.ProductId == id)
                                         .First();

            // TODO
            //if( /*User-ul are drepturi de stergere*/ )
            //{
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["message"] = "The product has been deleted successfully.";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
            //}
            //else
            //{
            //    TempData["message"] = "You do not have the rights to delete this product.";
            //    TempData["messageType"] = "alert-danger";
            //    return RedirectToAction("Index");
            //}



        }

        // TODO
        // Conditiile de afisare pentru butoanele de editare si stergere
        //private void SetAccessRights()
        //{

        //}


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // populare dropdown
            var selectList = new List<SelectListItem>();

            var categories = from category in db.Categories select category;

            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.CategoryId.ToString();
                listItem.Text = category.CategoryName;

                // push_back
                selectList.Add(listItem);
            }

            return selectList;
        }


        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Preluam utilizatorul curent
            var userId = _userManager.GetUserId(User);

            // Verificam dacă utilizatorul are un cos activ
            var cart = db.Carts.FirstOrDefault(c => c.UserId == userId && c.IsActive);

            // Daca nu exista un cos activ, cream unul nou
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    IsActive = true
                };
                db.Carts.Add(cart);
                db.SaveChanges(); // Salvam pentru a obtine ID-ul cosului
            }

            // Verificam daca produsul exista
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                TempData["message"] = "The product does not exist."; // Produsul nu exista
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Verificam dacă produsul are stoc suficient
            var cartProduct = db.CartProducts.FirstOrDefault(cp => cp.CartId == cart.CartId && cp.ProductId == productId);

            // Daca produsul nu este deja in cos, presupunem ca dorim sa adaugam 1
            int newQuantity = cartProduct == null ? 1 : cartProduct.Quantity + 1;

            if (product.Stock < newQuantity)
            {
                TempData["message"] = "Not enough stock available for this product."; // Stoc insuficient
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Adaugam produsul in cos sau actualizam cantitatea
            if (cartProduct == null)
            {
                // Daca produsul nu exista in cos, il adaugam
                cartProduct = new CartProduct
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = 1 // Cantitatea initiala este 1
                };
                db.CartProducts.Add(cartProduct);
            }
            else
            {
                // Daca produsul exista deja, incrementam cantitatea
                cartProduct.Quantity++;
            }

            // Salvam modificarile în baza de date (fara să scadem din stoc aici)
            db.SaveChanges();

            TempData["message"] = "The product has been added to your cart."; // Produsul a fost adaugat in cos
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }



    }

}

