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
    public class CcdPersonController : Controller
    {
        private readonly AppDbContext _context;

        public CcdPersonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CreditCard/CcdPerson
        public async Task<IActionResult> Index()
        {
            return View(await _context.CcdPerson.ToListAsync());
        }

        // GET: Forex/FrxCbotset/Create
        public IActionResult Create()
        {
            return PartialView("~/Areas/CreditCard/Views/CcdPerson/CreateEdit.cshtml");
        }

        // POST: CreditCard/CcdPerson/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,PersonName,Sex,DateOfBirth,IdcardNumber,Mobile,Email,PersonNote")] CcdPerson ccdPerson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ccdPerson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("~/Areas/CreditCard/Views/CcdPerson/CreateEdit.cshtml",ccdPerson);
        }

        // GET: CreditCard/CcdPerson/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdPerson = await _context.CcdPerson.SingleOrDefaultAsync(m => m.PersonId == id);
            if (ccdPerson == null)
            {
                return NotFound();
            }
            return PartialView("~/Areas/CreditCard/Views/CcdPerson/CreateEdit.cshtml",ccdPerson);
        }

        // POST: CreditCard/CcdPerson/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,PersonName,Sex,DateOfBirth,IdcardNumber,Mobile,Email,PersonNote")] CcdPerson ccdPerson)
        {
            if (id != ccdPerson.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ccdPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CcdPersonExists(ccdPerson.PersonId))
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
            return PartialView("~/Areas/CreditCard/Views/CcdPerson/CreateEdit.cshtml",ccdPerson);
        }

        // GET: CreditCard/CcdPerson/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdPerson = await _context.CcdPerson
                .SingleOrDefaultAsync(m => m.PersonId == id);
            if (ccdPerson == null)
            {
                return NotFound();
            }

            return PartialView("~/Areas/CreditCard/Views/CcdPerson/Delete.cshtml",ccdPerson.PersonName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,IFormCollection form)
        {
            var ccdPerson = await _context.CcdPerson.SingleOrDefaultAsync(m => m.PersonId == id);
            _context.CcdPerson.Remove(ccdPerson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CcdPersonExists(int id)
        {
            return _context.CcdPerson.Any(e => e.PersonId == id);
        }
    }
}
