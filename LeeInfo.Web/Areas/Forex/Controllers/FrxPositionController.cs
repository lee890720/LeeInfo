using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Connect_API.Accounts;
using Connect_API.Trading;
using System.Security.Cryptography.X509Certificates;
using LeeInfo.Data;
using LeeInfo.Data.AppIdentity;
using Microsoft.AspNetCore.Identity;
using ChartJSCore.Models;
using ChartJSCore.Helpers;
using ChartJSCore.Plugins;
using LeeInfo.Lib;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeeInfo.Data.Forex;
using System.Drawing;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> Index(int? acId)
        {
            #region Parameters
            string _accessToken = "";
            string _apiUrl = "https://api.spotware.com/";
            AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            _accessToken = _user.AccessToken;
            #endregion
            #region GetAccount
            var useraccounts = _identitycontext.AspNetUserForexAccount.Where(u => u.AppIdentityUserId == _user.Id).ToList();
            //var useraccounts = _user.AspNetUserForexAccount;
            var temp = _context.FrxAccount.Where(x => useraccounts.SingleOrDefault(s => s.AccountNumber == x.AccountNumber && s.Password == x.Password) != null).ToList();
            if (temp.Count == 0)
                return Redirect("/");
            var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
            foreach (var a in accounts)
            {
                var temp_ac = temp.SingleOrDefault(x => x.AccountNumber == a.AccountNumber);
                if (temp_ac != null)
                {
                    temp_ac.Balance = a.Balance / 100;
                    temp_ac.BrokerName = a.BrokerTitle;
                    temp_ac.Currency = a.DepositCurrency;
                    temp_ac.IsLive = a.Live;
                    temp_ac.PreciseLeverage = a.Leverage;
                    temp_ac.TraderRegistrationTime = ConvertJson.StampToDateTime(a.TraderRegistrationTimestamp);
                    _context.Update(temp_ac);
                    await _context.SaveChangesAsync();
                }
            }
            var frxaccounts = _context.FrxAccount.Where(x => useraccounts.SingleOrDefault(s => s.AccountNumber == x.AccountNumber && s.Password == x.Password) != null).ToList();
            var frxaccount = new FrxAccount();
            if (acId == null)
            {
                var tempuserac = useraccounts.SingleOrDefault(x => x.Alive == true);
                if (tempuserac == null)
                    tempuserac = useraccounts[0];
                frxaccount = frxaccounts.SingleOrDefault(x => x.AccountNumber == tempuserac.AccountNumber && x.Password == tempuserac.Password);
            }
            else
                frxaccount = frxaccounts.SingleOrDefault(x => x.AccountId == acId);
            #endregion
            #region GetPosition
            var temppositions = _context.FrxPosition.Where(x => x.AccountId == frxaccount.AccountId);
            _context.RemoveRange(temppositions);
            await _context.SaveChangesAsync();
            var position = Position.GetPositions(_apiUrl, frxaccount.AccountId.ToString(), _accessToken);
            foreach (var p in position)
            {
                FrxPosition fp = new FrxPosition();
                fp.Id = p.PositionID;
                fp.AccountId = frxaccount.AccountId;
                fp.Channel = p.Channel;
                fp.Comment = p.Comment;
                fp.Commissions = p.Commission * 2 / 100;
                fp.CurrentPrice = p.CurrentPrice;
                fp.EntryPrice = p.EntryPrice;
                fp.EntryTime = ConvertJson.StampToDateTime(p.EntryTimestamp);
                fp.GrossProfit = p.Profit / 100;
                fp.Label = p.Label;
                fp.MarginRate = p.MarginRate;
                fp.Swap = p.Swap / 100;
                fp.NetProfit = fp.GrossProfit + fp.Swap + fp.Commissions;
                fp.Pips = p.ProfitInPips;
                fp.Volume = p.Volume / 100;

                var tempvolume = Convert.ToDouble(fp.Volume);
                double tempsub = 100000;
                if (fp.SymbolCode == "XBRUSD" || fp.SymbolCode == "XTIUSD")
                    tempsub = 100;
                if (fp.SymbolCode == "XAGUSD" || fp.SymbolCode == "XAGEUR")
                    tempsub = 1000;
                if (fp.SymbolCode == "XAUUSD" || fp.SymbolCode == "XAUEUR")
                    tempsub = 100;
                fp.Quantity = tempvolume / tempsub;
                fp.StopLoss = p.StopLoss;
                fp.TakeProfit = p.TakeProfit;
                fp.SymbolCode = p.SymbolName;
                fp.TradeType = p.TradeSide == "BUY" ? TradeType.Buy : TradeType.Sell;
                fp.Margin = fp.MarginRate * fp.Volume / frxaccount.PreciseLeverage;
                _context.Add(fp);
                await _context.SaveChangesAsync();
            }
            var frxpositions = _context.FrxPosition.Where(x => x.AccountId == frxaccount.AccountId);
            #endregion
            #region UpdateAccount
            double unrnet = 0;
            double marginused = 0;
            foreach (var p in frxpositions)
            {
                unrnet += p.NetProfit;
                marginused += p.Margin;
            }
            frxaccount.Equity = frxaccount.Balance + unrnet;
            frxaccount.UnrealizedNetProfit = unrnet;
            frxaccount.MarginUsed = Math.Round(marginused, 2);
            frxaccount.FreeMargin = Math.Round(frxaccount.Equity - marginused, 2);
            frxaccount.MarginLevel = Math.Round(frxaccount.Equity / marginused * 100, 2);
            _context.Update(frxaccount);
            await _context.SaveChangesAsync();
            #endregion
            #region GetPosGroup
            List<PosGroup> poss = new List<PosGroup>();
            foreach (var p in frxpositions)
            {
                var temp_poss = poss.Where(x => x.SymbolCode == p.SymbolCode && x.TradeType == p.TradeType);
                if (temp_poss.Count() == 0)
                {
                    var pos = new PosGroup();
                    pos.SymbolCode = p.SymbolCode;
                    pos.TradeType = p.TradeType;
                    pos.Quantity = p.Quantity;
                    pos.EntryPrice = p.EntryPrice;
                    pos.Swap = p.Swap;
                    pos.NetProfit = p.NetProfit;
                    pos.Pips = p.Pips;
                    pos.Gain = p.NetProfit / frxaccount.Balance;
                    poss.Add(pos);
                }
                else
                {
                    var pos = poss.First(x => x.SymbolCode == p.SymbolCode && x.TradeType == p.TradeType);
                    pos.EntryPrice = (pos.EntryPrice * pos.Quantity + p.EntryPrice * p.Quantity) / (pos.Quantity + p.Quantity);
                    pos.Pips = (pos.Pips * pos.Quantity + p.Pips * p.Quantity) / (pos.Quantity + p.Quantity);
                    pos.Quantity += p.Quantity;
                    pos.Swap += p.Swap;
                    pos.NetProfit += p.NetProfit;
                    pos.Gain = pos.NetProfit / frxaccount.Balance;
                }
            }
            #endregion

            return View(Tuple.Create<FrxAccount, List<FrxAccount > ,List<PosGroup>> (frxaccount, frxaccounts.ToList(), poss));
        }

        public JsonResult GetPosition(int? acId)
        {
            var data = _context.FrxPosition.Where(x => x.AccountId == acId).ToList();
            return Json(new { data, data.Count });
        }

        public JsonResult GetPosGroup(int? acId)
        {
            var frxpositions = _context.FrxPosition.Where(x => x.AccountId == acId).ToList();
            List<PosGroup> poss = new List<PosGroup>();
            foreach (var p in frxpositions)
            {
                var temp_poss = poss.Where(x => x.SymbolCode == p.SymbolCode && x.TradeType == p.TradeType);
                if (temp_poss.Count() == 0)
                {
                    var pos = new PosGroup();
                    pos.SymbolCode = p.SymbolCode;
                    pos.TradeType = p.TradeType;
                    pos.Quantity = p.Quantity;
                    pos.EntryPrice = p.EntryPrice;
                    pos.Swap = p.Swap;
                    pos.NetProfit = p.NetProfit;
                    pos.Pips = p.Pips;
                    pos.Gain = 0;
                    poss.Add(pos);
                }
                else
                {
                    var pos = poss.First(x => x.SymbolCode == p.SymbolCode && x.TradeType == p.TradeType);
                    pos.EntryPrice = (pos.EntryPrice * pos.Quantity + p.EntryPrice * p.Quantity) / (pos.Quantity + p.Quantity);
                    pos.Pips = (pos.Pips * pos.Quantity + p.Pips * p.Quantity) / (pos.Quantity + p.Quantity);
                    pos.Quantity += p.Quantity;
                    pos.Swap += p.Swap;
                    pos.NetProfit += p.NetProfit;
                    pos.Gain = 0;
                }
            }
            var data = poss;
            return Json(new { data, data.Count });
        }
    }

    public class PosGroup
    {
        public string SymbolCode { get; set; }
        public TradeType TradeType { get; set; }
        public double Quantity { get; set; }
        public double EntryPrice { get; set; }
        public double Swap { get; set; }
        public double NetProfit { get; set; }
        public double Pips { get; set; }
        public double Gain { get; set; }
    }
}
