using LeeInfo.Data.AppIdentity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Data.AppIdentity
{
    public static class SeedData
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            var serviceScopeFactory = (IServiceScopeFactory)webHost.Services.GetService(typeof(IServiceScopeFactory));

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<AppIdentityDbContext>();
                dbContext.Database.Migrate();
                if (!dbContext.Users.Any())
                {
                    PasswordHasher<AppIdentityUser> passwordHasher = new PasswordHasher<AppIdentityUser>();
                    var user = new AppIdentityUser
                    {
                        UserName = "kk7k7",
                        Email = "kk7k7@126.com"
                    };
                    var pwd = passwordHasher.HashPassword(user, "kk777kk");
                    dbContext.Users.AddRange(
                        new AppIdentityUser
                        {
                            UserName = "kk7k7",
                            Email = "kk7k7@126.com",
                            PasswordHash = pwd
                        });
                    dbContext.SaveChanges();
                }
                if (!dbContext.Roles.Any())
                {
                    dbContext.Roles.AddRange(
                    new IdentityRole
                    {
                        Name="Administrator"
                    });
                    dbContext.SaveChanges();
                }
                if (!dbContext.UserRoles.Any())
                {
                    dbContext.UserRoles.AddRange(
                        new IdentityUserRole<string>
                        {
                            UserId = dbContext.Users.First().Id,
                            RoleId = dbContext.Roles.First().Id
                        });
                    dbContext.SaveChanges();
                }
            }
            return webHost;
        }
    }
}
