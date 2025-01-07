using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {

            // afisare edit, delete si add categorie
            var isAdmin = User.IsInRole("Admin");
            ViewBag.ShowEditDeleteAdd = isAdmin;


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            
            ViewBag.Categories = categories;
            return View();
        }

        // afisare paginata
        [HttpGet("/Categories/Show/{id:int}/{page:int?}")] // route constraint
        public ActionResult Show(int id, int page = 1)
        {
            int perPage = 6;
            var category = db.Categories.Include(c => c.Products)
                                        .ThenInclude(p => p.User)
                                        .Include(c => c.Products)
                                        .ThenInclude(p => p.Reviews)
                                        .FirstOrDefault(c => c.CategoryId == id);

            // afisare data si user produs
            var isAdmin = User.IsInRole("Admin");
            var isEditor = User.IsInRole("Editor");
            ViewBag.ShowDateUserProduct = isAdmin || isEditor;

            if (category == null)
            {
                return NotFound();
            }

            var products = category.Products.AsQueryable();

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

            var totalItems = products.Count();
            var paginatedProducts = products.Skip((page - 1) * perPage).Take(perPage).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / perPage);
            ViewBag.Category = category;
            ViewBag.Products = paginatedProducts;

            ViewBag.PaginationBaseUrl = $"/Categories/Show/{id}?sortOrder=" + sortOrder + "&page=";

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Category category)
        {
            if(ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["message"] = "The category has been added successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                return View(category);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);

            if (ModelState.IsValid)
            {

                category.CategoryName = requestCategory.CategoryName;
                db.SaveChanges();
                TempData["message"] = "The category has been edited successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {            
            Category category = db.Categories.Include("Products")
                                             .Include("Products.Reviews")
                                             .Where(c => c.CategoryId == id)
                                             .First();

            db.Categories.Remove(category);

            TempData["message"] = "The category has been deleted successfully.";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
