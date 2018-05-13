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
    public class FrxAnalyzeController : Controller
    {
        private readonly AppIdentityDbContext _identitycontext;
        private UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        private string _accessToken = "CMU_aak6k2uQctZ2UOTZdxGBqA-eeOOtf8rfOpfpOV4";
        private string _apiUrl = "https://api.spotware.com/";
        AppIdentityUser _user = new AppIdentityUser();
        AppIdentityUser _admin = new AppIdentityUser();

        public FrxAnalyzeController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
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

            #region GetCashflow
            var cashflow = CashflowHistory.GetCashflowHistory(_apiUrl, frxaccount.AccountId.ToString(), _accessToken);
            foreach (var c in cashflow)
            {
                var fc = new FrxCashflow();
                fc.Id = Convert.ToInt32(c.CashflowID);
                fc.AccountId = frxaccount.AccountId;
                fc.Balance = c.Balance;
                fc.BalanceVersion = c.BalanceVersion;
                fc.ChangeTime = ConvertJson.StampToDateTime(c.ChangeTimestamp);
                fc.Type = c.Type;
                fc.Delta = c.Delta / 100;
                fc.Equity = c.Equity;
                var tempfc = _context.FrxCashflow.SingleOrDefault(x => x.Id == fc.Id);
                if (tempfc == null)
                {
                    _context.FrxCashflow.Add(fc);
                    await _context.SaveChangesAsync();
                }
            }
            #endregion

            #region GetHistory
            DateTime fromtime;
            if (_context.FrxHistory.Count() != 0)
                fromtime = _context.FrxHistory.OrderByDescending(x => x.ClosingTime).ToList()[0].ClosingTime.AddDays(-1) ;
            else
                fromtime = frxaccount.TraderRegistrationTime;
            DateTime utcnow = DateTime.UtcNow;
            string fromtimestamp = ConvertJson.DateTimeToStamp(fromtime);
            string totimestamp = ConvertJson.DateTimeToStamp(utcnow.AddDays(1));
            var deal = Deal.GetDeals(_apiUrl, frxaccount.AccountId.ToString(), _accessToken, fromtimestamp, totimestamp);
            var deal_history = new List<Deal>();
            foreach (var d in deal)
            {
                if (d.PositionCloseDetails != null)
                    deal_history.Add(d);
            }
            foreach (var h in deal_history)
            {
                FrxHistory fh = new FrxHistory();
                fh.ClosingDealId = h.DealID;
                fh.AccountId = frxaccount.AccountId;
                fh.Balance = h.PositionCloseDetails.Balance / 100;
                fh.BalanceVersion = h.PositionCloseDetails.BalanceVersion;
                fh.BaseToUSDConversionRate = h.BaseToUSDConversionRate;
                fh.ClosedToDepoitConversionRate = h.PositionCloseDetails.ClosedToDepositConversionRate;
                fh.ClosingTime = ConvertJson.StampToDateTime(h.ExecutionTimestamp);
                fh.ClosingPrice = h.ExecutionPrice;
                fh.Comment = h.Comment;
                fh.Commissions = h.PositionCloseDetails.Commission / 100;
                fh.EntryPrice = h.PositionCloseDetails.EntryPrice;
                long tempstamp = System.Convert.ToInt64(h.ExecutionTimestamp);
                foreach (var d in deal)
                {
                    if (d.PositionID == h.PositionID)
                    {
                        if (System.Convert.ToInt64(d.ExecutionTimestamp) < tempstamp)
                            tempstamp = System.Convert.ToInt64(d.ExecutionTimestamp);
                    }
                }
                fh.EntryTime = ConvertJson.StampToDateTime(tempstamp.ToString());
                fh.Equity = h.PositionCloseDetails.Equity / 100;
                fh.EquityBaseRoi = h.PositionCloseDetails.EquityBasedRoi / 100;
                fh.GrossProfit = h.PositionCloseDetails.Profit / 100;
                fh.Label = h.Label;
                fh.MarginRate = h.MarginRate;
                fh.Swap = h.PositionCloseDetails.Swap / 100;
                fh.NetProfit = fh.GrossProfit + fh.Swap + fh.Commissions;
                fh.Pips = h.PositionCloseDetails.ProfitInPips;
                fh.PositionId = h.PositionID;
                fh.SymbolCode = h.SymbolName;
                fh.Volume = h.PositionCloseDetails.ClosedVolume / 100;

                var tempvolume = Convert.ToDouble(fh.Volume);
                double tempsub = 100000;
                if (fh.SymbolCode == "XBRUSD" || fh.SymbolCode == "XTIUSD")
                    tempsub = 100;
                if (fh.SymbolCode == "XAGUSD" || fh.SymbolCode == "XAGEUR")
                    tempsub = 1000;
                if (fh.SymbolCode == "XAUUSD" || fh.SymbolCode == "XAUEUR")
                    tempsub = 100;
                fh.Quantity = tempvolume / tempsub;
                fh.QuoteToDepositConversionRate = h.PositionCloseDetails.QuoteToDepositConversionRate;
                fh.Roi = h.PositionCloseDetails.Roi;
                fh.TradeType = h.TradeSide == "BUY" ? TradeType.Sell : TradeType.Buy;
                fh.Digits= symbols.SingleOrDefault(x => x.SymbolName == fh.SymbolCode).PipPosition;
                var result = _context.FrxHistory.SingleOrDefault(x => x.ClosingDealId == fh.ClosingDealId);
                if (result == null)
                {
                    _context.Add(fh);
                    await _context.SaveChangesAsync();
                }
            }
            var frxhistories = _context.FrxHistory.Where(x => x.AccountId == frxaccount.AccountId);
            #endregion

            return View(Tuple.Create<FrxAccount, List<FrxAccount>>(frxaccount, frxaccounts.ToList()));
        }

        public JsonResult GetMonthBase([FromBody]Params param)
        {
            //Get History,Account
            var account = _context.FrxAccount.SingleOrDefault(x => x.AccountId == param.AcId);
            var positions = Position.GetPositions(_apiUrl, param.AcId.ToString(), _accessToken);
            var histories = _context.FrxHistory.Where(x => x.AccountId == param.AcId).OrderByDescending(x => x.ClosingTime).ToList();
            var cashflow = _context.FrxCashflow.Where(x => x.AccountId == param.AcId);
            #region Get MonthBaseData
            //Get XData
            var lastHistory = histories[0];
            var lastHisTime = lastHistory.ClosingTime;
            var lastHisTimeYear = lastHisTime.Year;
            var lastHisTimeMonth = lastHisTime.Month;
            List<XData> xDatas = new List<XData>();
            var xTime = new DateTime(lastHisTimeYear, lastHisTimeMonth, 1);
            //Add Months
            for (int i = 0; i < 12; i++)
            {
                var xt = xTime.AddMonths(i - 11);
                var xn = xt.Month.ToString() + "月份";
                XData xData = new XData(xn, xt);
                xDatas.Add(xData);
            }
            //Add Year,Total
            var yearBeginTime = new DateTime(lastHisTimeYear, 1, 1);
            var totalBeginTime = account.TraderRegistrationTime;
            var yearXData = new XData("全年", yearBeginTime);
            var totalXData = new XData("总计", totalBeginTime);
            xDatas.Add(yearXData);
            xDatas.Add(totalXData);
            //Get MonthBaseData
            List<MonthBaseData> monthBaseData = new List<MonthBaseData>();
            for (int i = 0; i < 14; i++)
            {
                MonthBaseData mdata = new MonthBaseData();
                var tempHis = new List<FrxHistory>();
                DateTime endTime = new DateTime();
                mdata.XData = xDatas[i];
                if (i < 12)
                    endTime = mdata.XData.XTime.AddMonths(1);
                else
                    endTime = lastHisTime.AddDays(2);
                tempHis = histories.Where(x => x.ClosingTime > mdata.XData.XTime && x.ClosingTime < endTime).OrderBy(y => y.ClosingTime).ToList();
                if (tempHis.Count() != 0)
                {
                    var list_bsData = tempHis.GroupBy(g => new
                    {
                        g.SymbolCode
                    }).Select(s => new BuySellData
                    {
                        SymbolCode = s.Key.SymbolCode,
                        Count = s.Count(),
                        Lots = s.Sum(a => a.Quantity),
                        Pips = s.Sum(a => a.Pips * a.Quantity) / s.Sum(a => a.Quantity),
                        Profit = s.Sum(a => a.NetProfit),
                        BuyCount = s.Where(a => a.TradeType == TradeType.Buy).Count(),
                        BuyLots = s.Where(a => a.TradeType == TradeType.Buy).Sum(a => a.Quantity),
                        BuyPips = s.Where(a => a.TradeType == TradeType.Buy).Sum(a => a.Pips * a.Quantity) / s.Where(a => a.TradeType == TradeType.Buy).Sum(a => a.Quantity),
                        BuyProfit = s.Where(a => a.TradeType == TradeType.Buy).Sum(a => a.NetProfit),
                        BuyRate = s.Where(t => t.TradeType == TradeType.Buy).Count() == 0 ? 0
                        : (s.Where(t => t.TradeType == TradeType.Sell).Count() == 0 ? 1
                        : Math.Round(s.Where(t => t.TradeType == TradeType.Buy).Sum(a => a.Quantity) / s.Sum(b => b.Quantity), 4)),
                        SellCount = s.Where(a => a.TradeType == TradeType.Sell).Count(),
                        SellLots = s.Where(a => a.TradeType == TradeType.Sell).Sum(a => a.Quantity),
                        SellPips = s.Where(a => a.TradeType == TradeType.Sell).Sum(a => a.Pips * a.Quantity) / s.Where(a => a.TradeType == TradeType.Sell).Sum(a => a.Quantity),
                        SellProfit = s.Where(a => a.TradeType == TradeType.Sell).Sum(a => a.NetProfit),
                        SellRate = s.Where(t => t.TradeType == TradeType.Buy).Count() == 0 ? 1
                        : (s.Where(t => t.TradeType == TradeType.Sell).Count() == 0 ? 0
                        : (1 - Math.Round(s.Where(t => t.TradeType == TradeType.Buy).Sum(a => a.Quantity) / s.Sum(b => b.Quantity), 4))),
                        WinCount = s.Where(a => a.NetProfit > 0).Count(),
                        WinRate = s.Where(a => a.NetProfit > 0).Count() == 0 ? 0
                        : (s.Where(a => a.NetProfit <= 0).Count() == 0 ? 1
                        : Math.Round((double)s.Where(a => a.NetProfit > 0).Count() / (double)s.Count(), 4)),
                        LossCount = s.Where(a => a.NetProfit <= 0).Count(),
                        LossRate = s.Where(a => a.NetProfit > 0).Count() == 0 ? 1
                        : (s.Where(a => a.NetProfit <= 0).Count() == 0 ? 0
                        : (1 - Math.Round((double)s.Where(a => a.NetProfit > 0).Count() / (double)s.Count(), 4))),
                    }).ToList();
                    var initBalance = tempHis.ToList()[0].Balance - tempHis.ToList()[0].NetProfit;
                    var net = tempHis.Select(x => x.NetProfit).Sum();
                    var gain = net / initBalance;
                    var swap = tempHis.Select(x => x.Swap).Sum();
                    var pips = tempHis.Select(x => x.Pips).Sum();
                    var lots = tempHis.Select(x => x.Quantity).Sum();
                    mdata.BuySellData = list_bsData;
                    mdata.Net = Math.Round(net, 2);
                    mdata.Gain = Math.Round(gain, 4);
                    mdata.Swap = Math.Round(swap, 2);
                    mdata.Pips = Math.Round(pips, 2);
                    mdata.Lots = Math.Round(lots, 2);
                    monthBaseData.Add(mdata);
                }
                else
                {
                    mdata.BuySellData = new List<BuySellData>();
                    mdata.Net = 0;
                    mdata.Gain = 0;
                    mdata.Swap = 0;
                    mdata.Pips = 0;
                    mdata.Lots = 0;
                    monthBaseData.Add(mdata);
                }
            }
            #endregion
            #region GetAccountInfo
            var deposit = 0.00;
            var withdraw = 0.00;
            foreach (var c in cashflow)
            {
                if (c.Type == "DEPOSIT")
                    deposit += c.Delta;
                if (c.Type == "WITHDRAW")
                    withdraw += c.Delta;
            }
            var maxbalance = histories.Select(x => x.Balance).Max();
            DateTime maxtime = histories.SingleOrDefault(x => x.Balance == maxbalance).ClosingTime.Date;
            var maxdrawdown = histories.Select(x => Math.Round((x.Balance - x.Equity) / x.Balance, 4)).Max();
            DateTime maxdrawtime = histories.SingleOrDefault(x => Math.Round((x.Balance - x.Equity) / x.Balance, 4) == maxdrawdown).ClosingTime.Date;
            var totalswap = histories.Select(x => x.Swap).Sum();
            var twr = 1.00;
            foreach (var m in monthBaseData)
            {
                if (m.XData.XName != "全年" && m.XData.XName != "总计")
                {
                    twr = (1 + m.Gain) * twr;
                }
            }
            DateTime lasttradetime;
            if (histories.Count() > 0)
                lasttradetime = histories.OrderByDescending(x => x.ClosingTime).ToList()[0].ClosingTime;
            else
                lasttradetime = account.TraderRegistrationTime;
            var accountinfo = new AccountInfo
            {
                Gain = Math.Round(twr - 1, 4) * 100,
                AbsGain = Math.Round((account.Balance - deposit + withdraw) / deposit, 4) * 100,
                MaxDraw = maxdrawdown * 100,
                MaxDrawTime = maxdrawtime,
                Deposit = deposit,
                Withdraw = withdraw,
                Balance = account.Balance,
                Equity = account.Equity,
                MaxBalance = maxbalance,
                MaxBalanceTime = maxtime,
                TotalProfit = account.Balance - deposit + withdraw,
                TotalSwap = Math.Round(totalswap, 2),
                LastTradeTime = lasttradetime,
                RigistrationTime = account.TraderRegistrationTime
            };
            #endregion
            return Json(new { monthBaseData, accountinfo });
        }

        public JsonResult GetHistory([FromBody]Params param)
        {
            var data = _context.FrxHistory.Where(x => x.AccountId == param.AcId).ToList();
            return Json(new { data, data.Count });
        }
    }

    public class XData
    {
        public XData(string xn, DateTime xt)
        {
            XName = xn;
            XTime = xt;
        }
        public string XName { get; set; }
        public DateTime XTime { get; set; }
    }

    public class MonthBaseData
    {
        public XData XData { get; set; }
        public List<BuySellData> BuySellData { get; set; }
        public double Net { get; set; }
        public double Gain { get; set; }
        public double Swap { get; set; }
        public double Pips { get; set; }
        public double Lots { get; set; }
    }

    public class BuySellData
    {
        public string SymbolCode { get; set; }
        public int Count { get; set; }
        public double Lots { get; set; }
        public double Pips { get; set; }
        public double Profit { get; set; }
        public int BuyCount { get; set; }
        public double BuyLots { get; set; }
        public double BuyPips { get; set; }
        public double BuyProfit { get; set; }
        public double BuyRate { get; set; }
        public int SellCount { get; set; }
        public double SellLots { get; set; }
        public double SellPips { get; set; }
        public double SellProfit { get; set; }
        public double SellRate { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public double WinRate { get; set; }
        public double LossRate { get; set; }
    }

    public class AccountInfo
    {
        public double Gain { get; set; }
        public double AbsGain { get; set; }
        public double MaxDraw { get; set; }
        public DateTime MaxDrawTime { get; set; }
        public double Deposit { get; set; }
        public double Withdraw { get; set; }
        public double Balance { get; set; }
        public double Equity { get; set; }
        public double MaxBalance { get; set; }
        public DateTime MaxBalanceTime { get; set; }
        public double TotalProfit { get; set; }
        public double TotalSwap { get; set; }
        public DateTime LastTradeTime { get; set; }
        public DateTime RigistrationTime { get; set; }
    }
}
