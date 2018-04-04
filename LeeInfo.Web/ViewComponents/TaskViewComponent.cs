using LeeInfo.Data;
using LeeInfo.Web.Areas.CreditCard.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeInfo.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LeeInfo.Web.ViewComponents
{
    public class TaskViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;
        public TaskViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            #region ccdtemp1 Update the bills
            var ccdtemp1 = _context.CcdData.Include(c => c.CcdBill);
            foreach (var c in ccdtemp1)
            {
                double bill = 0;
                foreach (var b in c.CcdBill)
                {
                    if (c.AccountBill < b.BillDate && c.RepaymentDate > b.BillDate)
                    {
                        bill += b.BillAmount;
                    }
                }
                c.BillAmount = bill;
                _context.Entry(c).State = EntityState.Modified;
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
                CreditCardNumber = g.Key.CreditCardNumber,
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
            return View(appDbContext);
        }
    }
}
