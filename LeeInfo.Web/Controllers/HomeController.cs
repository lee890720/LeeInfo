using LeeInfo.Data;
using LeeInfo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LeeInfo.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (User.IsInRole("Forex"))
                return Redirect("/Forex/FrxUserAccount");
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult SendContact([FromBody] AppContact _contact)
        {
            var contact = _contact;
            contact.UserName = User.Identity.Name;
            _context.Add(contact);
            _context.SaveChanges();
            return Json(new { contact});
        }
    }
}
