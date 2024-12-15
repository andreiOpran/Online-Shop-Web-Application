using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required. Please enter a category name.")]
        public string? CategoryName { get; set; }

        // dintr-o categorie fac parte mai multe produse
        public virtual ICollection<Product>? Products { get; set; }


        // TODO
        // validare pe serviciu (IValidatableObject)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CategoryName != null && CategoryName.Length > 256)
            {
                yield return new ValidationResult("Category name must be at most 256 characters long.");
            }
            if (CategoryName != null && CategoryName.Length < 3)
            {
                yield return new ValidationResult("Category name must be at least 3 characters long.");
            }
            yield return ValidationResult.Success!;

        }
    }
}

/*


INSERT INTO Categories (CategoryName) VALUES ('Mobile Phones');
INSERT INTO Categories (CategoryName) VALUES ('Laptops');
INSERT INTO Categories (CategoryName) VALUES ('Tablets');
INSERT INTO Categories (CategoryName) VALUES ('Televisions');
INSERT INTO Categories (CategoryName) VALUES ('Home Appliances');

*/
