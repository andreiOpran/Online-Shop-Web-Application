using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // stergere comentariu
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Review review = db.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (review.UserId != userId && !isAdmin)
            {
                TempData["message"] = "You do not have permission to delete this review.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            db.Reviews.Remove(review);
            db.SaveChanges();
            return Redirect("/Products/Show/" + review.ProductId);
        }

        // edit review intr-o pagina separata
        public IActionResult Edit(int id)
        {
            Review review = db.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (review.UserId != userId && !isAdmin)
            {
                TempData["message"] = "You do not have permission to edit this review.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            return View(review);
        }

        // edit review
        [HttpPost]
        public IActionResult Edit(int id, Review requestReview)
        {
            Review review = db.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (review.UserId != userId && !isAdmin)
            {
                TempData["message"] = "You do not have permission to edit this review.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            if (ModelState.IsValid)
            {
                review.Rating = requestReview.Rating;
                review.Content = requestReview.Content;
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductId);
            }
            else
            {
                return View(requestReview);
            }
        }
    }
}
