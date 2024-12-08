using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using static OnlineShop.Models.CartProducts;

//Pasul 3: Crearea clasei ApplicationDbContext
//ApplicationDbContext este clasa care se ocupa de interactiunea cu baza de date
//in ea imi declar tabelele si relatia dintre ele
namespace OnlineShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Tabelele din baza de date
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }  

        public DbSet<Category> Categories { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // definirea relatiei many-to-many dintre Article si Bookmark

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<CartProduct>()
                .HasKey(cp => new { cp.Id, cp.CartId, cp.ProductId });

            // definire relatii cu modelele Bookmark si Article (FK)
            modelBuilder.Entity<CartProduct>()
                .HasOne(cp => cp.Product)
                .WithMany(cp => cp.CartProducts) // Fixed navigation property
                .HasForeignKey(cp => cp.ProductId); // Fixed foreign key

            modelBuilder.Entity<CartProduct>()
                .HasOne(cp => cp.Cart)
                .WithMany(cp => cp.CartProducts) // Fixed navigation property
                .HasForeignKey(cp => cp.CartId);
        }
    }
}
