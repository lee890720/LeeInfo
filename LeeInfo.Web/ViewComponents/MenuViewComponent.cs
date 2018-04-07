using LeeInfo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Web.ViewComponents
{
    public class MenuViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;
        public MenuViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(IList<string> list_str)
        {
            var temp_menu = await _context.AppMenu.ToListAsync();
            List<AppMenu> menu = new List<AppMenu>();
            foreach (var t in temp_menu)
            {
                foreach (var r in list_str)
                {
                    if (t.Description.IndexOf(r) != -1)
                        menu.Add(t);
                }
            }
            return View(menu);
        }
    }
}
