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


        // stegrere comentariu
        [HttpPost]
        // TODO
        public IActionResult Delete(int id)
        {
            Review review = db.Reviews.Find(id);


            //if( /* user-ul are permisiuni*/)
            //{
                db.Reviews.Remove(review);
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductId);
            //}
            //else
            //{
            //    TempData["message"] = "You do not have permission to delete this review.";
            //    TempData["messageType"] = "alert-danger";
            //    return RedirectToAction("Index", "Products");
            //}

        }

        // edit review intr-o pagina separata
        // [HttpGet] implicit
        // TODO
        // [Authorize(Roles = "")]
        public IActionResult Edit(int id)
        {
            Review review = db.Reviews.Find(id);

            //if ( /* user-ul are permisiuni*/)
            //{
                return View(review);
            //}
            //else
            //{
            //    TempData["message"] = "You do not have permission to edit this review.";
            //    TempData["messageType"] = "alert-danger";
            //    return RedirectToAction("Index", "Products");
            //}
        }


        // edit review
        [HttpPost]
        // TODO
        // [Authorize(Roles = "")]
        public IActionResult Edit(int id, Review requestReview)
        {
            Review review = db.Reviews.Find(id);

            //if (/* user-ul are permisiuni */)
            //{
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
                
            //}
            //else
            //{
            //    TempData["message"] = "You do not have permission to edit this review.";
            //    TempData["messageType"] = "alert-danger";
            //    return RedirectToAction("Index", "Products");
            //}

        }
    }
}
