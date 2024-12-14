using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        //foreign key
        public int? CartId { get; set; }

        public Cart? Cart { get; set; }

       
        public string? Status { get; set; }
        public DateTime? OrderDate { get; set; }

        public string? PaymentMethod { get; set; }

        public string? ShippingAddress { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

    }
}
