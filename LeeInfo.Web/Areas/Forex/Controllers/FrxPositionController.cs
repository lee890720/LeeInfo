using Connect_API.Accounts;
using LeeInfo.Data;
using LeeInfo.Data.AppIdentity;
using LeeInfo.Data.Forex;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class FrxPositionController : Controller
    {
        private readonly AppIdentityDbContext _identitycontext;
        private UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        private string _accessToken = "CMU_aak6k2uQctZ2UOTZdxGBqA-eeOOtf8rfOpfpOV4";
        private string _apiUrl = "https://api.spotware.com/";
        AppIdentityUser _user = new AppIdentityUser();
        AppIdentityUser _admin = new AppIdentityUser();

        public FrxPositionController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }
        public async Task<IActionResult> Index(int? acId)
        {
            #region Parameters
             _user= await _userManager.FindByNameAsync(User.Identity.Name);
             _admin= await _userManager.FindByNameAsync("lee890720");
            var symbols = _context.FrxSymbol;
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
            if (acId == null)
            {
                var tempac1 = new AspNetUserForexAccount();
                var tempac2 = useraccounts.SingleOrDefault(x => x.Alive == true);
                if (tempac2 == null)
                    tempac1 = useraccounts[0];
                else
                    tempac1 = tempac2;
                frxaccount = frxaccounts.SingleOrDefault(x => x.AccountNumber == tempac1.AccountNumber);
            }
            else
                frxaccount = frxaccounts.SingleOrDefault(x => x.AccountId == acId);
            var account = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken).SingleOrDefault(x => x.AccountId == frxaccount.AccountId);
            if (account != null)
            {
                frxaccount.Balance = account.Balance / 100;
                _context.Update(frxaccount);
                await _context.SaveChangesAsync();
            }
            else
                return Redirect("/");
            #endregion                      

            return View(Tuple.Create<FrxAccount, List<FrxAccount>>(frxaccount, frxaccounts));
        }

        public JsonResult GetPosition([FromBody]Params param)
        {
            var client = new RestClient(_apiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + param.AcId.ToString() + "/positions?oauth_token=" + _accessToken);
            var responsePosition = client.Execute<Position>(request);
            return Json(JObject.Parse(responsePosition.Content));
        }

        public JsonResult GetPosGroup([FromBody]Params param)
        {
            #region Parameters
            _user =  _identitycontext.Users.SingleOrDefault(x => x.UserName == User.Identity.Name);
            _admin = _identitycontext.Users.SingleOrDefault(x => x.UserName == "lee890720");
            if (_user.ConnectAPI)
                _accessToken = _user.AccessToken;
            else
                _accessToken = _admin.AccessToken;

            var positions = Position.GetPositions(_apiUrl, param.AcId.ToString(), _accessToken);
            var account = _context.FrxAccount.SingleOrDefault(x => x.AccountId == param.AcId);
            string[] bases = { "XAU", "XAG", "XBR", "XTI" };
            var symbols = _context.FrxSymbol.Where(x => (x.AssetClass == 1 || bases.Contains(x.BaseAsset)) && x.TradeEnabled).OrderBy(x => x.SymbolId).ToList();
            #endregion

            #region GetPosGroup
            List<PosGroup> data = new List<PosGroup>();
            data = positions.GroupBy(g => new { g.SymbolId, g.SymbolName, g.TradeSide })
                .Select(s => new PosGroup
                {
                    SymbolId = s.Key.SymbolId,
                    SymbolName = s.Key.SymbolName,
                    TradeSide = s.Key.TradeSide,
                    Volume = s.Sum(a => a.Volume/100),
                    Lot = 0.00,
                    EntryPrice = s.Sum(a => a.EntryPrice * a.Volume) / s.Sum(b => b.Volume),
                    Swap = s.Sum(a => a.Swap/100),
                    Profit = s.Sum(a => (a.Profit+a.Swap+a.Commission*2)/100),
                    Pips = s.Sum(a => (double)a.ProfitInPips * a.Volume) / s.Sum(b => b.Volume),
                    Gain = s.Sum(a => (double)a.Profit) / account.Balance,
                    PipPosition = symbols.SingleOrDefault(a => a.SymbolId == s.Key.SymbolId).PipPosition,
                    AssetClass=symbols.SingleOrDefault(a=>a.SymbolId==s.Key.SymbolId).AssetClass,
                    MinOrderVolume = symbols.SingleOrDefault(a => a.SymbolId == s.Key.SymbolId).MinOrderVolume,
                }).OrderBy(o => o.Profit).ToList();
            #endregion

            return Json(new { data, data.Count });
        }

        public JsonResult GetSymbol()
        {
            string[] bases = { "XAU", "XAG", "XBR", "XTI" };
            var data = _context.FrxSymbol.Where(x => (x.AssetClass == 1 || bases.Contains(x.BaseAsset)) && x.TradeEnabled).OrderBy(x => x.SymbolId).ToList();
            return Json(new { data, data.Count });
        }
    }

    public class PosGroup
    {
        public int SymbolId { get; set; }
        public string SymbolName { get; set; }
        public string TradeSide { get; set; }
        public double EntryPrice { get; set; }
        public long Volume { get; set; }
        public double? Lot { get; set; }
        public double Swap { get; set; }
        public double Pips { get; set; }
        public double? Profit { get; set; }
        public double Gain { get; set; }
        public int PipPosition { get; set; }
        public int AssetClass { get; set; }
        public long MinOrderVolume { get; set; }
    }
}
