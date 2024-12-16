using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using NuGet.Packaging.Signing;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using static OnlineShop.Models.CartProducts;

namespace OnlineShop.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CartsController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = _db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var carts = db.Carts.Include("User").Include("CartProducts");

            ViewBag.Carts = carts;
            return View();
        }

        [HttpGet]
        public IActionResult ShowByUser()
        {
            // Preluam utilizatorul curent
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["message"] = "Unable to retrieve the current user.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            // Gasim cosul activ asociat utilizatorului curent si includem utilizatorul
            var cart = db.Carts
                            .Include(c => c.CartProducts)
                                .ThenInclude(cp => cp.Product)
                            .Include(c => c.User)
                            .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null)
            {
                TempData["message"] = "No active cart found for the current user.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            // Calculam totalul pretului pentru produsele din cos
            decimal totalPrice = 0; // Schimba tipul la decimal
            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Product != null)
                {
                    totalPrice += (cartProduct.Product.Price ?? 0) * cartProduct.Quantity; // Tratam null pentru Price
                }
            }

            // Setam totalul în ViewBag cu format
            ViewBag.TotalPrice = totalPrice.ToString("F2");

            return View("ShowByUser", cart);
        }

        [HttpGet]
        [Route("Carts/ShowById/{cartId}")]
        public IActionResult ShowById(int cartId)
        {
            // Gasim cosul dupa ID si includem produsele asociate si utilizatorul
            var cart = db.Carts
             .Include(c => c.CartProducts)
             .ThenInclude(cp => cp.Product)
             .Include(c => c.User)
             .FirstOrDefault(c => c.CartId == cartId);

            if (cart == null)
            {
                TempData["message"] = "Cart not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Calculam totalul pretului pentru produsele din cos
            decimal totalPrice = 0;
            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Product != null)
                {
                    totalPrice += (cartProduct.Product.Price ?? 0) * cartProduct.Quantity;
                }
            }

            // Setam totalul în ViewBag cu format
            ViewBag.TotalPrice = totalPrice.ToString("F2");

            // Returnam cosul catre un alt view, numit "ShowById"
            return View("ShowById", cart);
        }



        [HttpPost]
        [Route("Carts/IncreaseQuantityShowById/{cartId}")]
        public IActionResult IncreaseQuantityShowById(int productId, int cartId)
        {
            // Gasim cosul activ prin ID si includem utilizatorul
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .Include(c => c.User) // Adaugam utilizatorul asociat cart-ului
                         .FirstOrDefault(c => c.CartId == cartId);

            if (cart == null)
            {
                TempData["message"] = "Cart not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Gasim produsul in cos
            var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                TempData["message"] = "Product not found in cart.";
                TempData["messageType"] = "alert-danger";
                return View("ShowById", cart);
            }

            // Verificam daca exista suficiente produse în stoc
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null || product.Stock < cartProduct.Quantity + 1)
            {
                TempData["message"] = "Not enough stock available to increase quantity.";
                TempData["messageType"] = "alert-danger";
                return View("ShowById", cart);
            }

            // Crestem cantitatea produsului in cos
            cartProduct.Quantity++;

            // Salvam modificarile in baza de date
            db.SaveChanges();

            // Calculam totalul pretului
            decimal totalPrice = cart.CartProducts
                                     .Where(cp => cp.Product != null)
                                     .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice.ToString("F2");

            TempData["message"] = "Product quantity increased in cart.";
            TempData["messageType"] = "alert-success";

            // Returnam cart-ul, inclusiv utilizatorul asociat
            return View("ShowById", cart);
        }






        [HttpPost]
        [Route("Carts/DecreaseQuantityShowById/{cartId}")]
        public IActionResult DecreaseQuantityShowById(int productId, int cartId)
        {
            // Gasim cosul activ prin ID si includem utilizatorul
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .Include(c => c.User) // Adaugam utilizatorul asociat cart-ului
                         .FirstOrDefault(c => c.CartId == cartId);

            if (cart == null)
            {
                TempData["message"] = "Cart not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Gasim produsul in cos
            var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                TempData["message"] = "Product not found in cart.";
                TempData["messageType"] = "alert-danger";
                return View("ShowById", cart);
            }

            // Reducem cantitatea produsului in cos
            cartProduct.Quantity--;

            // Daca cantitatea ajunge la 0, eliminam produsul din cos
            if (cartProduct.Quantity <= 0)
            {
                db.CartProducts.Remove(cartProduct);
                TempData["message"] = "Product removed from cart.";
                TempData["messageType"] = "alert-warning";
            }
            else
            {
                TempData["message"] = "Product quantity decreased in cart.";
                TempData["messageType"] = "alert-success";
            }

            // Salvam modificarile in baza de date
            db.SaveChanges();

            // Calculam totalul pretului
            decimal totalPrice = cart.CartProducts
                                     .Where(cp => cp.Product != null)
                                     .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice.ToString("F2");

            // Returnam cart-ul, inclusiv utilizatorul asociat
            return View("ShowById", cart);
        }

      

        [HttpPost]
        public IActionResult IncreaseQuantityShowByUser(int productId)
        {
            var userId = _userManager.GetUserId(User);

            // Preluam cosul activ al utilizatorului cu toate relatiile necesare
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .Include(c => c.User)
                         .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null)
            {
                TempData["message"] = "No active cart found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            // Gasim produsul in cos
            var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                TempData["message"] = "Product not found in cart.";
                TempData["messageType"] = "alert-danger";
                return View("ShowByUser", cart);
            }

            // Verificam daca exista suficiente produse in stoc
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null || product.Stock < cartProduct.Quantity + 1)
            {
                TempData["message"] = "Not enough stock available to increase quantity.";
                TempData["messageType"] = "alert-danger";
                return View("ShowByUser", cart);
            }

            // Crestem cantitatea produsului in cos
            cartProduct.Quantity++;

            // Salvam modificarile in baza de date
            db.SaveChanges();

            // Recalculam totalul pretului pentru produsele din cos
            decimal totalPrice = cart.CartProducts
                                     .Where(cp => cp.Product != null)
                                     .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            // Setam totalul in ViewBag cu format
            ViewBag.TotalPrice = totalPrice.ToString("F2");

            TempData["message"] = "Product quantity increased in cart.";
            TempData["messageType"] = "alert-success";

            // Returnam aceleasi date pentru view-ul `ShowByUser`
            return View("ShowByUser", cart);
        }




        [HttpPost]
        public IActionResult DecreaseQuantityShowByUser(int productId)
        {
            var userId = _userManager.GetUserId(User);

            // Preluam cosul activ al utilizatorului cu toate relatiile necesare
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .Include(c => c.User)
                         .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null)
            {
                TempData["message"] = "No active cart found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            // Gasim produsul in cos
            var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                TempData["message"] = "Product not found in cart.";
                TempData["messageType"] = "alert-danger";
                return View("ShowByUser", cart);
            }

            // Reducem cantitatea produsului in cos
            cartProduct.Quantity--;

            // Daca cantitatea ajunge la 0, eliminam produsul din cos
            if (cartProduct.Quantity <= 0)
            {
                db.CartProducts.Remove(cartProduct);
                TempData["message"] = "Product removed from cart.";
                TempData["messageType"] = "alert-warning";
            }
            else
            {
                TempData["message"] = "Product quantity decreased in cart.";
                TempData["messageType"] = "alert-success";
            }

            // Salvam modificarile in baza de date
            db.SaveChanges();

            // Recalculam totalul pretului pentru produsele ramase in coș
            decimal totalPrice = cart.CartProducts
                                     .Where(cp => cp.Product != null)
                                     .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            // Setam totalul în ViewBag cu format
            ViewBag.TotalPrice = totalPrice.ToString("F2");

            // Returnam aceleași date pentru view-ul `ShowByUser`
            return View("ShowByUser", cart);
        }

        [HttpPost]
        [Route("Carts/CheckStockAndRedirectById/{cartId}")]
        public IActionResult CheckStockAndRedirectById(int cartId)
        {
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .FirstOrDefault(c => c.CartId == cartId);

            if (cart == null || !cart.CartProducts.Any())
            {
                TempData["message"] = "Cart is empty or invalid.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("ShowById", new { cartId });
            }

            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Quantity > cartProduct.Product.Stock)
                {
                    TempData["message"] = $"Not enough stock for product {cartProduct.Product.Title}. Available: {cartProduct.Product.Stock}.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("ShowById", new { cartId });
                }
            }

            // Totul este OK, redirectionam către metoda `New` din `OrdersController`
            return RedirectToAction("New", "Orders", new { cartId });
        }

        [HttpPost]
        public IActionResult CheckStockAndRedirectByUser()
        {
            var userId = _userManager.GetUserId(User);

            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null || !cart.CartProducts.Any())
            {
                TempData["message"] = "No active cart found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("ShowByUser");
            }

            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Quantity > cartProduct.Product.Stock)
                {
                    TempData["message"] = $"Not enough stock for product {cartProduct.Product.Title}. Available: {cartProduct.Product.Stock}.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("ShowByUser");
                }
            }

            // Totul este OK, redirectionam către metoda `New` din `OrdersController`
            return RedirectToAction("New", "Orders", new { cartId = cart.CartId });
        }





    }
}
