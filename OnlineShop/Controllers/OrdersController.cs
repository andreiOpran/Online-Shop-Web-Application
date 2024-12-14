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
        [Route("Orders/Show/{orderId}")]
        public IActionResult Show(int orderId)
        {
            // Gasim comanda dupa ID si includem cosul si produsele asociate
            var order = db.Orders
                          .Include(o => o.Cart)
                              .ThenInclude(c => c.CartProducts)
                                  .ThenInclude(cp => cp.Product)
                          .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["message"] = "Order not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            return View(order);
        }


    }
}

