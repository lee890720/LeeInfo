using LeeInfo.Data;
using LeeInfo.Web.Areas.CreditCard.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeInfo.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LeeInfo.Web.ViewComponents
{
    public class ForexViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public ForexViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var forex = _context.FrxCbotset.Where(x => x.Signal != null);
            return View(await forex.ToListAsync());
        }
    }
}
