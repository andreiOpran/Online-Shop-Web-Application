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
namespace OnlineShop.Controllers

{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OrdersController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = _db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            // Preluam toate comenzile, incluzand datele relevante
            var orders = db.Orders
                           .Include(o => o.Cart)
                               .ThenInclude(c => c.User) // Include si utilizatorul asociat cosului
                           .Include(o => o.Cart.CartProducts)
                           .ThenInclude(cp => cp.Product) // Include produsele asociate
                           .OrderByDescending(o => o.OrderDate) // Sortam comenzile descrescator dupa data
                           .ToList();

            // Calculam totalul comenzilor
            var totalOrders = orders.Count;

            // Calculam totalul produselor din cosuri (din toate comenzile)
            var totalProductsInCarts = orders
                .SelectMany(o => o.Cart.CartProducts)
                .Sum(cp => cp.Quantity);

            // Stocam aceste valori în ViewBag pentru a fi utilizate în View
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalProductsInCarts = totalProductsInCarts;

            // Verificăm dacă există comenzi
            if (!orders.Any())
            {
                TempData["message"] = "No orders found.";
                TempData["messageType"] = "alert-warning";
            }

            return View(orders);
        }



        [HttpGet]
        [Route("Orders/Show/{orderId}")]
        public IActionResult Show(int orderId)
        {
            // Gasim comanda dupa ID si includem detalii despre cos, produse si utilizator
            var order = db.Orders
                          .Include(o => o.Cart)
                              .ThenInclude(c => c.CartProducts)
                                  .ThenInclude(cp => cp.Product)
                          .Include(o => o.Cart.User) // Includem utilizatorul asociat cosului
                          .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            // Calculam totalul comenzii
            decimal totalPrice = order.Cart.CartProducts
                                      .Where(cp => cp.Product != null)
                                      .Sum(cp => (cp.Product.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice.ToString("F2");

            return View(order);
        }



        [HttpGet]
        [Route("Orders/New/{cartId}")]
        public IActionResult New(int cartId)
        {
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .Include(c => c.User)
                         .FirstOrDefault(c => c.CartId == cartId);

            if (cart == null || !cart.CartProducts.Any())
            {
                TempData["message"] = "Cart is empty or invalid.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            // Cream un obiect Order cu campurile implicite
            var order = new Order
            {
                CartId = cart.CartId,
                PaymentMethod = "", // Implicit
                ShippingAddress = "", // Implicit
                Status = "Pending"   // Status implicit pentru comanda noua
            };

            return View(order); // Pasam obiectul Order direct în View
        }


        [HttpPost]
        [Route("Orders/PlaceOrder")]
        public IActionResult PlaceOrder(Order order)
        {
            var cart = db.Carts
                         .Include(c => c.CartProducts)
                             .ThenInclude(cp => cp.Product)
                         .FirstOrDefault(c => c.CartId == order.CartId);

            if (cart == null || !cart.CartProducts.Any())
            {
                TempData["message"] = "Cart is empty or invalid.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Quantity > cartProduct.Product.Stock)
                {
                    TempData["message"] = $"Not enough stock for product {cartProduct.Product.Title}. Available: {cartProduct.Product.Stock}.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("New", new { cartId = order.CartId });
                }

                // Reducem stocul produsului
                cartProduct.Product.Stock -= cartProduct.Quantity;
            }

            // Actualizam campurile comenzii
            order.OrderDate = DateTime.Now; // Setam data plasarii
            order.Status = "Pending";      // Status implicit pentru comenzile noi

            // Adaugam comanda in baza de date
            db.Orders.Add(order);

            // Facem cosul inactiv
            cart.IsActive = false;

            db.SaveChanges();

            TempData["message"] = "Order placed successfully!";
            TempData["messageType"] = "alert-success";

            // Redirectionam catre pagina de detalii a comenzii
            return RedirectToAction("Show", "Orders", new { orderId = order.OrderId });
        }


        [HttpPost]
        [Route("Orders/Delete/{orderId}")]
        public IActionResult Delete(int orderId)
        {
            // Gasim comanda dupa ID
            var order = db.Orders
                          .Include(o => o.Cart)
                              .ThenInclude(c => c.CartProducts)
                              .ThenInclude(cp => cp.Product)
                          .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Orders");
            }

            // Obtinem toate cosurile utilizatorului
            var userCarts = db.Carts
                              .Where(c => c.UserId == order.Cart.UserId)
                              .ToList();

            // Dezactivam toate cosurile utilizatorului
            foreach (var cart in userCarts)
            {
                cart.IsActive = false;
            }

            // Activam un singur cos (de exemplu, cosul asociat comenzii curente)
            if (order.Cart != null)
            {
                order.Cart.IsActive = true;

                // Golim cosul de produse
                if (order.Cart.CartProducts != null)
                {
                    db.CartProducts.RemoveRange(order.Cart.CartProducts);
                }
            }

            // Returnam cantitatile produselor in stoc
            if (order.Cart?.CartProducts != null)
            {
                foreach (var cartProduct in order.Cart.CartProducts)
                {
                    var product = cartProduct.Product;
                    if (product != null)
                    {
                        product.Stock += cartProduct.Quantity;
                    }
                }
            }

            // Stergem comanda
            db.Orders.Remove(order);

            // Salvam modificările în baza de date
            db.SaveChanges();

            TempData["message"] = "Order deleted successfully. All carts deactivated except one. Stock updated.";
            TempData["messageType"] = "alert-success";

            // Redirectionam utilizatorul
            return RedirectToAction("Index", "Orders");
        }
        [HttpGet]
        [Route("Orders/Edit/{orderId}")]
        public IActionResult Edit(int orderId)
        {
            // Gasim comanda dupa ID
            var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Returnam comanda catre view
            return View(order);
        }

        [HttpPost]
        [Route("Orders/Edit/{orderId}")]
        public IActionResult Edit(Order updatedOrder)
        {
            // Gasim comanda existenta
            var order = db.Orders.FirstOrDefault(o => o.OrderId == updatedOrder.OrderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Actualizam doar campurile editabile
            order.PaymentMethod = updatedOrder.PaymentMethod;
            order.ShippingAddress = updatedOrder.ShippingAddress;

            // Salvam modificarile
            db.SaveChanges();

            TempData["message"] = "Order updated successfully.";
            TempData["messageType"] = "alert-success";

            // Redirectionam catre pagina de detalii
            return RedirectToAction("Show", new { orderId = order.OrderId });
        }
        [HttpPost]
        [Route("Orders/UpdateStatus")]
        public IActionResult UpdateStatus(int OrderId, string Status)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderId == OrderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            order.Status = Status;
            db.SaveChanges();

            TempData["message"] = $"Order status updated to '{Status}'.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult ShowOrderHistory()
        {
            // Preluam ID-ul utilizatorului curent
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["message"] = "Unable to retrieve the current user.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            // Gasim toate comenzile utilizatorului curent si includem relatiile necesare
            var orders = db.Orders
                            .Include(o => o.Cart)
                                .ThenInclude(c => c.User) // Include utilizatorul asociat Cart
                            .Include(o => o.Cart)
                                .ThenInclude(c => c.CartProducts)
                                .ThenInclude(cp => cp.Product)
                            .Where(o => o.Cart.UserId == userId) // Filtram comenzile dupa utilizatorul curent
                            .OrderByDescending(o => o.OrderDate) // Sortam descrescator dupa data comenzii
                            .ToList();

            if (!orders.Any())
            {
                TempData["message"] = "You have no orders in your history.";
                TempData["messageType"] = "alert-warning";
                return RedirectToAction("Index", "Products");
            }

            return View("ShowOrderHistory", orders);
        }
    }
}