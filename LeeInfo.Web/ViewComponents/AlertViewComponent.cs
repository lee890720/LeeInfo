using LeeInfo.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeInfo.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LeeInfo.Web.ViewComponents
{
    public class AlertViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public AlertViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<AlertViewModel> list_alert = new List<AlertViewModel>();
            var ccdData = await _context.CcdData.Where(c => c.RepaymentDate.AddDays(-3) < DateTime.Now).ToListAsync();
            if (ccdData.Count != 0)
            {
                list_alert.Add(new AlertViewModel { Info = "CreditCard Bills", Count = ccdData.Count, Ico = "fa fa-credit-card text-green", Url = "/CreditCard/CcdData" });
            }
            var ccdDebt = await _context.CcdDebt.Where(c => c.RepaymentDate.AddDays(-3) < DateTime.Now).ToListAsync();
            if (ccdDebt.Count != 0)
            {
                list_alert.Add(new AlertViewModel { Info = "Debt Bills", Count = ccdDebt.Count, Ico = "fa fa-yen text-yellow", Url = "/CreditCard/CcdDebt" });
            }
            var frxServer = await _context.FrxServer.ToListAsync() ;
            foreach (var f in frxServer)
            {
                if (f.ServerTime.AddMinutes(1) < DateTime.UtcNow)
                {
                    list_alert.Add(new AlertViewModel { Info = f.ServerName+" has a mistaks.", Ico = "fa fa-database text-red", Url = "/" });
                }
            }
            return View(list_alert);
        }
    }
}
