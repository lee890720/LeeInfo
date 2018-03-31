using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeeInfo.Data;
using LeeInfo.Data.CreditCard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace LeeInfo.Web.Areas.CreditCard.Controllers
{
    [Area("CreditCard")]
    [Authorize(Roles = "Admins")]
    public class CcdPosController : Controller
    {
        private readonly AppDbContext _context;

        public CcdPosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CreditCard/CcdPos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CcdPos.Include(c => c.CcdPerson);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CreditCard/CcdPos/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName");
           return PartialView("~/Areas/CreditCard/Views/CcdPos/CreateEdit.cshtml");
        }

        // POST: CreditCard/CcdPos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PosId,PersonId,PosName,PosNote")] CcdPos ccdPos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ccdPos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdPos.PersonId);
           return PartialView("~/Areas/CreditCard/Views/CcdPos/CreateEdit.cshtml",ccdPos);
        }

        // GET: CreditCard/CcdPos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdPos = await _context.CcdPos.SingleOrDefaultAsync(m => m.PosId == id);
            if (ccdPos == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdPos.PersonId);
                       return PartialView("~/Areas/CreditCard/Views/CcdPos/CreateEdit.cshtml",ccdPos);
        }

        // POST: CreditCard/CcdPos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PosId,PersonId,PosName,PosNote")] CcdPos ccdPos)
        {
            if (id != ccdPos.PosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ccdPos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CcdPosExists(ccdPos.PosId))
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
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdPos.PersonId);
                   return PartialView("~/Areas/CreditCard/Views/CcdPos/CreateEdit.cshtml",ccdPos);
        }

        // GET: CreditCard/CcdPos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdPos = await _context.CcdPos
                .Include(c => c.CcdPerson)
                .SingleOrDefaultAsync(m => m.PosId == id);
            if (ccdPos == null)
            {
                return NotFound();
            }

            return PartialView("~/Areas/CreditCard/Views/CcdPos/Delete.cshtml",ccdPos.PosName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,IFormCollection form)
        {
            var ccdPos = await _context.CcdPos.SingleOrDefaultAsync(m => m.PosId == id);
            _context.CcdPos.Remove(ccdPos);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CcdPosExists(int id)
        {
            return _context.CcdPos.Any(e => e.PosId == id);
        }
    }
}
