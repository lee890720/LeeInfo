using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeeInfo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using ChartJSCore.Models;
using LeeInfo.Data;
using LeeInfo.Data.AppIdentity;

namespace LeeInfo.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppIdentityDbContext _context;
        public HomeController(AppIdentityDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (User.IsInRole("Forex"))
                return Redirect("/Forex/FrxSymbol");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
