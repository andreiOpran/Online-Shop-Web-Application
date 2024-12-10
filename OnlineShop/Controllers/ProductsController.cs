using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

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

            int perPage = 2;
            int totalItems = products.Count();

            // /Products/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // pagina 1 -> offset = 0, pagina 2 -> offset = 2, pagina 3 -> offset = 4 ...
            var offset = 0;
            if(!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * perPage;
            }

            // preluam produsele pentru pagina curenta
            var paginatedProducts = products.Skip(offset).Take(perPage);

            // numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)perPage);

            // trimitem produsele catre view
            ViewBag.Products = paginatedProducts;

            // verificare search

            if(search != "")
            {
                ViewBag.PaginationBaseUrl = "/Products/Index?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?page";
            }

            return View();
                   
        }


        // TODO
        // afisare un singur produs in functie de id imprepuna cu categoria
        // se preiau si review-urile produsului
        //public IActionResult Show(int id)
        //{
        //    Product product = db.Products.Include("Category")
        //                                 .Include("Reviews")
        //                                 .Include("Reviews.User")
        //                                 .Where(p => p.ProductId == id)
        //                                 .First();

        //    if (TempData.ContainsKey("message"))
        //    {
        //        ViewBag.Message = TempData["message"];
        //        ViewBag.Alert = TempData["messageType"];
        //    }

        //    return View(product);
        //}

    }
}
