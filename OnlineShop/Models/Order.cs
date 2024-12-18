using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Foreign key
        public int? CartId { get; set; }

        public Cart? Cart { get; set; }

        public string? Status { get; set; }

        public DateTime? OrderDate { get; set; }

        // Validare pentru PaymentMethod
        [Required(ErrorMessage = "Payment method is required.")]
        public string? PaymentMethod { get; set; }

        // Validare pentru ShippingAddress
        [Required(ErrorMessage = "Shipping address is required.")]
        [MinLength(10, ErrorMessage = "Shipping address must be at least 10 characters.")]
        [MaxLength(200, ErrorMessage = "Shipping address cannot exceed 200 characters.")]
        public string? ShippingAddress { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }
    }
}
