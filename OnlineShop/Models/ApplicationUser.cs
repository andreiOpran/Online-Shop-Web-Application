using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnlineShop.Models
{
    //Pasul 1 :Extidem functionalitatea clasei IdentityUser
    public class ApplicationUser : IdentityUser
    {

        //un user poate avea mai multe produse
        public virtual ICollection<Product>? Products { get; set; }

        //un user poate avea mai multe carturi
        public virtual ICollection<Cart>? Carts { get; set; }

        //un user poate avea mai multe review-uri
        public virtual ICollection<Review>? Reviews { get; set; }


        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        //variabila care va fi folosita pentru a stoca rolul selectat de utilizator
        //pentru popularea unui dropdown
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
