using OnlineShop.Data;
using OnlineShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArticlesApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;

            return View();
        }

        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            ViewBag.UserCurent = await _userManager.GetUserAsync(User);

            return View(user);
        }

        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); // Lista de nume de roluri

            // Cautam ID-ul rolului in baza de date
            ViewBag.UserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); // Selectam 1 singur rol

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();


            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.FirstName = newData.FirstName;
                user.LastName = newData.LastName;
                user.PhoneNumber = newData.PhoneNumber;


                // Cautam toate rolurile din baza de date
                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    // Scoatem userul din rolurile anterioare
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                // Adaugam noul rol selectat
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(string id)
        {
            // Gaseste utilizatorul impreuna cu Reviews si Carts
            var user = db.Users
                         .Include(u => u.Reviews)   // Incarca recenziile
                         .Include(u => u.Carts)     // Incarca cosurile
                         .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // 1. Sterg recenziile utilizatorului
            if (user.Reviews?.Count > 0)
            {
                db.Reviews.RemoveRange(user.Reviews);
            }

            // 2. Sterg comenzile si produsele asociate cosurilor utilizatorului
            if (user.Carts?.Count > 0)
            {
                foreach (var cart in user.Carts)
                {
                    // Sterg comenzile asociate cosului
                    var orders = db.Orders.Where(o => o.CartId == cart.CartId);
                    if (orders.Any())
                    {
                        db.Orders.RemoveRange(orders);
                    }

                    // Sterg legaturile din CartProducts pentru acest cos
                    var cartProducts = db.CartProducts.Where(cp => cp.CartId == cart.CartId);
                    if (cartProducts.Any())
                    {
                        db.CartProducts.RemoveRange(cartProducts);
                    }
                }

                // Sterg toate cosurile utilizatorului
                db.Carts.RemoveRange(user.Carts);
            }

            // 3. Sterg utilizatorul
            db.Users.Remove(user);

            // Salvarea modificarilor
            db.SaveChanges();

            return RedirectToAction("Index");
        }



        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}