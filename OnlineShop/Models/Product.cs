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
        public string Title { get; set; }

        // TODO
        // validare custom folosind atribute personalizate 
        // [Max200CharsValidation] 
        [Required(ErrorMessage = "Description is required. Please enter a description.")]
        public string? Description { get; set; }

        // stocare imagine
        public string? ImagePath { get; set; }

        [Required(ErrorMessage = "Price is required. Please enter a price.")]
        public decimal? Price { get; set; }

        public int? Stock { get; set; }

        public DateTime? CreatedDate { get; set; }

        public decimal? SalePercentage { get; set; } = 0;

        //[NotMapped]
        //public decimal ReducedPrice => Price.HasValue ? Price.Value - (Price.Value * (SalePercentage ?? 0) / 100) : 0;

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

        public decimal Rating { get; set; } = 0;

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


-- Mobile Phones (CategoryId = 1)
INSERT INTO Products (Title, Description, ImagePath, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId)
VALUES
('Apple iPhone 16 Pro Max, 256GB', 'The iPhone 16 Pro features a grade 5 titanium design with a refined new finish.', '/images/DefaultImage.jpg', 959.99, 10, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Samsung Galaxy S24 Ultra, 512GB', 'Welcome to the new era of AI. With the Galaxy S24 Ultra, you can explore a new world of creativity and productivity.', '/images/DefaultImage.jpg', 849.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Apple iPhone 15, 128GB', 'Dynamic Island displays alerts and live activities so you won’t miss anything while multitasking.', '/images/DefaultImage.jpg', 399.99, 30, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('OnePlus 12 Pro, 256GB', 'Speed and performance redefined with the Snapdragon 8 Gen 3.', '/images/DefaultImage.jpg', 599.99, 25, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Google Pixel 9 Pro, 128GB', 'AI-enhanced photography and cutting-edge Android.', '/images/DefaultImage.jpg', 649.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Xiaomi 13 Pro, 256GB', 'High-quality camera and efficient performance in a sleek design.', '/images/DefaultImage.jpg', 699.99, 30, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Sony Xperia 1 V, 256GB', 'Perfect for content creators with advanced video capabilities.', '/images/DefaultImage.jpg', 799.99, 15, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Oppo Find X6 Pro, 512GB', 'Experience innovation with advanced AI features.', '/images/DefaultImage.jpg', 799.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Realme GT 5, 512GB', 'Unmatched speed and design at an incredible value.', '/images/DefaultImage.jpg', 499.99, 25, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Motorola Edge 40 Pro, 256GB', 'Cutting-edge technology and an immersive display.', '/images/DefaultImage.jpg', 629.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Laptops (CategoryId = 2)
INSERT INTO Products (Title, Description, ImagePath, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId)
VALUES
('Apple MacBook Pro 14, M2 Pro, 512GB SSD', 'Unparalleled performance with the M2 Pro chip and a stunning Retina display.', '/images/DefaultImage.jpg', 1299.99, 15, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('ASUS ROG Strix G16, Intel i7, 16GB RAM', 'Built for gamers and creators, offering extreme performance.', '/images/DefaultImage.jpg', 749.99, 25, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Lenovo ThinkPad X1 Carbon Gen 11', 'Slim, lightweight, and durable, ideal for business.', '/images/DefaultImage.jpg', 1099.99, 20, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('HP Pavilion 15, AMD Ryzen 7, 512GB SSD', 'Excellent performance in a compact design.', '/images/DefaultImage.jpg', 459.99, 30, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Dell XPS 15, Intel i9, 1TB SSD', 'High performance in a sleek, lightweight body.', '/images/DefaultImage.jpg', 1399.99, 10, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Razer Blade 15, RTX 4080, 1TB SSD', 'Powerful gaming laptop with a stunning display.', '/images/DefaultImage.jpg', 2199.99, 10, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Acer Swift X, AMD Ryzen 5, 512GB SSD', 'Compact and efficient for students and professionals.', '/images/DefaultImage.jpg', 749.99, 25, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Microsoft Surface Laptop 5, 256GB SSD', 'Style meets performance in this elegant design.', '/images/DefaultImage.jpg', 1299.99, 15, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('MSI Prestige 14, Intel i7, 1TB SSD', 'Ultra-portable and stylish with top-notch performance.', '/images/DefaultImage.jpg', 1149.99, 20, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Gigabyte Aero 16, OLED Display, 1TB SSD', 'Perfect for creators with vivid visuals and powerful specs.', '/images/DefaultImage.jpg', 1949.99, 10, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Tablets (CategoryId = 3)
INSERT INTO Products (Title, Description, ImagePath, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId)
VALUES
('Apple iPad Pro 12.9, M2, 128GB', 'The iPad Pro redefines the boundaries of productivity.', '/images/DefaultImage.jpg', 599.99, 15, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Samsung Galaxy Tab S9, 256GB', 'Power and portability with a stunning AMOLED display.', '/images/DefaultImage.jpg', 449.99, 20, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Lenovo Tab P11 Pro, 128GB', 'Perfect for entertainment and multitasking.', '/images/DefaultImage.jpg', 199.99, 25, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Microsoft Surface Pro 9, Intel i5', 'A laptop and tablet in one device.', '/images/DefaultImage.jpg', 699.99, 10, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Apple iPad Air 5, M1, 256GB', 'Incredible power in a lightweight design.', '/images/DefaultImage.jpg', 399.99, 25, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Huawei MatePad Pro 11, 128GB', 'Sleek and professional tablet with HarmonyOS.', '/images/DefaultImage.jpg', 499.99, 15, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Xiaomi Pad 6 Pro, 256GB', 'Affordable performance with a stunning display.', '/images/DefaultImage.jpg', 349.99, 30, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Amazon Fire Max 11, 64GB', 'Affordable tablet for entertainment and e-books.', '/images/DefaultImage.jpg', 149.99, 50, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Realme Pad X, 128GB', 'High-quality design for everyday tasks.', '/images/DefaultImage.jpg', 299.99, 20, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Asus ROG Flow Z13, Intel i7, 512GB SSD', 'Gaming tablet with superior power and versatility.', '/images/DefaultImage.jpg', 1499.99, 5, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Televisions (CategoryId = 4)
INSERT INTO Products (Title, Description, ImagePath, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId)
VALUES
('Samsung QLED 4K 65Q80C', 'Breathtaking image quality and vibrant colors with QLED technology.', '/images/DefaultImage.jpg', 649.99, 15, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('LG OLED 4K 55C3', 'Premium cinematic experience with OLED technology.', '/images/DefaultImage.jpg', 739.99, 10, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Sony BRAVIA XR 75X95J', 'Natural image quality and immersive sound.', '/images/DefaultImage.jpg', 999.99, 5, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('TCL Mini-LED 4K 65C935', 'Next-generation Mini-LED technology for superior contrast.', '/images/DefaultImage.jpg', 499.99, 20, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Sony BRAVIA A90J OLED 65"', 'True-to-life colors and cinematic sound.', '/images/DefaultImage.jpg', 1149.99, 8, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Philips Ambilight 55OLED807', 'Immersive viewing experience with Ambilight technology.', '/images/DefaultImage.jpg', 899.99, 10, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Panasonic JZ2000 OLED 65"', 'Professional-grade picture quality with Dolby Vision.', '/images/DefaultImage.jpg', 1299.99, 6, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Hisense U8H Mini-LED 65"', 'Affordable Mini-LED TV with excellent HDR performance.', '/images/DefaultImage.jpg', 799.99, 15, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Vizio P-Series Quantum 75"', 'Quantum Dot technology at an unbeatable price.', '/images/DefaultImage.jpg', 1099.99, 8, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Sharp Aquos 4K 70"', 'Exceptional clarity and brightness at a reasonable price.', '/images/DefaultImage.jpg', 699.99, 20, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Home Appliances (CategoryId = 5)
INSERT INTO Products (Title, Description, ImagePath, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId)
VALUES
('Dyson V15 Detect Vacuum Cleaner', 'Cordless, lightweight vacuum cleaner with laser dust detection.', '/images/DefaultImage.jpg', 649.99, 15, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Philips Air Fryer XXL', 'Healthier cooking with rapid air technology and family-size capacity.', '/images/DefaultImage.jpg', 229.99, 25, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Bosch Serie 6 Washing Machine', 'Eco-friendly washing with energy-saving technology.', '/images/DefaultImage.jpg', 699.99, 10, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('LG NeoChef Smart Microwave', 'Smart inverter technology for even cooking and reheating.', '/images/DefaultImage.jpg', 279.99, 20, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Breville Barista Express Espresso Machine', 'Professional coffee making at home with built-in grinder.', '/images/DefaultImage.jpg', 599.99, 10, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('KitchenAid Artisan Stand Mixer', 'Iconic design and unmatched versatility for home baking.', '/images/DefaultImage.jpg', 349.99, 15, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Samsung Smart Refrigerator Family Hub', 'Touchscreen-enabled smart fridge for a connected home.', '/images/DefaultImage.jpg', 1999.99, 5, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Tefal Turbo Pro Steam Iron', 'Advanced steam system for perfect wrinkle removal.', '/images/DefaultImage.jpg', 79.99, 30, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Eufy RoboVac G30', 'Smart robotic vacuum cleaner with precise navigation.', '/images/DefaultImage.jpg', 249.99, 20, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'),
('Honeywell Air Purifier HPA300', 'High-efficiency air purifier for clean and healthy air.', '/images/DefaultImage.jpg', 219.99, 10, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');


*/