using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;

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

                context.SaveChanges();
            }
        }
    }
}