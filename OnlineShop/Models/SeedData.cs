using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

//Pasul 4: Adaugam SeedData.cs in folderul Models
namespace OnlineShop.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificăm dacă în baza de date există cel puțin un rol
                if (context.Roles.Any())
                {
                    return; // baza de date conține deja roluri
                }

                // CREAREA ROLURILOR ÎN BD
                context.Roles.AddRange(
                    new IdentityRole
                    {
                        Id = "7b336cf4-a47b-4581-9294-f636b0aa7ee0",
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new IdentityRole
                    {
                        Id = "7b336cf4-a47b-4581-9294-f636b0aa7ee1",
                        Name = "Editor",
                        NormalizedName = "EDITOR"
                    },
                    new IdentityRole
                    {
                        Id = "7b336cf4-a47b-4581-9294-f636b0aa7ee2",
                        Name = "User",
                        NormalizedName = "USER"
                    }
                );

                // O nouă instanță pe care o vom utiliza pentru crearea parolelor utilizatorilor
                var hasher = new PasswordHasher<ApplicationUser>();

                // CREAREA UTILIZATORILOR ÎN BD
                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "e50b44eb-1fe1-4194-ae7c-9066a6c829c0",
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                    new ApplicationUser
                    {
                        Id = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1",
                        UserName = "editor@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "EDITOR@TEST.COM",
                        Email = "editor@test.com",
                        NormalizedUserName = "EDITOR@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Editor1!")
                    },
                    new ApplicationUser
                    {
                        Id = "e50b44eb-1fe1-4194-ae7c-9066a6c829c2",
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    }
                );

                // ASOCIEREA UTILIZATORILOR CU ROLURI
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "7b336cf4-a47b-4581-9294-f636b0aa7ee0",
                        UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c0"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "7b336cf4-a47b-4581-9294-f636b0aa7ee1",
                        UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "7b336cf4-a47b-4581-9294-f636b0aa7ee2",
                        UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c2"
                    }
                );

                // SEEDING CATEGORIES
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { CategoryName = "Mobile Phones" },
                        new Category { CategoryName = "Laptops" },
                        new Category { CategoryName = "Tablets" },
                        new Category { CategoryName = "Televisions" },
                        new Category { CategoryName = "Home Appliances" }
                    );
                }

                // SEEDING PRODUCTS
                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                        // Category 1: Mobile Phones
                        new Product
                        {
                            Title = "Apple iPhone 16 Pro Max, 256GB",
                            Description = "The iPhone 16 Pro features a grade 5 titanium design with a refined new finish.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 959.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Samsung Galaxy S24 Ultra, 512GB",
                            Description = "Welcome to the new era of AI. With the Galaxy S24 Ultra, you can explore a new world of creativity and productivity.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 849.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Apple iPhone 15, 128GB",
                            Description = "Dynamic Island displays alerts and live activities so you won’t miss anything while multitasking.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 399.99m,
                            Stock = 30,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "OnePlus 12 Pro, 256GB",
                            Description = "Speed and performance redefined with the Snapdragon 8 Gen 3.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 599.99m,
                            Stock = 25,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Google Pixel 9 Pro, 128GB",
                            Description = "AI-enhanced photography and cutting-edge Android.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 649.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Xiaomi 13 Pro, 256GB",
                            Description = "High-quality camera and efficient performance in a sleek design.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 699.99m,
                            Stock = 30,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Sony Xperia 1 V, 256GB",
                            Description = "Perfect for content creators with advanced video capabilities.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 799.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Oppo Find X6 Pro, 512GB",
                            Description = "Experience innovation with advanced AI features.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 799.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Realme GT 5, 512GB",
                            Description = "Unmatched speed and design at an incredible value.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 499.99m,
                            Stock = 25,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Motorola Edge 40 Pro, 256GB",
                            Description = "Cutting-edge technology and an immersive display.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 629.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 1,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        // Category 2: Laptops
                        new Product
                        {
                            Title = "Apple MacBook Pro 14, M2 Pro, 512GB SSD",
                            Description = "Unparalleled performance with the M2 Pro chip and a stunning Retina display.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1299.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "ASUS ROG Strix G16, Intel i7, 16GB RAM",
                            Description = "Built for gamers and creators, offering extreme performance.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 749.99m,
                            Stock = 25,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Lenovo ThinkPad X1 Carbon Gen 11",
                            Description = "Slim, lightweight, and durable, ideal for business.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1099.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "HP Pavilion 15, AMD Ryzen 7, 512GB SSD",
                            Description = "Excellent performance in a compact design.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 459.99m,
                            Stock = 30,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Dell XPS 15, Intel i9, 1TB SSD",
                            Description = "High performance in a sleek, lightweight body.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1399.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Razer Blade 15, RTX 4080, 1TB SSD",
                            Description = "Powerful gaming laptop with a stunning display.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 2199.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Acer Swift X, AMD Ryzen 5, 512GB SSD",
                            Description = "Compact and efficient for students and professionals.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 749.99m,
                            Stock = 25,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Microsoft Surface Laptop 5, 256GB SSD",
                            Description = "Style meets performance in this elegant design.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1299.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "MSI Prestige 14, Intel i7, 1TB SSD",
                            Description = "Ultra-portable and stylish with top-notch performance.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1149.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Gigabyte Aero 16, OLED Display, 1TB SSD",
                            Description = "Perfect for creators with vivid visuals and powerful specs.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1949.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 2,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        // Category 3: Tablets
                        new Product
                        {
                            Title = "Apple iPad Pro 12.9, M2, 128GB",
                            Description = "The iPad Pro redefines the boundaries of productivity.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 599.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Samsung Galaxy Tab S9, 256GB",
                            Description = "Power and portability with a stunning AMOLED display.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 449.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Lenovo Tab P11 Pro, 128GB",
                            Description = "Perfect for entertainment and multitasking.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 199.99m,
                            Stock = 25,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Microsoft Surface Pro 9, Intel i5",
                            Description = "A laptop and tablet in one device.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 699.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Apple iPad Air 5, M1, 256GB",
                            Description = "Incredible power in a lightweight design.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 399.99m,
                            Stock = 25,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Huawei MatePad Pro 11, 128GB",
                            Description = "Sleek and professional tablet with HarmonyOS.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 499.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Xiaomi Pad 6 Pro, 256GB",
                            Description = "Affordable performance with a stunning display.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 349.99m,
                            Stock = 30,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Amazon Fire Max 11, 64GB",
                            Description = "Affordable tablet for entertainment and e-books.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 149.99m,
                            Stock = 50,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Realme Pad X, 128GB",
                            Description = "High-quality design for everyday tasks.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 299.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Asus ROG Flow Z13, Intel i7, 512GB SSD",
                            Description = "Gaming tablet with superior power and versatility.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1499.99m,
                            Stock = 5,
                            CreatedDate = DateTime.Now,
                            CategoryId = 3,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        // Category 4: Televisions
                        new Product
                        {
                            Title = "Samsung QLED 4K 65Q80C",
                            Description = "Breathtaking image quality and vibrant colors with QLED technology.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 649.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "LG OLED 4K 55C3",
                            Description = "Premium cinematic experience with OLED technology.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 739.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Sony BRAVIA XR 75X95J",
                            Description = "Natural image quality and immersive sound.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 999.99m,
                            Stock = 5,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "TCL Mini-LED 4K 65C935",
                            Description = "Next-generation Mini-LED technology for superior contrast.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 499.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Sony BRAVIA A90J OLED 65\"",
                            Description = "True-to-life colors and cinematic sound.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1149.99m,
                            Stock = 8,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Philips Ambilight 55OLED807",
                            Description = "Immersive viewing experience with Ambilight technology.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 899.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Panasonic JZ2000 OLED 65\"",
                            Description = "Professional-grade picture quality with Dolby Vision.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1299.99m,
                            Stock = 6,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Hisense U8H Mini-LED 65\"",
                            Description = "Affordable Mini-LED TV with excellent HDR performance.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 799.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Vizio P-Series Quantum 75\"",
                            Description = "Quantum Dot technology at an unbeatable price.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1099.99m,
                            Stock = 8,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Sharp Aquos 4K 70\"",
                            Description = "Exceptional clarity and brightness at a reasonable price.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 699.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 4,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        // Category 5: Home Appliances
                        new Product
                        {
                            Title = "Dyson V15 Detect Vacuum Cleaner",
                            Description = "Cordless, lightweight vacuum cleaner with laser dust detection.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 649.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Philips Air Fryer XXL",
                            Description = "Healthier cooking with rapid air technology and family-size capacity.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 229.99m,
                            Stock = 25,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Bosch Serie 6 Washing Machine",
                            Description = "Eco-friendly washing with energy-saving technology.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 699.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "LG NeoChef Smart Microwave",
                            Description = "Smart inverter technology for even cooking and reheating.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 279.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Breville Barista Express Espresso Machine",
                            Description = "Professional coffee making at home with built-in grinder.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 599.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "KitchenAid Artisan Stand Mixer",
                            Description = "Iconic design and unmatched versatility for home baking.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 349.99m,
                            Stock = 15,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Samsung Smart Refrigerator Family Hub",
                            Description = "Touchscreen-enabled smart fridge for a connected home.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 1999.99m,
                            Stock = 5,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Tefal Turbo Pro Steam Iron",
                            Description = "Advanced steam system for perfect wrinkle removal.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 79.99m,
                            Stock = 30,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Eufy RoboVac G30",
                            Description = "Smart robotic vacuum cleaner with precise navigation.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 249.99m,
                            Stock = 20,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        },
                        new Product
                        {
                            Title = "Honeywell Air Purifier HPA300",
                            Description = "High-efficiency air purifier for clean and healthy air.",
                            ImagePath = "/images/DefaultImage.jpg",
                            Price = 219.99m,
                            Stock = 10,
                            CreatedDate = DateTime.Now,
                            CategoryId = 5,
                            UserId = "e50b44eb-1fe1-4194-ae7c-9066a6c829c1"
                        }

                    );
                }

                context.SaveChanges();
            }
        }
    }
}