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

namespace LeeInfo.Web.Areas.CreditCard.Controllers
{
    [Area("CreditCard")]
    [Authorize(Roles = "Admins")]
    public class CcdRecordsController : Controller
    {
        private readonly AppDbContext _context;

        public CcdRecordsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CreditCard/CcdRecords
        public async Task<IActionResult> Index()
        {
            for(var i=0;i<5;i++)
            {
                var ccdr = new CcdRecord();
                ccdr.CreditCardId = 1;
                ccdr.RecordDate = DateTime.Now;
                ccdr.PosId = 1;
                ccdr.Deposit = i + 1;
                ccdr.Expend = 1;
                _context.Add(ccdr);
            }
            await _context.SaveChangesAsync();
            var appDbContext = _context.CcdRecord.Include(c => c.CcdData).Include(c => c.CcdPos);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CreditCard/CcdRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdRecord = await _context.CcdRecord
                .Include(c => c.CcdData)
                .Include(c => c.CcdPos)
                .SingleOrDefaultAsync(m => m.RecordId == id);
            if (ccdRecord == null)
            {
                return NotFound();
            }

            return View(ccdRecord);
        }

        // GET: CreditCard/CcdRecords/Create
        public IActionResult Create()
        {
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber");
            ViewData["PosId"] = new SelectList(_context.CcdPos, "PosId", "PosName");
            return View();
        }

        // POST: CreditCard/CcdRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecordId,CreditCardId,RecordDate,PosId,Deposit,Expend")] CcdRecord ccdRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ccdRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber", ccdRecord.CreditCardId);
            ViewData["PosId"] = new SelectList(_context.CcdPos, "PosId", "PosName", ccdRecord.PosId);
            return View(ccdRecord);
        }

        // GET: CreditCard/CcdRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdRecord = await _context.CcdRecord.SingleOrDefaultAsync(m => m.RecordId == id);
            if (ccdRecord == null)
            {
                return NotFound();
            }
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber", ccdRecord.CreditCardId);
            ViewData["PosId"] = new SelectList(_context.CcdPos, "PosId", "PosName", ccdRecord.PosId);
            return View(ccdRecord);
        }

        // POST: CreditCard/CcdRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordId,CreditCardId,RecordDate,PosId,Deposit,Expend")] CcdRecord ccdRecord)
        {
            if (id != ccdRecord.RecordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ccdRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CcdRecordExists(ccdRecord.RecordId))
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
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber", ccdRecord.CreditCardId);
            ViewData["PosId"] = new SelectList(_context.CcdPos, "PosId", "PosName", ccdRecord.PosId);
            return View(ccdRecord);
        }

        // GET: CreditCard/CcdRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdRecord = await _context.CcdRecord
                .Include(c => c.CcdData)
                .Include(c => c.CcdPos)
                .SingleOrDefaultAsync(m => m.RecordId == id);
            if (ccdRecord == null)
            {
                return NotFound();
            }

            return View(ccdRecord);
        }

        // POST: CreditCard/CcdRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ccdRecord = await _context.CcdRecord.SingleOrDefaultAsync(m => m.RecordId == id);
            _context.CcdRecord.Remove(ccdRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CcdRecordExists(int id)
        {
            return _context.CcdRecord.Any(e => e.RecordId == id);
        }
    }
}
