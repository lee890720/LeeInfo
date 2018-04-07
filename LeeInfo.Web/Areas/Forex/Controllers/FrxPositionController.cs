using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeeInfo.Data;
using LeeInfo.Data.Forex;
using Microsoft.AspNetCore.Authorization;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class FrxPositionController : Controller
    {
        private readonly AppDbContext _context;

        public FrxPositionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Forex/FrxPosition
        public async Task<IActionResult> Index()
        {
            return View(await _context.FrxPosition.ToListAsync());
        }

        // GET: Forex/FrxPosition/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var frxPosition = await _context.FrxPosition
                .SingleOrDefaultAsync(m => m.Id == id);
            if (frxPosition == null)
            {
                return NotFound();
            }

            return View(frxPosition);
        }

        private bool FrxPositionExists(int id)
        {
            return _context.FrxPosition.Any(e => e.Id == id);
        }
    }
}
