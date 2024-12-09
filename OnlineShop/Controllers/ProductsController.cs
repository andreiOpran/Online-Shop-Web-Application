using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
