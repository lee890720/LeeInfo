using Connect_API.Accounts;
using LeeInfo.Data;
using LeeInfo.Data.AppIdentity;
using LeeInfo.Data.Forex;
using LeeInfo.Web.Areas.Forex.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeInfo.Web.Areas.Forex.Models;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class FrxPositionController : Controller
    {
        private readonly AppIdentityDbContext _identitycontext;
        private UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        public FrxPositionController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }
        public IActionResult Index(int? acId)
        {
            ConnectAPI connect = ConnectAPI.GetConnectAPI(_identitycontext, _context, User.Identity.Name,acId);
            if (connect.AccountId == 0)
                return Redirect("/");
            var useraccounts = _identitycontext.AspNetUserForexAccount.Where(u => u.AppIdentityUserId == connect.UserId).ToList();
            var accounts = _context.FrxAccount.Where(x => useraccounts.SingleOrDefault(s => s.AccountNumber == x.AccountNumber && s.Password == x.Password) != null).ToList();
            return View(Tuple.Create<ConnectAPI, List<FrxAccount>>(connect, accounts));
        }

        public JsonResult GetPosition([FromBody]Params param)
        {
            var client = new RestClient(param.ApiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + param.AccountId.ToString() + "/positions?oauth_token=" + param.AccessToken);
            var responsePosition = client.Execute<Position>(request);
            return Json(JObject.Parse(responsePosition.Content));
        }

        public JsonResult GetPosGroup([FromBody]Params param)
        {
            #region Parameters
            var positions = Position.GetPositions(param.ApiUrl, param.AccountId.ToString(), param.AccessToken);
            var account = _context.FrxAccount.SingleOrDefault(x => x.AccountId == param.AccountId);
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
