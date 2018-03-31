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
using Microsoft.AspNetCore.Http;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins")]
    public class FrxCbotsetController : Controller
    {
        private readonly AppDbContext _context;

        public FrxCbotsetController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Forex/FrxCbotset
        public async Task<IActionResult> Index()
        {
            return View(await _context.FrxCbotset.ToListAsync());
        }

        // GET: Forex/FrxCbotset/Create
        public IActionResult Create()
        {
            return PartialView("~/Areas/Forex/Views/FrxCbotset/CreateEdit.cshtml");
        }

        // POST: Forex/FrxCbotset/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Symbol,Initvolume,Tmr,Brk,Distance,Istrade,Isbreak,Isbrkfirst,Result,Average,Magnify,Sub,Cr,Ca,Sr,Sa,Signal,Alike")] FrxCbotset frxCbotset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(frxCbotset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("~/Areas/Forex/Views/FrxCbotset/CreateEdit.cshtml",frxCbotset);
        }

        // GET: Forex/FrxCbotset/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var frxCbotset = await _context.FrxCbotset.SingleOrDefaultAsync(m => m.Id == id);
            if (frxCbotset == null)
            {
                return NotFound();
            }
            return PartialView("~/Areas/Forex/Views/FrxCbotset/CreateEdit.cshtml", frxCbotset);
        }

        // POST: Forex/FrxCbotset/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Symbol,Initvolume,Tmr,Brk,Distance,Istrade,Isbreak,Isbrkfirst,Result,Average,Magnify,Sub,Cr,Ca,Sr,Sa,Signal,Alike")] FrxCbotset frxCbotset)
        {
            if (id != frxCbotset.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(frxCbotset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FrxCbotsetExists(frxCbotset.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return PartialView("~/Areas/Forex/Views/FrxCbotset/CreateEdit.cshtml", frxCbotset);
        }

        // GET: Forex/FrxCbotset/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var frxCbotset = await _context.FrxCbotset
                .SingleOrDefaultAsync(m => m.Id == id);
            if (frxCbotset == null)
            {
                return NotFound();
            }

            //return View(frxCbotset);
            return PartialView("~/Areas/Forex/Views/FrxCbotset/Delete.cshtml", frxCbotset.Symbol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,IFormCollection form)
        {
            var frxCbotset = await _context.FrxCbotset.SingleOrDefaultAsync(m => m.Id == id);
            _context.FrxCbotset.Remove(frxCbotset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FrxCbotsetExists(int id)
        {
            return _context.FrxCbotset.Any(e => e.Id == id);
        }
    }
}
