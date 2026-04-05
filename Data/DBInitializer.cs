using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeighForce.Models;

namespace WeighForce.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();
            var roles = new List<IdentityRole>{
                new IdentityRole("Admin"),
                new IdentityRole("Operator"),
                new IdentityRole("Manager")
            };
            if (!context.Roles.Any())
            {
                roles.ForEach(role => roleManager.CreateAsync(role).GetAwaiter().GetResult());
            }
            if (!context.Roles.Any(r => r.Name == "Upload"))
            {
                roleManager.CreateAsync(new IdentityRole("Upload")).GetAwaiter().GetResult();
            }
            if (!context.Roles.Any(r => r.Name == "Dispatch"))
            {
                roleManager.CreateAsync(new IdentityRole("Dispatch")).GetAwaiter().GetResult();
            }
            if (!context.Roles.Any(r => r.Name == "Sys"))
            {
                roleManager.CreateAsync(new IdentityRole("Sys")).GetAwaiter().GetResult();
            }
            if (!context.Roles.Any(r => r.Name == "Sync"))
            {
                roleManager.CreateAsync(new IdentityRole("Sync")).GetAwaiter().GetResult();
            }
            if (!context.Roles.Any(r => r.Name == "Link"))
            {
                roleManager.CreateAsync(new IdentityRole("Link")).GetAwaiter().GetResult();
            }
            // if (!context.Users.Any(u => u.Name == "Admin"))
            // {
            //     var user = new ApplicationUser
            //     {
            //         UserName = "admin@wf.app",
            //         Email = "admin@wf.app",
            //         Name = "Admin"
            //     };
            //     userManager.CreateAsync(user, "qwerty").GetAwaiter().GetResult();
            //     userManager.AddToRoleAsync(user, roles.First().Name).GetAwaiter().GetResult();
            //     var sysuser = new ApplicationUser
            //     {
            //         UserName = "system@wf.app",
            //         Email = "system@wf.app",
            //         Name = "System"
            //     };
            //     userManager.CreateAsync(sysuser, "qwertyfldksjak").GetAwaiter().GetResult();
            // }
        }
    }
}