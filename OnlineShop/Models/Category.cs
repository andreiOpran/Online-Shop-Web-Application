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
    }
}

/*

INSERT INTO Categories (CategoryName) VALUES ('Mobile Phones');
INSERT INTO Categories (CategoryName) VALUES ('Laptops');
INSERT INTO Categories (CategoryName) VALUES ('Tablets');
INSERT INTO Categories (CategoryName) VALUES ('Televisions');
INSERT INTO Categories (CategoryName) VALUES ('Home Appliances');

*/
