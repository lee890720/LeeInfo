using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeeInfo.Data;
using LeeInfo.Data.Forex;
using Microsoft.AspNetCore.Identity;
using LeeInfo.Data.AppIdentity;
using Connect_API.Accounts;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    public class FrxSymbolController : Controller
    {
        private readonly AppIdentityDbContext _identitycontext;
        private UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        public FrxSymbolController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            #region Parameters
            string _accessToken = "";
            string _apiUrl = "https://api.spotware.com/";
            AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            AppIdentityUser _admin = await _userManager.FindByNameAsync("lee890720");
            if (_user.ConnectAPI)
                _accessToken = _user.AccessToken;
            else
                _accessToken = _admin.AccessToken;
            #endregion
            #region GetAccount
            var useraccounts = _identitycontext.AspNetUserForexAccount.Where(u => u.AppIdentityUserId == _user.Id).ToList();
            var frxaccounts = _context.FrxAccount.Where(x => useraccounts.SingleOrDefault(s => s.AccountNumber == x.AccountNumber && s.Password == x.Password) != null).ToList();
            if (frxaccounts.Count == 0)
                return Redirect("/");
            var frxaccount = new FrxAccount();
            var TAC = new AspNetUserForexAccount();
            var tempAC = useraccounts.SingleOrDefault(x => x.Alive == true);
            if (tempAC == null)
                TAC = useraccounts[0];
            else
                TAC = tempAC;
            frxaccount = frxaccounts.SingleOrDefault(x => x.AccountNumber == TAC.AccountNumber);
            #endregion
            #region GetSymbols
            var tempsymbols = _context.FrxSymbol;
            _context.RemoveRange(tempsymbols);
            await _context.SaveChangesAsync();
            var symbols = Symbols.GetSymbols(_apiUrl, frxaccount.AccountId.ToString(), _accessToken);
            foreach (var s in symbols)
            {
                var symbol = new FrxSymbol();
                symbol.SymbolId = s.SymbolId;
                symbol.SymbolName = s.SymbolName;
                symbol.Digits = s.Digits;
                symbol.PipPosition = s.PipPosition;
                symbol.MeasurementUnits = s.MeasurementUnits;
                symbol.BaseAsset = s.BaseAsset;
                symbol.QuoteAsset = s.QuoteAsset;
                symbol.TradeEnabled = s.TradeEnabled;
                symbol.TickSize = s.TickSize;
                symbol.Description = s.Description;
                symbol.MaxLeverage = s.MaxLeverage;
                symbol.SwapLong = s.SwapLong;
                symbol.SwapShort = s.SwapShort;
                symbol.ThreeDaysSwaps = s.ThreeDaysSwaps;
                symbol.MinOrderVolume = s.MinOrderVolume;
                symbol.MinOrderStep = s.MinOrderStep;
                symbol.MaxOrderVolume = s.MaxOrderVolume;
                symbol.AssetClass = s.AssetClass;
                symbol.LastBid = s.LastBid;
                symbol.LastAsk = s.LastAsk;
                symbol.TradingMode = s.TradingMode;
                _context.Add(symbol);
                await _context.SaveChangesAsync();
            }
            #endregion
            string[] bases = { "XAU", "XAG", "XBR", "XTI" };
            var result = _context.FrxSymbol.Where(x => (x.AssetClass == 1 || bases.Contains(x.BaseAsset))&&x.TradeEnabled);
            return View(await result.ToListAsync());
        }
    }
}
