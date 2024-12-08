using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Collections.Generic;


//Pasull 4: Crearea clasei SeedData
//se creeaza utilizatori si roluri in baza de date
//folosim Service Provider pentru a injecta dependentele in baza de date
public static class SeedData
{

    //Id-urile sunt generate cu GUID Generator
    public static void Initialize(IServiceProvider
    serviceProvider)
    {
        using (var context = new ApplicationDbContext(
        serviceProvider.GetRequiredService
        <DbContextOptions<ApplicationDbContext>>()))
        {
            // Verificam daca in baza de date exista cel putin un
            //rol
            // insemnand ca a fost rulat codul
            // De aceea facem return pentru a nu insera rolurile
            //inca o data
            // Acesta metoda trebuie sa se execute o singura data
            if (context.Roles.Any())
            {
                return; // baza de date contine deja roluri
            }

            // CREAREA ROLURILOR IN BD
            // daca nu contine roluri, acestea se vor crea
            context.Roles.AddRange(

            new IdentityRole
            {
                Id = "24c07985-fd13-4347-92da-c0e5817fbef0",
                Name = "Admin",
                NormalizedName = "Admin".ToUpper()
            },


            new IdentityRole
            {
                Id = "24c07985-fd13-4347-92da-c0e5817fbef1",
                Name = "Editor",
                NormalizedName = "Editor".ToUpper()
            },


            new IdentityRole
            {
                Id = "24c07985-fd13-4347-92da-c0e5817fbef2",
                Name = "User",
                NormalizedName = "User".ToUpper()
            }


            );

            // o noua instanta pe care o vom utiliza pentru
            //crearea parolelor utilizatorilor
            // parolele sunt de tip hash
            var hasher = new PasswordHasher<ApplicationUser>();

            // CREAREA USERILOR IN BD
            // Se creeaza cate un user pentru fiecare rol
            context.Users.AddRange(
            new ApplicationUser

            {

                Id = "520606d1-0f9b-4582-a4a1-c1b9a74579e0",
                // primary key
                UserName = "admin@test.com",
                EmailConfirmed = true,
                NormalizedEmail = "ADMIN@TEST.COM",
                Email = "admin@test.com",
                NormalizedUserName = "ADMIN@TEST.COM",
                PasswordHash = hasher.HashPassword(null,

            "Admin1!")
            },

            new ApplicationUser
            {

                Id = "520606d1-0f9b-4582-a4a1-c1b9a74579e1",
                // primary key
                UserName = "editor@test.com",
                EmailConfirmed = true,
                NormalizedEmail = "EDITOR@TEST.COM",
                Email = "editor@test.com",
                NormalizedUserName = "EDITOR@TEST.COM",
                PasswordHash = hasher.HashPassword(null,

            "Editor1!")
            },

        new ApplicationUser
        {

                Id = "520606d1-0f9b-4582-a4a1-c1b9a74579e2",
                // primary key
                UserName = "user@test.com",
                EmailConfirmed = true,
                NormalizedEmail = "USER@TEST.COM",
                Email = "user@test.com",
                NormalizedUserName = "USER@TEST.COM",
                PasswordHash = hasher.HashPassword(null,

                "User1!")
            }
);

            // ASOCIEREA USER-ROLE
            context.UserRoles.AddRange(
            new IdentityUserRole<string>
            {

                RoleId = "24c07985-fd13-4347-92da-c0e5817fbef0",


                UserId = "520606d1-0f9b-4582-a4a1-c1b9a74579e0"
            },

            new IdentityUserRole<string>

            {

                RoleId = "24c07985-fd13-4347-92da-c0e5817fbef1",


                UserId = "520606d1-0f9b-4582-a4a1-c1b9a74579e1"
            },

            new IdentityUserRole<string>

            {

                RoleId = "24c07985-fd13-4347-92da-c0e5817fbef2",


                UserId = "520606d1-0f9b-4582-a4a1-c1b9a74579e2"
            }
            );
            context.SaveChanges();
        }
    }
}