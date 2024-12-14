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
            if(TempData.ContainsKey("message"))
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
            int perPage = 3;
            var category = db.Categories.Include(c => c.Products)
                                        .FirstOrDefault(c => c.CategoryId == id);

            var totalItems = category.Products.Count();
            var paginatedProducts = category.Products.Skip((page - 1) * perPage).Take(perPage).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / perPage);
            ViewBag.Category = category;
            ViewBag.Products = paginatedProducts;

            ViewBag.PaginationBaseUrl = $"/Categories/Show/{id}?page=";

            return View(category);
        }


        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
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

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
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
