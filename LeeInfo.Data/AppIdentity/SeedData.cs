using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

// dotnet aspnet-codegenerator razorpage -m Contact -dc AppIdentityDbContext -outDir Pages\Contacts --referenceScriptLibraries
namespace LeeInfo.Data.AppIdentity
{
    public static class SeedData
    {
        #region snippet_Initialize
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration config)
        {
            using (var context = new AppIdentityDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppIdentityDbContext>>()))
            {
                // For sample purposes we are seeding 2 users both with the same password.
                // The password is set with the following command:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, config["Data:AdminUser:Password"],config["Data:AdminUser:UserName"]);
                await EnsureRole(serviceProvider, adminID, config["Data:AdminUser:RoleName"]);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<AppIdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new AppIdentityUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<AppIdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        #endregion

        public static void SeedDB(AppIdentityDbContext context, string adminID)
        {
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            context.Users.AddRange(
                new AppIdentityUser
                {
                    UserName = "lee_dh",
                    Email = "lee_dh@126.com"
                });
            context.SaveChanges();
        }
    }
}