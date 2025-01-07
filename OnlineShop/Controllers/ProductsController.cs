using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Security.Claims;
using System.Xml.Schema;
using static OnlineShop.Models.CartProducts;

namespace OnlineShop.Controllers
{
    
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
        public IActionResult Index()
        {
            IQueryable<Product> products = db.Products.Include(p => p.Category)
                                                      .Include(p => p.User)
                                                      .Include(p => p.Reviews)
                                                      .Where(p => p.Status != "Initial");
            

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            
            // afisare data si user produs
            var isAdmin = User.IsInRole("Admin");
            var isEditor = User.IsInRole("Editor");
            ViewBag.ShowDateUserProduct = isAdmin || isEditor;



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

                // cautare in categorii (CategoryName)
                List<int> categoryIds = db.Categories.Where(
                      c => c.CategoryName.Contains(search)
                      ).SelectMany(c => c.Products.Select(p => p.ProductId)).ToList();

                // concatenare
                List<int> searchIds = productIds.Union(categoryIds).ToList();

                // filtrare
                products = db.Products.Where(product => searchIds.Contains(product.ProductId))
                                      .Include(p => p.Category)
                                      .Include(p => p.User)
                                      .Include(p => p.Reviews);
            }

            ViewBag.SearchString = search;

            // sortare
            var sortOrder = HttpContext.Request.Query["sortOrder"].ToString();
            ViewBag.CurrentSort = sortOrder;

            switch (sortOrder)
            {
                case "PriceAscending":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "PriceDescending":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "RatingAscending":
                    products = products.OrderBy(p => p.Rating);
                    break;
                case "RatingDescending":
                    products = products.OrderByDescending(p => p.Rating);
                    break;
                case "ProductIdAscending":
                    products = products.OrderBy(p => p.ProductId);
                    break;
            }

            // daca nu este admin, atunci utilizatorul vede doar produsele Approved
            if (!User.IsInRole("Admin"))
            {
                products = products.Where(p => p.Status == "Approved");
            }

            // afisare paginata

            int perPage = 12;
            int totalItems = products.Count();

            // /Products/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // pagina 1 -> offset = 0, pagina 2 -> offset = 12, pagina 3 -> offset = 24 ...
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * perPage;
            }

            // preluam produsele pentru pagina curenta
            var paginatedProducts = products.Skip(offset).Take(perPage).ToList();

            // numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)perPage);

            ViewBag.CurrentPage = currentPage == 0 ? 1 : currentPage;

            // trimitem produsele catre view
            ViewBag.Products = paginatedProducts;

            // search-ul ramane in url chiar daca schimbam pagina
            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Products/Index?search=" + search + "&sortOrder=" + sortOrder + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Products/Index?sortOrder=" + sortOrder + "&page";
            }

            return View();
        }


        // afisare un singur produs in functie de id imprepuna cu categoria
        // se preiau si review-urile produsului
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

            
            var isAdmin = User.IsInRole("Admin");
            var isEditor = User.IsInRole("Editor");

            // produsul pending sau denied nu poate fi vizualizat de user
            if ((product.Status == "Pending" || product.Status == "Denied") && !isAdmin)
            {
                TempData["message"] = "You do not have permission to view this product.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            ViewBag.ShowProductUserDetails = isAdmin || isEditor;
            SetAccessRightsReview();


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            if (product.Reviews.Any(r => r.Rating.HasValue))
            {
                product.Rating = Math.Round((decimal)product.Reviews.Where(r => r.Rating.HasValue).Average(r => r.Rating.Value), 2);
            }
            else
            {
                product.Rating = 0;
            }
            db.SaveChanges();

            ViewBag.ReviewsCount = product.Reviews.Count();

            SetAccessRights();

            return View(product);
        }


        // adaugare review in DB
        [HttpPost]
        public IActionResult Show([FromForm] Review review)
        {

            // Verificam daca utilizatorul este logat
            if (!User.Identity.IsAuthenticated)
            {
                TempData["message"] = "You need to log in or register to add a review.";
                TempData["messageType"] = "alert-warning";
                return Redirect("/Identity/Account/Register"); 
            }

            review.CreatedDate = DateTime.Now;

            review.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();

                // update rating 
                var product = db.Products.Include("Reviews").FirstOrDefault(p => p.ProductId == review.ProductId);
                if (product != null)
                {
                    if (product.Reviews.Any(r => r.Rating.HasValue))
                    {
                        product.Rating = Math.Round((decimal)product.Reviews.Where(r => r.Rating.HasValue).Average(r => r.Rating), 2);
                    }
                    else
                    {
                        product.Rating = 0;
                    }                    
                    db.SaveChanges();
                }

                return Redirect("/Products/Show/" + review.ProductId);
            }
            else
            {
                Product product = db.Products.Include("Category")
                                             .Include("Reviews")
                                             .Include("Reviews.User")
                                             .Where(p => p.ProductId == review.ProductId)
                                             .First();

                SetAccessRights();

                return View(product);
            }
        }


        // formular pentru adaugare produs + selectare categorie
        // [HttpGet] implicit
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New()
        {
            Product product = new Product();

            product.Categories = GetAllCategories();

            return View(product);

        }

        // salvare produs in baza de date
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> New(Product product, IFormFile Image)
        {
            var sanitizer = new HtmlSanitizer();
            product.CreatedDate = DateTime.Now;
            product.UserId = _userManager.GetUserId(User);

            if(User.IsInRole("Editor"))
            {
                product.Status = "Pending";
            }
            else if(User.IsInRole("Admin"))
            {
                product.Status = "Approved";
            }

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

        [Authorize(Roles = "Admin")]
        public IActionResult PendingProducts()
        {
            var pendingProducts = db.Products.Include(p => p.Category)
                                             .Include(p => p.User)
                                             .Where(p => p.Status == "Pending")
                                             .ToList();
            return View(pendingProducts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                product.Status = "Approved";
                db.SaveChanges();
            }
            return RedirectToAction("PendingProducts");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DenyProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                product.Status = "Denied";
                db.SaveChanges();
            }
            return RedirectToAction("PendingProducts");
        }

        // edit articol impreuna cu categoria sa (categoria se selecteaza din dropdown)
        // datele existente ale produsului se incarca in fromular
        // [HttpGet] implicit
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Product product = db.Products.Include("Category")
                                         .Where(p => p.ProductId == id)
                                         .First();

            product.Categories = GetAllCategories();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if(product.UserId != userId && !isAdmin)
            {
                TempData["message"] = "You do not have the rights to edit this product.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            return View(product);
                
        }

        // salvare produs modificat in baza de date
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile? Image)
        {
            var sanitizer = new HtmlSanitizer();

            Product product = db.Products.Find(id);

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (ModelState.IsValid)
            {
                if (product.UserId == userId && !isAdmin)
                {
                    product.PendingEdit = true;
                    product.Status = "PendingEdit";

                    // memorare edit-uri in tabel separat
                    var pendingEdit = new PendingEdit
                    {
                        ProductId = product.ProductId,
                        OriginalProduct = product,
                        EditedProduct = requestProduct,
                        UserId = userId,
                        CreatedDate = DateTime.Now
                    };
                    db.PendingEdits.Add(pendingEdit);
                    await db.SaveChangesAsync();

                    TempData["message"] = "The product edit request has been submitted for approval.";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }

                product.Title = requestProduct.Title;
                product.Description = sanitizer.Sanitize(requestProduct.Description);
                product.Price = requestProduct.Price;
                product.Stock = requestProduct.Stock;
                product.CategoryId = requestProduct.CategoryId;
                product.Category = requestProduct.Category;
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
            }
            else
            {
                requestProduct.Categories = GetAllCategories();
                return View(requestProduct);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PendingEdits()
        {
            var pendingEdits = db.PendingEdits.Include(pe => pe.OriginalProduct)
                                              .ThenInclude(p => p.Category)
                                              .Include(pe => pe.EditedProduct)
                                              .ThenInclude(p => p.Category)
                                              .Include(pe => pe.User)
                                              .ToList();
            return View(pendingEdits);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveEdit(int id)
        {
            var pendingEdit = db.PendingEdits.Include(pe => pe.EditedProduct)
                                             .FirstOrDefault(pe => pe.ProductId == id);
            if (pendingEdit != null)
            {
                var product = db.Products.Find(id);
                if (product != null)
                {
                    product.Title = pendingEdit.EditedProduct.Title;
                    product.Description = pendingEdit.EditedProduct.Description;
                    product.Price = pendingEdit.EditedProduct.Price;
                    product.Stock = pendingEdit.EditedProduct.Stock;
                    product.CategoryId = pendingEdit.EditedProduct.CategoryId;
                    product.Category = pendingEdit.EditedProduct.Category;
                    product.SalePercentage = pendingEdit.EditedProduct.SalePercentage;
                    product.ImagePath = pendingEdit.EditedProduct.ImagePath;
                    product.PendingEdit = false;
                    product.Status = "Approved";
                    db.PendingEdits.Remove(pendingEdit);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("PendingEdits");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DenyEdit(int id)
        {
            var pendingEdit = db.PendingEdits.Include(pe => pe.EditedProduct)
                                             .FirstOrDefault(pe => pe.ProductId == id);
            if (pendingEdit != null)
            {
                var product = db.Products.Find(id);
                if (product != null)
                {
                    product.PendingEdit = false;
                    product.Status = "Approved";
                    db.PendingEdits.Remove(pendingEdit);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("PendingEdits");
        }

        // stergere produs        
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Include("Reviews")
                                         .Where(p => p.ProductId == id)
                                         .First();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (product.UserId == userId && !isAdmin)
            {
                product.PendingDelete = true;
                product.Status = "PendingDelete";
                db.SaveChanges();

                TempData["message"] = "The product delete request has been submitted for approval.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            if (product.UserId != userId && !isAdmin)
            {
                TempData["message"] = "You do not have the rights to delete this product.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");

            }
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["message"] = "The product has been deleted successfully.";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin")]
        public IActionResult PendingDeletions()
        {
            var pendingDeletions = db.Products.Include(p => p.Category)
                                              .Include(p => p.User)
                                              .Where(p => p.PendingDelete)
                                              .ToList();
            return View(pendingDeletions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveDelete(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }
            return RedirectToAction("PendingDeletions");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DenyDelete(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                product.PendingDelete = false;
                product.Status = "Approved";
                db.SaveChanges();
            }
            return RedirectToAction("PendingDeletions");
        }

        // Conditiile de afisare pentru butoanele de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Editor"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

        // Conditiile de afisare pentru butoanele de editare si stergere Review
        private void SetAccessRightsReview()
        {
            ViewBag.UserCurentReview = _userManager.GetUserId(User);
            ViewBag.EsteAdminReview = User.IsInRole("Admin");
        }

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
            // Verificam daca utilizatorul este logat
            if (!User.Identity.IsAuthenticated)
            {
                TempData["message"] = "You need to log in or register to add products to the cart.";
                TempData["messageType"] = "alert-warning";
                return Redirect("/Identity/Account/Register"); // Redirectionam catre pagina de inregistrare
            }

            // Preluam utilizatorul curent
            var userId = _userManager.GetUserId(User);

            // Verificam daca utilizatorul are un cos activ
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

            // Verificam daca produsul are stoc suficient
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

            // Salvam modificarile in baza de date
            db.SaveChanges();

            TempData["message"] = "The product has been added to your cart."; // Produsul a fost adaugat in cos
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }

    }

}

