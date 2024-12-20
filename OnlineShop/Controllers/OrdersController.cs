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
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using static OnlineShop.Models.CartProducts;
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Route("Orders/NewByUser/{cartId}")]
        [Authorize(Roles = "Admin,Editor,User")]
        public IActionResult NewByUser(int cartId)
        {
            var sanitizer = new HtmlSanitizer(); // Initializam sanitizer-ul

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

            // Cream un obiect Order cu campurile implicite si curatam datele
            var order = new Order
            {
                CartId = cart.CartId,
                PaymentMethod = sanitizer.Sanitize(""), // Implicit si curatat
                ShippingAddress = sanitizer.Sanitize(""), // Implicit si curatat
                Status = "Pending" // Status implicit pentru comanda noua
            };

            return View(order); // Pasam obiectul Order direct in View
        }

        [HttpPost]
        [Route("Orders/PlaceOrderByUser")]
        [Authorize(Roles = "Admin,Editor,User")]
        public IActionResult PlaceOrderByUser(Order order)
        {
            var sanitizer = new HtmlSanitizer(); // Initializam sanitizer-ul

            // Curatam input-urile din formular
            order.PaymentMethod = sanitizer.Sanitize(order.PaymentMethod);
            order.ShippingAddress = sanitizer.Sanitize(order.ShippingAddress);

            // Validam modelul
            if (!ModelState.IsValid)
            {
                TempData["message"] = "Please correct the errors in the form.";
                TempData["messageType"] = "alert-danger";
                return View("NewByUser", order); // Reincarcam formularul cu datele introduse
            }

            // Verificam daca exista cartul si daca are produse
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

            // Verificam stocul produselor
            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Quantity > cartProduct.Product.Stock)
                {
                    TempData["message"] = $"Not enough stock for product {cartProduct.Product.Title}. Available: {cartProduct.Product.Stock}.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("NewByUser", new { cartId = order.CartId });
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
            return RedirectToAction("ShowOrderHistory", "Orders");
        }




        [HttpGet]
        [Route("Orders/New/{cartId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult New(int cartId)
        {
            var sanitizer = new HtmlSanitizer(); // Initializam sanitizer-ul

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

            // Cream un obiect Order cu campurile implicite si curatam datele
            var order = new Order
            {
                CartId = cart.CartId,
                PaymentMethod = sanitizer.Sanitize(""), // Implicit si curatat
                ShippingAddress = sanitizer.Sanitize(""), // Implicit si curatat
                Status = "Pending" // Status implicit pentru comanda noua
            };

            return View(order); // Pasam obiectul Order direct in View
        }

        [HttpPost]
        [Route("Orders/PlaceOrder")]
        [Authorize(Roles = "Admin")]
        public IActionResult PlaceOrder(Order order)
        {
            var sanitizer = new HtmlSanitizer(); // Initializam sanitizer-ul

            // Curatam input-urile din formular
            order.PaymentMethod = sanitizer.Sanitize(order.PaymentMethod);
            order.ShippingAddress = sanitizer.Sanitize(order.ShippingAddress);

            // Validam modelul
            if (!ModelState.IsValid)
            {
                TempData["message"] = "Please correct the errors in the form.";
                TempData["messageType"] = "alert-danger";
                return View("New", order); // Reincarcam formularul cu datele introduse
            }

            // Verificam daca exista cartul si daca are produse
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

            // Verificam stocul produselor
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
        [Authorize(Roles = "Admin")]
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

            // Stocam referinta la cosul asociat comenzii
            var orderCart = order.Cart;

            // Returnam cantitatile produselor in stoc
            if (orderCart?.CartProducts != null)
            {
                foreach (var cartProduct in orderCart.CartProducts)
                {
                    var product = cartProduct.Product;
                    if (product != null)
                    {
                        product.Stock += cartProduct.Quantity;
                    }
                }

                // Stergem produsele din cos
                db.CartProducts.RemoveRange(orderCart.CartProducts);
            }

            // Stergem comanda
            db.Orders.Remove(order);

            // Stergem si cosul asociat comenzii
            if (orderCart != null)
            {
                db.Carts.Remove(orderCart);
            }

            // Salvam modificarile in baza de date
            db.SaveChanges();

            TempData["message"] = "Order and its associated cart deleted successfully. Stock updated.";
            TempData["messageType"] = "alert-success";

            // Redirectionam utilizatorul
            return RedirectToAction("Index", "Orders");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Editor,User")]
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


        [HttpGet]
        [Route("Orders/Edit/{orderId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int orderId)
        {
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

            return View(order); // Trimitem comanda în View pentru editare
        }


        [HttpPost]
        [Route("Orders/Edit/{orderId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Order order)
        {
            var sanitizer = new HtmlSanitizer();
            order.PaymentMethod = sanitizer.Sanitize(order.PaymentMethod);
            order.ShippingAddress = sanitizer.Sanitize(order.ShippingAddress);

            if (!ModelState.IsValid)
            {
                // Reincarci din baza de date pentru a avea Cart si CartProducts
                var fullOrder = db.Orders
                                  .Include(o => o.Cart)
                                      .ThenInclude(c => c.CartProducts)
                                          .ThenInclude(cp => cp.Product)
                                  .FirstOrDefault(o => o.OrderId == order.OrderId);

                if (fullOrder == null)
                {
                    TempData["message"] = "Order not found.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Orders");
                }

                // Suprascrii campurile editabile cu cele trimise din formular
                fullOrder.PaymentMethod = order.PaymentMethod;
                fullOrder.ShippingAddress = order.ShippingAddress;
                fullOrder.Status = order.Status; // Actualizare și a câmpului Status

                // Returnezi acest fullOrder către View
                return View(fullOrder);
            }

            var existingOrder = db.Orders.FirstOrDefault(o => o.OrderId == order.OrderId);
            if (existingOrder == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Orders");
            }

            existingOrder.PaymentMethod = order.PaymentMethod;
            existingOrder.ShippingAddress = order.ShippingAddress;
            existingOrder.Status = order.Status; // Actualizare și a câmpului Status

            db.SaveChanges();

            TempData["message"] = "Order updated successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Show", "Orders", new { orderId = order.OrderId });
        }


        [HttpPost]
        [Route("Orders/UpdateProductQuantity")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProductQuantity([FromForm] int orderId, [FromForm] int productId)
        {

            // Incercare de a analiza noua valoare introdusa pentru cantitate
            string newQuantityString = Request.Form["newQuantity"];
            if (!int.TryParse(newQuantityString, out int newQuantity))
            {
                // Non-numeric input
                TempData["message"] = "Invalid quantity. Please enter a numeric value.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }


            // Preia comanda si produsele asociate
            var order = db.Orders
                          .Include(o => o.Cart)
                              .ThenInclude(c => c.CartProducts)
                                  .ThenInclude(cp => cp.Product)
                          .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            var cartProduct = order.Cart?.CartProducts?.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                TempData["message"] = "Product not found in cart.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            int oldQuantity = cartProduct.Quantity;
            int difference = newQuantity - oldQuantity;


            // Daca noua cantitate este 0, elimina produsul si restabileste stocul acestuia
            if (newQuantity == 0)
            {
                if (oldQuantity > 0 && cartProduct.Product?.Stock != null)
                {
                    cartProduct.Product.Stock += oldQuantity;
                }

                db.Remove(cartProduct);
                db.SaveChanges();

                TempData["message"] = "The product has been removed from the order.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            // Daca noua cantitate este mai mare decat cea veche, scade stocul
            if (difference > 0)
            {
                if (cartProduct.Product?.Stock == null || cartProduct.Product.Stock < difference)
                {
                    TempData["message"] = $"Not enough stock for product {cartProduct.Product?.Title}. Available: {cartProduct.Product?.Stock}.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Edit", new { orderId });
                }

                cartProduct.Product.Stock -= difference;
                cartProduct.Quantity = newQuantity;
            }
            else if (difference < 0)
            {
                // Noua cantitate este mai mica, creste stocul
                int restoreAmount = Math.Abs(difference);
                if (cartProduct.Product?.Stock != null)
                {
                    cartProduct.Product.Stock += restoreAmount;
                }
                cartProduct.Quantity = newQuantity;
            }
            else
            {
                // difference == 0 inseamna ca nu exista nicio modificare
                return RedirectToAction("Edit", new { orderId });
            }

            db.SaveChanges();

            TempData["message"] = "Product quantity updated successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Edit", new { orderId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Orders/AddProductToOrder")]
        public IActionResult AddProductToOrder(int orderId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                TempData["message"] = "Quantity must be greater than zero.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            if (productId <= 0)
            {
                TempData["message"] = "Product id must be greater than zero.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            var order = db.Orders
                .Include(o => o.Cart)
                    .ThenInclude(c => c.CartProducts)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                TempData["message"] = "Product not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            if (product.Stock == null || product.Stock < quantity)
            {
                TempData["message"] = $"Not enough stock for product {product.Title}.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Edit", new { orderId });
            }

            // Verificam daca produsul exista deja in lista
            var existingCartProduct = order.Cart.CartProducts
                .FirstOrDefault(cp => cp.ProductId == productId);

            if (existingCartProduct != null)
            {
                // Produsul exista deja, marim doar cantitatea
                existingCartProduct.Quantity += quantity;
                product.Stock -= quantity; // scadem stocul cu cantitatea nou adaugata
            }
            else
            {
                // Cream un nou CartProduct
                var newCartProduct = new CartProducts.CartProduct
                {
                    CartId = order.Cart.CartId,
                    ProductId = productId,
                    Quantity = quantity
                };

                product.Stock -= quantity; // scadem stocul
                db.CartProducts.Add(newCartProduct);
            }

            db.SaveChanges();

            TempData["message"] = "Product added successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Edit", new { orderId });
        }





    }
}