using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using static OnlineShop.Models.CartProducts;

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
        public virtual Category? Category { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Categories { get; set; }

        // un produs este publicat de catre un utilizator (utilizator colaborator)
        // foreign key
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // un produs are review-uri; review-ul nu este un camp obligatoriu
        public virtual ICollection<Review>? Reviews { get; set; }

        // pentru relatia de many-to-many
        public virtual ICollection<CartProduct>? CartProducts { get; set; }

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


/*

V1
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES  ('Telefon mobil Apple iPhone 16 Pro Max, 256GB', 'iPhone 16 Pro prezinta un design din titan de grad 5 cu un nou finisaj rafinat.', NULL, NULL, 9599.99, 10, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Telefon mobil Samsung Galaxy S24 Ultra, 512 GB', 'Bun venit in noua era AI. Cu Galaxy S24 Ultra poti descoperi o noua lume a creativitatii si productivitatii.', NULL, NULL, 8499.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Telefon mobil Apple iPhone 15, 128GB', 'Dynamic Island afiseaza alerte si Activitati live, ca sa nu ratezi nimic in timp ce te ocupi de altceva.', NULL, NULL, 3999.99, 30, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');


V2
-- Mobile Phones (CategoryId = 1)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Apple iPhone 16 Pro Max, 256GB', 'The iPhone 16 Pro features a grade 5 titanium design with a refined new finish.', NULL, NULL, 9599.99, 10, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Galaxy S24 Ultra, 512GB', 'Welcome to the new era of AI. With the Galaxy S24 Ultra, you can explore a new world of creativity and productivity.', NULL, NULL, 8499.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Apple iPhone 15, 128GB', 'Dynamic Island displays alerts and live activities so you won’t miss anything while multitasking.', NULL, NULL, 3999.99, 30, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Laptops (CategoryId = 2)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Apple MacBook Pro 14, M2 Pro, 512GB SSD', 'Unparalleled performance with the M2 Pro chip and a stunning Retina display.', NULL, NULL, 12999.99, 15, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('ASUS ROG Strix G16, Intel i7, 16GB RAM', 'Built for gamers and creators, offering extreme performance.', NULL, NULL, 7499.99, 25, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Lenovo ThinkPad X1 Carbon Gen 11', 'Slim, lightweight, and durable, ideal for business.', NULL, NULL, 10999.99, 20, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('HP Pavilion 15, AMD Ryzen 7, 512GB SSD', 'Excellent performance in a compact design.', NULL, NULL, 4599.99, 30, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Tablets (CategoryId = 3)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Apple iPad Pro 12.9, M2, 128GB', 'The iPad Pro redefines the boundaries of productivity.', NULL, NULL, 5999.99, 15, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Galaxy Tab S9, 256GB', 'Power and portability with a stunning AMOLED display.', NULL, NULL, 4499.99, 20, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Lenovo Tab P11 Pro, 128GB', 'Perfect for entertainment and multitasking.', NULL, NULL, 1999.99, 25, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Microsoft Surface Pro 9, Intel i5', 'A laptop and tablet in one device.', NULL, NULL, 6999.99, 10, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Televisions (CategoryId = 4)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Samsung QLED 4K 65Q80C', 'Breathtaking image quality and vibrant colors with QLED technology.', NULL, NULL, 6499.99, 15, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('LG OLED 4K 55C3', 'Premium cinematic experience with OLED technology.', NULL, NULL, 7399.99, 10, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Sony BRAVIA XR 75X95J', 'Natural image quality and immersive sound.', NULL, NULL, 9999.99, 5, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Home Appliances (CategoryId = 5)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Bosch Serie 6 Washing Machine, 8kg', 'Energy-efficient and excellent washing results.', NULL, NULL, 2999.99, 20, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Xiaomi Roborock S7 Robot Vacuum Cleaner', 'Efficient cleaning and simultaneous mopping.', NULL, NULL, 2699.99, 25, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Microwave Oven, 28L', 'Cook quickly and easily with smart functions.', NULL, NULL, 899.99, 30, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('LG Side-by-Side Refrigerator, 635L', 'Large capacity and efficient cooling.', NULL, NULL, 7999.99, 10, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Bosch Induction Hob, 4 Zones', 'Elegant design with state-of-the-art technology.', NULL, NULL, 3799.99, 15, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');


*/