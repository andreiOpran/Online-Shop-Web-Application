using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace OnlineShop.Models
{
    public class Product
    {

        [Key]
        public int ProductId { get; set; }

        [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
        [StringLength(50, ErrorMessage = "Title must be at most 50 characters long.")]
        [Required(ErrorMessage = "Title is required. Please enter a title.")]
        public required string Title { get; set; }

        // TODO
        // validare custom folosind atribute personalizate 
        // [Max200CharsValidation] 
        [Required(ErrorMessage = "Description is required. Please enter a description.")]
        public string? Description { get; set; }

        // stocare imagine
        public byte[]? Image { get; set; }
        // stocare tip continut imagine (spre exemplu: "image/png")
        public string? ImageContentType { get; set; }

        [Required(ErrorMessage = "Price is required. Please enter a price.")]
        public decimal? Price { get; set; }
        
        public int? Stock { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public decimal? SalePercentage { get; set; }

        // un produs are o categorie
        // foreign key
        [Required(ErrorMessage = "Category is required. Please select a category.")]
        public int? CategoryId { get; set; }
        // BLOCKED
        public virtual Category category { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Categories { get; set; }

        // un produs este publicat de catre un utilizator (utilizator colaborator)
        // foreign key
        public string? UserId { get; set; }
        // BLOCKED
        public virtual ApplicationUser? User { get; set; }  

        // un produs are review-uri; review-ul nu este un camp obligatoriu
        // BLOCKED
        // public virtual ICollection<Review>? Reviews { get; set; } 


        // TODO
        // validare pe serviciu (IValidatableObject)
        
        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Description != null && Description.Length > 1024)
            {
                yield return new ValidationResult("Description must be at most 256 characters long.");
            }
            if(Description != null && Description.Length < 10)
            {
                yield return new ValidationResult("Description must be at least 10 characters long.");
            }
            if(Price.HasValue && Price < 0)
            {
                yield return new ValidationResult("Price must be a positive number.");
            }
            if(Stock.HasValue && Stock < 0)
            {
                yield return new ValidationResult("Stock must be a positive number.");
            }
            if(SalePercentage.HasValue && (SalePercentage < 0 || SalePercentage > 100))
            {
                yield return new ValidationResult("Sale percentage must be between 0 and 100.");
            }

            // "!" comunica compilatorului ca nu se va returna niciodata null
            yield return ValidationResult.Success!; 
        }
        */

    }
}
