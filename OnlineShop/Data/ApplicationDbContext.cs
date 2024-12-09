using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static OnlineShop.Models.CartProducts;
using System.Reflection.Emit;
using OnlineShop.Models;

//Pasul 3: Adaugam ApplicationDbContext.cs in folderul Data
namespace OnlineShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //definirea relatiei many-to-many intre Cart si Product
            base.OnModelCreating(builder);

            //definire primary key compus pentru CartProduct
            builder.Entity<CartProduct>().HasKey(cp => new { cp.Id, cp.ProductId, cp.CartId });

            builder.Entity<CartProduct>()
                .HasOne(cp => cp.Cart)
                .WithMany(cp => cp.CartProducts) // Fixed line
                .HasForeignKey(cp => cp.CartId);

            builder.Entity<CartProduct>()
                .HasOne(cp => cp.Product)
                .WithMany(cp => cp.CartProducts) // Fixed line
                .HasForeignKey(cp => cp.ProductId);
        }
    }
}