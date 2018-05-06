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
using Microsoft.AspNetCore.Identity;
using LeeInfo.Data.AppIdentity;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class FrxUserAccountController : Controller
    {
        private readonly AppIdentityDbContext _context;
        private UserManager<AppIdentityUser> _userManager;

        public FrxUserAccountController(AppIdentityDbContext context, UserManager<AppIdentityUser> userMgr)
        {
            _context = context;
            _userManager = userMgr;
        }

        // GET: Forex/FrxUserAccount
        public async Task<IActionResult> Index()
        {
            AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(await _context.AspNetUserForexAccount.Where(x => x.AppIdentityUserId==_user.Id).ToListAsync());
        }

        // GET: Forex/FrxCbotset/Create
        public IActionResult Create()
        {
            return PartialView("~/Areas/Forex/Views/FrxUserAccount/CreateEdit.cshtml");
        }

        // POST: Forex/FrxUserAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountNumber,Password,Alive")] AspNetUserForexAccount frxUserAccount)
        {
            if (ModelState.IsValid)
            {
                AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
                frxUserAccount.AppIdentityUserId = _user.Id;
                _context.Add(frxUserAccount);
                await _context.SaveChangesAsync();
                if(frxUserAccount.Alive==true)
                {
                    var temp = _context.AspNetUserForexAccount.Where(x => x.AppIdentityUserId == _user.Id && x.AccountNumber != frxUserAccount.AccountNumber).ToList();
                    foreach(var t in temp)
                    {
                        t.Alive = false;
                    }
                    _context.UpdateRange(temp);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return PartialView("~/Areas/Forex/Views/FrxUserAccount/CreateEdit.cshtml", frxUserAccount);
        }

        // GET: Forex/FrxUserAccount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var frxUserAccount = await _context.AspNetUserForexAccount.SingleOrDefaultAsync(m => m.Id == id);
            if (frxUserAccount == null)
            {
                return NotFound();
            }
            return PartialView("~/Areas/Forex/Views/FrxUserAccount/CreateEdit.cshtml", frxUserAccount);
        }

        // POST: Forex/FrxUserAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountNumber,Password,Alive")] AspNetUserForexAccount frxUserAccount)
        {
            if (id != frxUserAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
                    frxUserAccount.AppIdentityUserId = _user.Id;
                    _context.Update(frxUserAccount);
                    await _context.SaveChangesAsync();
                    if (frxUserAccount.Alive == true)
                    {
                        var temp = _context.AspNetUserForexAccount.Where(x => x.AppIdentityUserId == _user.Id && x.AccountNumber != frxUserAccount.AccountNumber).ToList();
                        foreach (var t in temp)
                        {
                            t.Alive = false;
                        }
                        _context.UpdateRange(temp);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FrxUserAccountExists(frxUserAccount.Id))
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
            return PartialView("~/Areas/Forex/Views/FrxUserAccount/CreateEdit.cshtml", frxUserAccount);
        }

        // GET: Forex/FrxUserAccount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var frxUserAccount = await _context.AspNetUserForexAccount.SingleOrDefaultAsync(m => m.Id == id);
            if (frxUserAccount == null)
            {
                return NotFound();
            }

            //return View(frxAccount);
            return PartialView("~/Areas/Forex/Views/FrxUserAccount/Delete.cshtml", frxUserAccount.AccountNumber.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, IFormCollection form)
        {
            var frxUserAccount = await _context.AspNetUserForexAccount.SingleOrDefaultAsync(m => m.Id == id);
            _context.AspNetUserForexAccount.Remove(frxUserAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FrxUserAccountExists(int id)
        {
            return _context.AspNetUserForexAccount.Any(e => e.Id == id);
        }
    }
}
