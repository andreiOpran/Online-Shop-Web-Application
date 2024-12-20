using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var carts = db.Carts.Include("User").Include("CartProducts");

            ViewBag.Carts = carts;

            if (!carts.Any())
            {
                TempData["message"] = "No carts found.";
                TempData["messageType"] = "alert-warning";
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Editor,User")]
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
                TempData["message"] = "Add a product to your cart.";
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult IncreaseQuantityShowById(int productId, int cartId)
        {
            decimal totalPrice;

            // Gasim cosul prin ID si includem utilizatorul
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

            // Calculam totalul pretului
            totalPrice = cart.CartProducts
                             .Where(cp => cp.Product != null)
                             .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice.ToString("F2");

            // Verificam daca cosul este activ
            if (!cart.IsActive)
            {
                TempData["message"] = "This cart is associated with an order. To modify it, the order must be canceled or updated.";
                TempData["messageType"] = "alert-danger";
                return View("ShowById", cart);
            }

            // Gasim produsul in cos
            var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                TempData["message"] = "Product not found in cart.";
                TempData["messageType"] = "alert-danger";
                return View("ShowById", cart);
            }

            // Verificam daca exista suficiente produse in stoc
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null || product.Stock < cartProduct.Quantity + 1)
            {
                TempData["message"] = "Not enough stock available to increase quantity.";
                TempData["messageType"] = "alert-danger";
                return View("ShowById", cart);
            }

            // Crestem cantitatea produsului in cos
            cartProduct.Quantity++;

            // Salvam modificarile
            db.SaveChanges();

            // Recalculam totalul pretului dupa modificare
            totalPrice = cart.CartProducts
                             .Where(cp => cp.Product != null)
                             .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice.ToString("F2");

            TempData["message"] = "Product quantity increased in cart.";
            TempData["messageType"] = "alert-success";

            return View("ShowById", cart);
        }

        [HttpPost]
        [Route("Carts/DecreaseQuantityShowById/{cartId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DecreaseQuantityShowById(int productId, int cartId)
        {
            decimal totalPrice;

            // Gasim cosul prin ID si includem utilizatorul
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

            // Calculam totalul pretului
            totalPrice = cart.CartProducts
                             .Where(cp => cp.Product != null)
                             .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice.ToString("F2");

            // Verificam daca cosul este activ
            if (!cart.IsActive)
            {
                TempData["message"] = "This cart is associated with an order. To modify it, the order must be canceled or updated.";
                TempData["messageType"] = "alert-danger";
                return View("ShowById", cart);
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

            // Salvam modificarile
            db.SaveChanges();

            // Recalculam totalul pretului dupa modificare
            totalPrice = cart.CartProducts
                             .Where(cp => cp.Product != null)
                             .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice.ToString("F2");

            return View("ShowById", cart);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor,User")]
        public IActionResult IncreaseQuantityShowByUser(int productId)
        {
            var userId = _userManager.GetUserId(User);

            // Preluam cosul activ al utilizatorului cu toate relatiile necesare
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .Include(c => c.User)
                         .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            // Calculam Total Price chiar daca apar erori
            decimal totalPrice = cart?.CartProducts
                                    .Where(cp => cp.Product != null)
                                    .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity) ?? 0;
            ViewBag.TotalPrice = totalPrice.ToString("F2");

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

            // Verificam stocul
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null || product.Stock < cartProduct.Quantity + 1)
            {
                TempData["message"] = "Not enough stock available to increase quantity.";
                TempData["messageType"] = "alert-danger";
                return View("ShowByUser", cart);
            }

            // Crestem cantitatea
            cartProduct.Quantity++;
            db.SaveChanges();

            // Recalculam totalul
            totalPrice = cart.CartProducts
                             .Where(cp => cp.Product != null)
                             .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);
            ViewBag.TotalPrice = totalPrice.ToString("F2");

            TempData["message"] = "Product quantity increased in cart.";
            TempData["messageType"] = "alert-success";

            return View("ShowByUser", cart);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Editor,User")]
        public IActionResult DecreaseQuantityShowByUser(int productId)
        {
            var userId = _userManager.GetUserId(User);

            // Preluam cosul activ al utilizatorului cu toate relatiile necesare
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .Include(c => c.User)
                         .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            // Calculam Total Price chiar daca apar erori
            decimal totalPrice = cart?.CartProducts
                                    .Where(cp => cp.Product != null)
                                    .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity) ?? 0;
            ViewBag.TotalPrice = totalPrice.ToString("F2");

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

            // Reducem cantitatea
            cartProduct.Quantity--;

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

            db.SaveChanges();

            // Recalculam totalul
            totalPrice = cart.CartProducts
                             .Where(cp => cp.Product != null)
                             .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);
            ViewBag.TotalPrice = totalPrice.ToString("F2");

            return View("ShowByUser", cart);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Carts/CheckStockAndRedirectById/{cartId}")]
        public IActionResult CheckStockAndRedirectById(int cartId)
        {
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .FirstOrDefault(c => c.CartId == cartId);

            if (cart == null || !cart.IsActive || !cart.CartProducts.Any())
            {
                TempData["message"] = "Cart is empty, invalid, or inactive.";
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
        [Authorize(Roles = "Admin,Editor,User")]
        public IActionResult CheckStockAndRedirectByUser()
        {
            var userId = _userManager.GetUserId(User);

            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null || !cart.IsActive || !cart.CartProducts.Any())
            {
                TempData["message"] = "You cannot place an order with an empty cart.";
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
            return RedirectToAction("NewByUser", "Orders", new { cartId = cart.CartId });
        }





    }
}
