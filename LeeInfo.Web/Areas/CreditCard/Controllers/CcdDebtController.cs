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
    public class CcdDebtController : Controller
    {
        private readonly AppDbContext _context;

        public CcdDebtController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CreditCard/CcdDebt
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CcdDebt.Include(c => c.CcdPerson);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CreditCard/CcdDebt/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName");
            return PartialView("~/Areas/CreditCard/Views/CcdDebt/CreateEdit.cshtml");
        }

        // POST: CreditCard/CcdDebt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DebtId,PersonId,DebtTitle,RepaymentDate,BillAmount,DebtNote,CurrentAmount,InterestRate")] CcdDebt ccdDebt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ccdDebt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdDebt.PersonId);
       return PartialView("~/Areas/CreditCard/Views/CcdDebt/CreateEdit.cshtml",ccdDebt);
        }

        // GET: CreditCard/CcdDebt/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdDebt = await _context.CcdDebt.SingleOrDefaultAsync(m => m.DebtId == id);
            if (ccdDebt == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdDebt.PersonId);
               return PartialView("~/Areas/CreditCard/Views/CcdDebt/CreateEdit.cshtml",ccdDebt);
        }

        // POST: CreditCard/CcdDebt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DebtId,PersonId,DebtTitle,RepaymentDate,BillAmount,DebtNote,CurrentAmount,InterestRate")] CcdDebt ccdDebt)
        {
            if (id != ccdDebt.DebtId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ccdDebt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CcdDebtExists(ccdDebt.DebtId))
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
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdDebt.PersonId);
             return PartialView("~/Areas/CreditCard/Views/CcdDebt/CreateEdit.cshtml",ccdDebt);
        }

        // GET: CreditCard/CcdDebt/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdDebt = await _context.CcdDebt
                .Include(c => c.CcdPerson)
                .SingleOrDefaultAsync(m => m.DebtId == id);
            if (ccdDebt == null)
            {
                return NotFound();
            }

             return PartialView("~/Areas/CreditCard/Views/CcdDebt/Delete.cshtml",ccdDebt.DebtTitle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,IFormCollection form)
        {
            var ccdDebt = await _context.CcdDebt.SingleOrDefaultAsync(m => m.DebtId == id);
            _context.CcdDebt.Remove(ccdDebt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CcdDebtExists(int id)
        {
            return _context.CcdDebt.Any(e => e.DebtId == id);
        }
    }
}
