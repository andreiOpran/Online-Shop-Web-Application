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


-- Mobile Phones (CategoryId = 1)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Apple iPhone 16 Pro Max, 256GB', 'The iPhone 16 Pro features a grade 5 titanium design with a refined new finish.', NULL, NULL, 9599.99, 10, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Galaxy S24 Ultra, 512GB', 'Welcome to the new era of AI. With the Galaxy S24 Ultra, you can explore a new world of creativity and productivity.', NULL, NULL, 8499.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Apple iPhone 15, 128GB', 'Dynamic Island displays alerts and live activities so you won’t miss anything while multitasking.', NULL, NULL, 3999.99, 30, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('OnePlus 12 Pro, 256GB', 'Speed and performance redefined with the Snapdragon 8 Gen 3.', NULL, NULL, 5999.99, 25, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Google Pixel 9 Pro, 128GB', 'AI-enhanced photography and cutting-edge Android.', NULL, NULL, 6499.99, 20, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Xiaomi 14 Pro, 512GB', 'Innovative technology with an elegant design.', NULL, NULL, 4499.99, 15, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Sony Xperia 1 VI, 256GB', 'Professional-grade photography on a smartphone.', NULL, NULL, 7499.99, 10, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Motorola Edge 40 Ultra, 128GB', 'A perfect balance of power and elegance.', NULL, NULL, 3799.99, 30, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Huawei Mate 60 Pro, 512GB', 'Seamless performance with next-gen HarmonyOS.', NULL, NULL, 8299.99, 12, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Realme GT 5, 256GB', 'Flagship performance at an unbeatable price.', NULL, NULL, 2999.99, 40, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('OPPO Find X6 Pro, 256GB', 'Photography-focused smartphone with stunning clarity.', NULL, NULL, 4999.99, 18, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Vivo X100 Pro+, 256GB', 'Capture every moment in exceptional detail.', NULL, NULL, 5599.99, 15, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Nokia XR21, 128GB', 'Built to last with military-grade durability.', NULL, NULL, 2999.99, 25, NOW(), NULL, 1, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Laptops (CategoryId = 2)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Apple MacBook Pro 14, M2 Pro, 512GB SSD', 'Unparalleled performance with the M2 Pro chip and a stunning Retina display.', NULL, NULL, 12999.99, 15, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('ASUS ROG Strix G16, Intel i7, 16GB RAM', 'Built for gamers and creators, offering extreme performance.', NULL, NULL, 7499.99, 25, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Lenovo ThinkPad X1 Carbon Gen 11', 'Slim, lightweight, and durable, ideal for business.', NULL, NULL, 10999.99, 20, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('HP Pavilion 15, AMD Ryzen 7, 512GB SSD', 'Excellent performance in a compact design.', NULL, NULL, 4599.99, 30, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Dell XPS 15, Intel i9, 1TB SSD', 'High performance in a sleek, lightweight body.', NULL, NULL, 13999.99, 10, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Razer Blade 15, NVIDIA RTX 4080', 'Ultimate gaming laptop with powerful graphics.', NULL, NULL, 15999.99, 8, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Microsoft Surface Laptop 5, Intel i7', 'Slim, elegant, and built for productivity.', NULL, NULL, 8499.99, 12, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Acer Swift X, AMD Ryzen 9, 1TB SSD', 'Lightweight design with uncompromised power.', NULL, NULL, 6599.99, 20, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('HP Omen 17, NVIDIA RTX 4090', 'Gaming-focused laptop with cutting-edge technology.', NULL, NULL, 18999.99, 5, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('ASUS ZenBook Pro Duo, Intel i7', 'Dual-screen innovation for creators.', NULL, NULL, 11499.99, 8, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Gigabyte Aero 16 OLED, Intel i9', '4K OLED display with extreme power.', NULL, NULL, 12999.99, 7, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Lenovo Legion 5 Pro, AMD Ryzen 7', 'Designed for gamers with precision performance.', NULL, NULL, 7599.99, 15, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('MSI Creator Z16P, NVIDIA RTX 4070', 'Perfect blend of aesthetics and performance.', NULL, NULL, 10999.99, 12, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Apple MacBook Air 15, M2', 'Lightweight and powerful for everyday tasks.', NULL, NULL, 7999.99, 20, NOW(), NULL, 2, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Tablets (CategoryId = 3)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Apple iPad Pro 12.9, M2, 128GB', 'The iPad Pro redefines the boundaries of productivity.', NULL, NULL, 5999.99, 15, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Galaxy Tab S9, 256GB', 'Power and portability with a stunning AMOLED display.', NULL, NULL, 4499.99, 20, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Lenovo Tab P11 Pro, 128GB', 'Perfect for entertainment and multitasking.', NULL, NULL, 1999.99, 25, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Microsoft Surface Pro 9, Intel i5', 'A laptop and tablet in one device.', NULL, NULL, 6999.99, 10, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Apple iPad Air 5, M1, 256GB', 'Incredible power in a lightweight design.', NULL, NULL, 3999.99, 25, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Galaxy Tab S8 Ultra, 512GB', 'Ultimate tablet experience with a 14.6" AMOLED screen.', NULL, NULL, 6299.99, 10, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Microsoft Surface Go 4, Intel i3', 'Compact and versatile for work and play.', NULL, NULL, 2999.99, 30, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Huawei MatePad Pro 12.6, 256GB', 'High-performance tablet for creativity and productivity.', NULL, NULL, 4299.99, 18, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Amazon Fire HD 10 Plus, 64GB', 'Affordable and powerful with Alexa built-in.', NULL, NULL, 999.99, 40, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Lenovo Yoga Tab 13, 128GB', 'Immersive entertainment with Dolby Atmos.', NULL, NULL, 2999.99, 25, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Xiaomi Pad 6 Pro, 256GB', 'Powerful performance with a sleek design.', NULL, NULL, 2699.99, 20, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Realme Pad X, 128GB', 'All-day battery life and seamless multitasking.', NULL, NULL, 1999.99, 30, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Google Pixel Tablet, 128GB', 'Smart home hub and entertainment device.', NULL, NULL, 3799.99, 12, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Sony Xperia Tablet Z6, 256GB', 'Crystal-clear display and superior audio.', NULL, NULL, 4999.99, 15, NOW(), NULL, 3, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Televisions (CategoryId = 4)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Samsung QLED 4K 65Q80C', 'Breathtaking image quality and vibrant colors with QLED technology.', NULL, NULL, 6499.99, 15, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('LG OLED 4K 55C3', 'Premium cinematic experience with OLED technology.', NULL, NULL, 7399.99, 10, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Sony BRAVIA XR 75X95J', 'Natural image quality and immersive sound.', NULL, NULL, 9999.99, 5, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('TCL Mini-LED 4K 65C935', 'Next-generation Mini-LED technology for superior contrast.', NULL, NULL, 4999.99, 20, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Sony BRAVIA A90J OLED 65"', 'True-to-life colors and cinematic sound.', NULL, NULL, 11499.99, 8, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Hisense U8H 4K 75"', 'Exceptional brightness with ULED technology.', NULL, NULL, 6999.99, 15, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Philips Ambilight 58PUS9007', 'Immersive viewing with Ambilight technology.', NULL, NULL, 5499.99, 18, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Panasonic LZ2000 OLED 65"', 'Industry-leading OLED performance and sound.', NULL, NULL, 13499.99, 5, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Sharp AQUOS 4K 60"', 'Affordable 4K experience with vibrant colors.', NULL, NULL, 3999.99, 25, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Neo QLED 8K 75QN900B', 'Stunning 8K resolution with advanced HDR.', NULL, NULL, 18999.99, 3, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('LG NanoCell 4K 55NANO85', 'Enhanced colors with NanoCell technology.', NULL, NULL, 3799.99, 30, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Vizio M-Series Quantum 4K 70"', 'Exceptional value with Quantum Dot technology.', NULL, NULL, 4599.99, 20, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung The Frame 55"', 'Art when it’s off, TV when it’s on.', NULL, NULL, 6299.99, 12, NOW(), NULL, 4, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');

-- Home Appliances (CategoryId = 5)
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Bosch Serie 6 Washing Machine, 8kg', 'Energy-efficient and excellent washing results.', NULL, NULL, 2999.99, 20, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Xiaomi Roborock S7 Robot Vacuum Cleaner', 'Efficient cleaning and simultaneous mopping.', NULL, NULL, 2699.99, 25, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Microwave Oven, 28L', 'Cook quickly and easily with smart functions.', NULL, NULL, 899.99, 30, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('LG Side-by-Side Refrigerator, 635L', 'Large capacity and efficient cooling.', NULL, NULL, 7999.99, 10, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Bosch Induction Hob, 4 Zones', 'Elegant design with state-of-the-art technology.', NULL, NULL, 3799.99, 15, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');
INSERT INTO Products (Title, Description, Image, ImageContentType, Price, Stock, CreatedDate, SalePercentage, CategoryId, UserId) VALUES ('Dyson V15 Detect Cordless Vacuum', 'Revolutionary cleaning with laser dust detection.', NULL, NULL, 3299.99, 20, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Philips Air Fryer XXL', 'Healthy frying with minimal oil.', NULL, NULL, 1299.99, 30, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Whirlpool 10kg Front Load Washing Machine', 'Energy-efficient with a sleek design.', NULL, NULL, 4999.99, 10, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Samsung Smart Refrigerator Family Hub, 700L', 'Revolutionary fridge with a smart display.', NULL, NULL, 10999.99, 8, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Instant Pot Duo Plus, 6L', 'All-in-one pressure cooker and slow cooker.', NULL, NULL, 799.99, 50, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('iRobot Roomba J7+', 'AI-powered robot vacuum with smart mapping.', NULL, NULL, 3999.99, 15, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Panasonic Microwave Oven, 30L', 'Premium microwave with inverter technology.', NULL, NULL, 1299.99, 25, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Bosch Series 8 Dishwasher', 'Quiet and efficient with advanced cleaning.', NULL, NULL, 4999.99, 12, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('Miele Coffee Machine CM6360', 'Brew barista-quality coffee at home.', NULL, NULL, 7499.99, 10, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1'), ('LG TWINWash Washer and Dryer', 'Innovative dual washer with efficient drying.', NULL, NULL, 9999.99, 5, NOW(), NULL, 5, 'e50b44eb-1fe1-4194-ae7c-9066a6c829c1');


*/