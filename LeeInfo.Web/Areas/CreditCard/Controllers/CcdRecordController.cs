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
    public class CcdRecordController : Controller
    {
        private readonly AppDbContext _context;

        public CcdRecordController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CreditCard/CcdRecord
        public async Task<IActionResult> Index()
        {
            #region Update the bills
            var ccdtemp1 = _context.CcdData.Include(c => c.CcdBill);
            foreach (var c in ccdtemp1)
            {
                bool IsChanged = false;
                foreach (var b in c.CcdBill)
                {
                    if (c.CreditCardId == b.CreditCardId)
                        if (c.AccountBill < b.BillDate && c.RepaymentDate > b.BillDate)
                        {
                            c.BillAmount = b.BillAmount;
                            _context.Entry(c).State = EntityState.Modified;
                            IsChanged = true;
                        }
                }
                if (!IsChanged)
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
                x.CcdData.CreditCardNumber,
                x.CcdData.CcdPerson.PersonName,
                x.CcdData.IssuingBank,
                x.CcdData.BillAmount,
                x.CcdData.RepaymentDate
            })
            .Select(g => new RecordGroupViewModel
            {
                CreditCardId = g.Key.CreditCardId,
                CreditCardNumber = g.Key.CreditCardNumber,
                PersonName = g.Key.PersonName,
                IssuingBank = g.Key.IssuingBank,
                BillAmount = g.Key.BillAmount,
                RepaymentDate = g.Key.RepaymentDate,
                OutstandingAmount = g.Key.BillAmount - (double)(g.Sum(y => y.Deposit) ?? 0),
                DepositCount = g.Where(y => y.Deposit != 0).Count(),
                DepositSum = (double)(g.Sum(y => y.Deposit) ?? 0),
                ExpendCount = g.Where(y => y.Expend != 0).Count(),
                ExpendSum = (double)(g.Sum(y => y.Expend) ?? 0),
                Total = (double)(g.Sum(y => y.Deposit) ?? 0) - (double)(g.Sum(y => y.Expend) ?? 0),
                PosId = 0
            }).OrderBy(xx => xx.RepaymentDate).ToList();
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
            #region 
            var ccdtemp3 = await _context.CcdRecord.GroupBy(x => new
            {
                x.CreditCardId,
                x.CcdData.CreditCardNumber,
                x.CcdData.CcdPerson.PersonName,
                x.CcdData.IssuingBank,
                x.CcdData.BillAmount,
                x.CcdData.RepaymentDate,
                x.CcdData.PrePayment,
            }).Select(g => new
            {
                CreditCardId = g.Key.CreditCardId,
                CreditCardNumber = g.Key.CreditCardNumber,
                PersonName = g.Key.PersonName,
                IssuingBank = g.Key.IssuingBank,
                BillAmount = g.Key.BillAmount,
                RepaymentDate = g.Key.RepaymentDate,
                PrePayment = g.Key.PrePayment,
                Total = (double)g.Sum(y => y.Deposit) - (double)g.Sum(y => y.Expend)
            }).OrderBy(xx => xx.CreditCardId).ToListAsync();
            #endregion
            for (int dt = ccdtemp3.Count() - 1; dt >= 0; dt--)
            {
                bool IsAlive = false;
                for (int d = ccdtemp2.Count() - 1; d >= 0; d--)
                {
                    if (ccdtemp3[dt].CreditCardId == ccdtemp2[d].CreditCardId)
                        IsAlive = true;
                }
                if (!IsAlive)
                {
                    RecordGroupViewModel dAdd = new RecordGroupViewModel
                    {
                        CreditCardId = ccdtemp3[dt].CreditCardId,
                        CreditCardNumber = ccdtemp3[dt].CreditCardNumber,
                        PersonName = ccdtemp3[dt].PersonName,
                        IssuingBank = ccdtemp3[dt].IssuingBank,
                        BillAmount = ccdtemp3[dt].BillAmount,
                        RepaymentDate = ccdtemp3[dt].RepaymentDate,
                        OutstandingAmount = ccdtemp3[dt].PrePayment,
                        DepositCount = 0,
                        DepositSum = 0,
                        ExpendCount = 0,
                        ExpendSum = 0,
                        Total = ccdtemp3[dt].Total,
                        PosId = 1,
                    };
                    ccdtemp2.Add(dAdd);
                }
            }
            for (int d = ccdtemp2.Count() - 1; d >= 0; d--)
            {
                #region ccdtemp2 from ccdtemp3
                if (ccdtemp2[d].OutstandingAmount < 0)
                    ccdtemp2[d].OutstandingAmount = 0;
                foreach (var dt in ccdtemp3)
                {
                    if (ccdtemp2[d].CreditCardId == dt.CreditCardId)
                        ccdtemp2[d].Total = dt.Total;
                }
                #endregion
                if (ccdtemp2[d].OutstandingAmount == 0 && ccdtemp2[d].Total == 0)
                    ccdtemp2.Remove(ccdtemp2[d]);
            }
            var appDbContext = ccdtemp2.OrderBy(d => d.RepaymentDate).ToList();
            ViewBag.SumTotal = appDbContext.Sum(d => d.Total);
            ViewBag.SumOut = appDbContext.Where(d => d.OutstandingAmount > 0).Sum(d => d.OutstandingAmount);
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber");
            ViewData["PosId"] = new SelectList(_context.CcdPos, "PosId", "PosName");
            return View(appDbContext);
        }

        public async Task<IActionResult> RecordList()
        {
            var ccdRecord = _context.CcdRecord.Include(c => c.CcdData).Include(c => c.CcdPos).Include(c => c.CcdData.CcdPerson).OrderByDescending(c => c.RecordId).Take(30);
            return View(await ccdRecord.ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordList(List<int> ids)
        {
            if (ids.Count > 0)
            {
                _context.CcdRecord.Where(x => ids.Contains(x.RecordId)).ToList().ForEach(x =>
                {
                    _context.CcdRecord.Remove(x);
                });
                await _context.SaveChangesAsync();
            }
            var ccdRecord = _context.CcdRecord.Include(c => c.CcdData).Include(c => c.CcdPos).Include(c => c.CcdData.CcdPerson).OrderByDescending(c => c.RecordId).Take(30);
            return View(await ccdRecord.ToListAsync());
        }

        // GET: CreditCard/CcdRecord/Create
        public async Task<IActionResult> Create()
        {
            #region ccdtemp1 Update the bills
            var ccdtemp1 = _context.CcdData.Include(c => c.CcdBill);
            foreach (var c in ccdtemp1)
            {
                bool IsChanged = false;
                foreach (var b in c.CcdBill)
                {
                    if (c.CreditCardId == b.CreditCardId)
                        if (c.AccountBill < b.BillDate && c.RepaymentDate > b.BillDate)
                        {
                            c.BillAmount = b.BillAmount;
                            _context.Entry(c).State = EntityState.Modified;
                            IsChanged = true;
                        }
                }
                if (!IsChanged)
                {
                    c.BillAmount = 0;
                    _context.Entry(c).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
            #endregion
            #region ccdtemp2 Get RecordGroupViewModel
            var ccdtemp2 = _context.CcdRecord.Where(x => (x.CcdData.AccountBill < DateTime.Now
              && x.RecordDate > x.CcdData.AccountBill)
              || (x.CcdData.AccountBill > DateTime.Now
              && x.RecordDate > x.CcdData.AccountBill.AddMonths(-1)))
            .GroupBy(x => new
            {
                x.CreditCardId,
                x.CcdData.CreditCardNumber,
                x.CcdData.CcdPerson.PersonName,
                x.CcdData.IssuingBank,
                x.CcdData.BillAmount,
                x.CcdData.RepaymentDate
            })
            .Select(g => new RecordGroupViewModel
            {
                CreditCardId = g.Key.CreditCardId,
                CreditCardNumber=g.Key.CreditCardNumber,
                PersonName = g.Key.PersonName,
                IssuingBank = g.Key.IssuingBank,
                BillAmount = g.Key.BillAmount,
                RepaymentDate = g.Key.RepaymentDate,
                OutstandingAmount = g.Key.BillAmount - (double)(g.Sum(y => y.Deposit) ?? 0),
                DepositCount = g.Where(y => y.Deposit != 0).Count(),
                DepositSum = (double)(g.Sum(y => y.Deposit) ?? 0),
                ExpendCount = g.Where(y => y.Expend != 0).Count(),
                ExpendSum = (double)(g.Sum(y => y.Expend) ?? 0),
                Total = (double)(g.Sum(y => y.Deposit) ?? 0) - (double)(g.Sum(y => y.Expend) ?? 0),
                PosId = 0
            }).OrderBy(xx => xx.RepaymentDate).ToList();
            #endregion
            #region  Update the CcdData
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
            #region ccdtemp3
            var ccdtemp3 = await _context.CcdRecord.GroupBy(x => new
            {
                x.CreditCardId,
                x.CcdData.CreditCardNumber,
                x.CcdData.CcdPerson.PersonName,
                x.CcdData.IssuingBank,
                x.CcdData.BillAmount,
                x.CcdData.RepaymentDate,
                x.CcdData.PrePayment,
            }).Select(g => new
            {
                CreditCardId = g.Key.CreditCardId,
                CreditCardNumber=g.Key.CreditCardNumber,
                PersonName = g.Key.PersonName,
                IssuingBank = g.Key.IssuingBank,
                BillAmount = g.Key.BillAmount,
                RepaymentDate = g.Key.RepaymentDate,
                PrePayment = g.Key.PrePayment,
                Total = (double)g.Sum(y => y.Deposit) - (double)g.Sum(y => y.Expend)
            }).OrderBy(xx => xx.CreditCardId).ToListAsync();
            #endregion
            #region ccdtemp4 
            var ccdtemp4 = _context.CcdData.Select(x => new
            {
                CreditCardId = x.CreditCardId,
                CreditCardNumber = x.CreditCardNumber,
                IdNum = x.CreditCardId + "-" + x.CreditCardNumber
            }).OrderBy(o => o.IdNum).ToList();
            #endregion
            for (int dt = ccdtemp3.Count() - 1; dt >= 0; dt--)
            {
                bool IsAlive = false;
                for (int d = ccdtemp2.Count() - 1; d >= 0; d--)
                {
                    if (ccdtemp3[dt].CreditCardId == ccdtemp2[d].CreditCardId)
                        IsAlive = true;
                }
                if (!IsAlive)
                {
                    RecordGroupViewModel dAdd = new RecordGroupViewModel
                    {
                        CreditCardId = ccdtemp3[dt].CreditCardId,
                        CreditCardNumber = ccdtemp3[dt].CreditCardNumber,
                        PersonName = ccdtemp3[dt].PersonName,
                        IssuingBank = ccdtemp3[dt].IssuingBank,
                        BillAmount = ccdtemp3[dt].BillAmount,
                        RepaymentDate = ccdtemp3[dt].RepaymentDate,
                        OutstandingAmount = ccdtemp3[dt].PrePayment,
                        DepositCount = 0,
                        DepositSum = 0,
                        ExpendCount = 0,
                        ExpendSum = 0,
                        Total = ccdtemp3[dt].Total,
                        PosId = 1,
                    };
                    ccdtemp2.Add(dAdd);
                }
            }
            for (int d = ccdtemp2.Count() - 1; d >= 0; d--)
            {
                #region ccdtemp2 from ccdtemp3
                if (ccdtemp2[d].OutstandingAmount < 0)
                    ccdtemp2[d].OutstandingAmount = 0;
                foreach (var dt in ccdtemp3)
                {
                    if (ccdtemp2[d].CreditCardId == dt.CreditCardId)
                        ccdtemp2[d].Total = dt.Total;
                }
                #endregion
                if (ccdtemp2[d].OutstandingAmount == 0 && ccdtemp2[d].Total == 0)
                    ccdtemp2.Remove(ccdtemp2[d]);
            }
            var appDbContext = ccdtemp2.OrderBy(d => d.RepaymentDate).ToList();
            ViewBag.SumTotal = appDbContext.Sum(d => d.Total);
            ViewBag.SumOut = appDbContext.Where(d => d.OutstandingAmount > 0).Sum(d => d.OutstandingAmount);
            ViewData["CreditCardId"] = new SelectList(ccdtemp4, "CreditCardId", "IdNum");
            ViewData["PosId"] = new SelectList(_context.CcdPos, "PosId", "PosName");
            return View(appDbContext);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecordId,CreditCardId,PosId,Deposit,Expend")] CcdRecord ccdRecord)
        {
            if (ModelState.IsValid)
            {
                if (ccdRecord.Deposit == null)
                    ccdRecord.Deposit = 0;
                if (ccdRecord.Expend == null)
                    ccdRecord.Expend = 0;
                ccdRecord.RecordDate = DateTime.Now;
                    _context.Add(ccdRecord);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View();
        }

        // GET: CreditCard/CcdRecord/Edit/5
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
            return PartialView("~/Areas/CreditCard/Views/CcdRecord/CreateEdit.cshtml", ccdRecord);
        }

        // POST: CreditCard/CcdRecord/Edit/5
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
                return RedirectToAction(nameof(RecordList));
            }
            ViewData["CreditCardId"] = new SelectList(_context.CcdData, "CreditCardId", "CreditCardNumber", ccdRecord.CreditCardId);
            ViewData["PosId"] = new SelectList(_context.CcdPos, "PosId", "PosName", ccdRecord.PosId);
            return PartialView("~/Areas/CreditCard/Views/CcdRecord/CreateEdit.cshtml", ccdRecord);
        }

        // GET: CreditCard/CcdRecord/Delete/5
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

            return PartialView("~/Areas/CreditCard/Views/CcdRecord/Delete.cshtml", ccdRecord.RecordId.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, IFormCollection form)
        {
            var ccdRecord = await _context.CcdRecord.SingleOrDefaultAsync(m => m.RecordId == id);
            _context.CcdRecord.Remove(ccdRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(RecordList));
        }

        private bool CcdRecordExists(int id)
        {
            return _context.CcdRecord.Any(e => e.RecordId == id);
        }
    }
}
