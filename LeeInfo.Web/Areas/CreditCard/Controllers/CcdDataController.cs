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
using LeeInfo.Web.Areas.CreditCard.Models;

namespace LeeInfo.Web.Areas.CreditCard.Controllers
{
    [Area("CreditCard")]
    [Authorize(Roles = "Admins")]
    public class CcdDataController : Controller
    {
        private readonly AppDbContext _context;

        public CcdDataController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CreditCard/CcdData
        public async Task<IActionResult> Index()
        {
            #region Update the bills
            var ccdtemp1 = _context.CcdData.Include(c => c.CcdBill);
            foreach(var c in ccdtemp1)
            {
                bool IsChanged = false;
                foreach(var b in c.CcdBill)
                {
                    if(c.CreditCardId==b.CreditCardId)
                        if(c.AccountBill<b.BillDate&&c.RepaymentDate>b.BillDate)
                        {
                            c.BillAmount = b.BillAmount;
                            _context.Entry(c).State = EntityState.Modified;
                            IsChanged = true;
                        }
                }
                if(!IsChanged)
                {
                    c.BillAmount = 0;
                    _context.Entry(c).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
            #endregion
            #region  Get RecordGroupViewModel
            var ccdtemp2 = _context.CcdRecord.Where(x => (x.CcdData.AccountBill < DateTime.Now
              && x.RecordDate > x.CcdData.AccountBill)
              || (x.CcdData.AccountBill > DateTime.Now
              && x.RecordDate > x.CcdData.AccountBill.AddMonths(-1)))
            .GroupBy(x => new
            {
                x.CreditCardId,
                x.CcdData.CcdPerson.PersonName,
                x.CcdData.IssuingBank,
                x.CcdData.BillAmount
            })
            .Select(g => new RecordGroupViewModel
            {
                CreditCardId = g.Key.CreditCardId,
                PersonName = g.Key.PersonName,
                IssuingBank = g.Key.IssuingBank,
                BillAmount = g.Key.BillAmount,
                OutstandingAmount = g.Key.BillAmount - (double)(g.Sum(y => y.Deposit) ?? 0),
                DepositCount = g.Where(y => y.Deposit != 0).Count(),
                DepositSum = (double)(g.Sum(y => y.Deposit) ?? 0),
                ExpendCount = g.Where(y => y.Expend != 0).Count(),
                ExpendSum = (double)(g.Sum(y => y.Expend) ?? 0),
                Total = (double)(g.Sum(y => y.Deposit) ?? 0) - (double)(g.Sum(y => y.Expend) ?? 0),
                PosId = 0
            }).OrderBy(xx => xx.CreditCardId);
            #endregion
            #region Update the CcdData
            foreach (var c in _context.CcdData)
            {
                if (c.BillAmount != 0)
                {
                    bool IsChange = false;
                    foreach (var d in ccdtemp2)
                    {
                        if (c.CreditCardId == d.CreditCardId)
                        {
                            c.HasPayment = d.DepositSum;
                            c.PrePayment = c.BillAmount - c.HasPayment;
                            IsChange = true;
                            break;
                        }
                    }
                    if (!IsChange)
                    {
                        c.HasPayment = 0;
                        c.PrePayment = c.BillAmount - c.HasPayment;
                    }
                }
                else
                {
                    c.HasPayment = 0;
                    c.PrePayment = 0;
                }
                _context.Entry(c).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            #endregion
            var appDbContext = _context.CcdData.Include(c => c.CcdPerson);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Forex/FrxCbotset/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName");
            return PartialView("~/Areas/CreditCard/Views/CcdData/CreateEdit.cshtml");
        }

        // POST: CreditCard/CcdData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreditCardId,PersonId,IssuingBank,CreditCardNumber,Limit,Temporary,TempDate,AccountBill,RepaymentDate,BillAmount,ValidThru,Cvv,TransactionPw,InquriyPw,OnlineBankingPw")] CcdData ccdData)
        {
            if (ModelState.IsValid)
            {
                ccdData.PrePayment = 0;
                ccdData.HasPayment = 0;
                _context.Add(ccdData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdData.PersonId);
            return PartialView("~/Areas/CreditCard/Views/CcdData/CreateEdit.cshtml",ccdData);
        }

        // GET: CreditCard/CcdData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdData = await _context.CcdData.SingleOrDefaultAsync(m => m.CreditCardId == id);
            if (ccdData == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdData.PersonId);
            return PartialView("~/Areas/CreditCard/Views/CcdData/CreateEdit.cshtml",ccdData);
        }

        // POST: CreditCard/CcdData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CreditCardId,PersonId,IssuingBank,CreditCardNumber,Limit,Temporary,TempDate,AccountBill,RepaymentDate,BillAmount,ValidThru,Cvv,TransactionPw,InquriyPw,OnlineBankingPw")] CcdData ccdData)
        {
            if (id != ccdData.CreditCardId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ccdData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CcdDataExists(ccdData.CreditCardId))
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
            ViewData["PersonId"] = new SelectList(_context.CcdPerson, "PersonId", "PersonName", ccdData.PersonId);
            return PartialView("~/Areas/CreditCard/Views/CcdData/CreateEdit.cshtml",ccdData);
        }

        // GET: CreditCard/CcdData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ccdData = await _context.CcdData
                .Include(c => c.CcdPerson)
                .SingleOrDefaultAsync(m => m.CreditCardId == id);
            if (ccdData == null)
            {
                return NotFound();
            }
            var str = ccdData.IssuingBank.ToString() + "-" + ccdData.CcdPerson.PersonName;
            return PartialView("~/Areas/CreditCard/Views/CcdData/Delete.cshtml",str);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,IFormCollection form)
        {
            var ccdData = await _context.CcdData.SingleOrDefaultAsync(m => m.CreditCardId == id);
            _context.CcdData.Remove(ccdData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult AddBill(int? d)
        {
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber");
            return PartialView("~/Areas/CreditCard/Views/CcdData/AddBill.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBill([Bind("CreditCardId,BillDate,BillAmount")] CcdBill ccdBill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ccdBill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber");
            return PartialView("~/Areas/CreditCard/Views/CcdData/AddBill.cshtml",ccdBill);
        }

        private bool CcdDataExists(int id)
        {
            return _context.CcdData.Any(e => e.CreditCardId == id);
        }
    }
}
