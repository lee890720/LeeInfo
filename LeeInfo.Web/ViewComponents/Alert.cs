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
    public class Alert:ViewComponent
    {
        private readonly AppDbContext _context;
        public Alert(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<AlertViewModel> list_alert = new List<AlertViewModel>();
            var ccdData=await _context.CcdData.Where(c => c.RepaymentDate.AddDays(-3) < DateTime.Now).ToListAsync();
            if(ccdData.Count!=0)
            {
                list_alert.Add(new AlertViewModel { Info = "CreditCard Bills", Count = ccdData.Count, Ico = "fa fa-credit-card text-green",Url="/CreditCard/CcdData" });
            }
            var ccdDebt = await _context.CcdDebt.Where(c => c.RepaymentDate.AddDays(-3) < DateTime.Now).ToListAsync();
            if(ccdDebt.Count!=0)
            {
                list_alert.Add(new AlertViewModel { Info = "Debt Bills", Count = ccdDebt.Count, Ico = "fa fa-yen text-yellow" ,Url="/CreditCard/CcdDebt"});
            }
            var frxEcs = await _context.FrxEcs.SingleOrDefaultAsync(f => f.EcsName == "LeeInfo");
            if(frxEcs.EcsTime.AddMinutes(1)<DateTime.UtcNow)
            {
                list_alert.Add(new AlertViewModel { Info = "The ECS has a mistake", Ico = "fa fa-database text-red", Url = "/" });
                }
            else
            {
                list_alert.Add(new AlertViewModel { Info = "The ECS is ok", Ico = "fa fa-database text-blue", Url = "/" });
            }
            return View(list_alert);
        }
    }
}
