
//Pasul 1: Crearea clasei ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class ApplicationUser:IdentityUser
    {

        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles {  get; set; }


    }
}
//Clasa ApplicationUser mosteneste clasa IdentityUser din namespace-ul Microsoft.AspNetCore.Identity
//ceea ce inseamna ca toate proprietatile si metodele din IdentityUser sunt mostenite de ApplicationUser

//Extinderea modelului de utilizator ,prin crearea unei clase ApplicationUser care mosteneste clasa IdentityUser,
//este utila atunci cand se doreste adaugarea de proprietati suplimentare la modelul de utilizator.