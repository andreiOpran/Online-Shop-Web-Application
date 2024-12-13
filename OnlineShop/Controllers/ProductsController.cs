using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Xml.Schema;

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

            var products = db.Products.Include("Category");

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
                                      .Include("Category");
            }

            ViewBag.SearchString = search;


            // afisare paginata

            int perPage = 5;
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
        public IActionResult New(Product product)
        {
            var sanitizer = new HtmlSanitizer();

            product.CreatedDate = DateTime.Now;
            
            // preluare user id care posteaza
            product.UserId = _userManager.GetUserId(User);

            if(ModelState.IsValid)
            {
                product.Description = sanitizer.Sanitize(product.Description);

                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "The product has been added succesfully.";
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
        public IActionResult Edit(int id, Product requestProduct)
        {
            var sanitizer = new HtmlSanitizer();

            Product product = db.Products.Find(id);

            if(ModelState.IsValid)
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

                    db.SaveChanges();

                    TempData["message"] = "The product has been modified succesfully.";
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
                TempData["message"] = "The product has been deleted succesfully.";
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

    }

}
