using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LeeInfo.Data.AppIdentity
{
    public class AppIdentityDbContext : IdentityDbContext<AppIdentityUser>
    {
        public virtual DbSet<AspNetUserForexAccount> AspNetUserForexAccount { get; set; }
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
