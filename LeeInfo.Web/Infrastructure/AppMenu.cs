using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeInfo.Data;
using Microsoft.EntityFrameworkCore;

namespace LeeInfo.Web.Infrastructure
{
    public class AppMenu
    {
        private readonly AppDbContext _dbContext;
        private DbSet<AppSidemenu> _sidemenu;

        public AppMenu(AppDbContext context)
        {
            this._dbContext = context;
            _sidemenu = context.Set<AppSidemenu>();
        }
   
        public List<AppSidemenu> GetAll(List<string> roles)
        {
            var temp=_sidemenu.AsQueryable().ToList();
            List<AppSidemenu> menu = new List<AppSidemenu>();
            foreach(var t in temp)
            {
                foreach(var r in roles)
                {
                    if (t.Description.IndexOf(r) != -1)
                        menu.Add(t);
                }
            }
            return menu;
        }
    }
}
