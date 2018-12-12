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
using Connect_API.Accounts;
using LeeInfo.Lib;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class FrxUserAccountController : Controller
    {
        private readonly AppIdentityDbContext _identitycontext;
        private UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        public FrxUserAccountController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }

        // GET: Forex/FrxUserAccount
        public async Task<IActionResult> Index()
        {
            AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User.Identity.Name == "lee890720" || _user.ConnectAPI)
            {
                string _accessToken = "";
                string _apiUrl = "";
                _accessToken = _user.AccessToken;
                _apiUrl = _user.ApiUrl;
                #region GetAccount
                var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
                foreach (var a in accounts)
                {
                    var result = _context.FrxAccount.SingleOrDefault(x => x.AccountId == a.AccountId);
                    if (result == null)
                    {
                        var temp = new FrxAccount();
                        temp.AccountId = a.AccountId;
                        temp.AccountNumber = a.AccountNumber;
                        temp.BrokerName = a.BrokerTitle;
                        temp.Currency = a.DepositCurrency;
                        temp.TraderRegistrationTimestamp =a.TraderRegistrationTimestamp;
                        temp.IsLive = a.Live;
                        temp.Balance = a.Balance;
                        temp.PreciseLeverage = a.Leverage;
                        temp.ClientId = _user.ClientId;
                        temp.ClientSecret = _user.ClientSecret;
                        temp.AccessToken = _user.AccessToken;
                        temp.RefreshToken = _user.RefreshToken;
                        temp.ConnectUrl = _user.ConnectUrl;
                        temp.ApiUrl = _user.ApiUrl;
                        temp.ApiHost = _user.ApiHost;
                        temp.ApiPort = _user.ApiPort;
                        _context.Add(temp);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_user.ClientId) && !string.IsNullOrEmpty(_user.ClientSecret))
                            if (!string.IsNullOrEmpty(_user.AccessToken) && !string.IsNullOrEmpty(_user.RefreshToken))
                                if (!string.IsNullOrEmpty(_user.ConnectUrl) && !string.IsNullOrEmpty(_user.ApiUrl))
                                    if (!string.IsNullOrEmpty(_user.ApiHost) && _user.ApiPort != 0)
                                    {
                                        result.ClientId = _user.ClientId;
                                        result.ClientSecret = _user.ClientSecret;
                                        result.AccessToken = _user.AccessToken;
                                        result.RefreshToken = _user.RefreshToken;
                                        result.ConnectUrl = _user.ConnectUrl;
                                        result.ApiUrl = _user.ApiUrl;
                                        result.ApiHost = _user.ApiHost;
                                        result.ApiPort = _user.ApiPort;
                                        _context.Update(result);
                                        await _context.SaveChangesAsync();
                                    }
                    }
                }
                #endregion
            }

            return View(await _identitycontext.AspNetUserForexAccount.Where(x => x.AppIdentityUserId == _user.Id).ToListAsync());
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
                _identitycontext.Add(frxUserAccount);
                await _identitycontext.SaveChangesAsync();
                if (frxUserAccount.Alive == true)
                {
                    var temp = _identitycontext.AspNetUserForexAccount.Where(x => x.AppIdentityUserId == _user.Id && x.AccountNumber != frxUserAccount.AccountNumber).ToList();
                    foreach (var t in temp)
                    {
                        t.Alive = false;
                    }
                    _identitycontext.UpdateRange(temp);
                    await _identitycontext.SaveChangesAsync();
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

            var frxUserAccount = await _identitycontext.AspNetUserForexAccount.SingleOrDefaultAsync(m => m.Id == id);
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
                    _identitycontext.Update(frxUserAccount);
                    await _identitycontext.SaveChangesAsync();
                    if (frxUserAccount.Alive == true)
                    {
                        var temp = _identitycontext.AspNetUserForexAccount.Where(x => x.AppIdentityUserId == _user.Id && x.AccountNumber != frxUserAccount.AccountNumber).ToList();
                        foreach (var t in temp)
                        {
                            t.Alive = false;
                        }
                        _identitycontext.UpdateRange(temp);
                        await _identitycontext.SaveChangesAsync();
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

            var frxUserAccount = await _identitycontext.AspNetUserForexAccount.SingleOrDefaultAsync(m => m.Id == id);
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
            var frxUserAccount = await _identitycontext.AspNetUserForexAccount.SingleOrDefaultAsync(m => m.Id == id);
            _identitycontext.AspNetUserForexAccount.Remove(frxUserAccount);
            await _identitycontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FrxUserAccountExists(int id)
        {
            return _identitycontext.AspNetUserForexAccount.Any(e => e.Id == id);
        }
    }
}
