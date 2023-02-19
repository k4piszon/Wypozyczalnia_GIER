using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Company.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Company.DAL
{
    public class Initializer : DropCreateDatabaseIfModelChanges<CompanyContext>
    {
        protected override void Seed(CompanyContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            roleManager.Create(new IdentityRole("Admin"));

            var user = new ApplicationUser { UserName = "xena2021@int.pl" };
            string password = "Inzynier2020!";

            userManager.Create(user, password);

            userManager.AddToRole(user.Id, "Admin");

            var profiles = new List<Profile>
            {
                new Profile
                {
                    UserName = "xena2021@int.pl",
                    Password = "Inzynier2020!",
                    Name = "Jan",
                    Surname = "Nowak",
                    Address = "Aleje Jerozolimskie 33 Warszawa"
                }

            };
            profiles.ForEach(o => context.Profiles.Add(o));
            context.SaveChanges();

        }
    }
}