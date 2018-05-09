﻿using System;
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

        public FrxAnalyzeController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
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
            #region GetCashflow
            var cashflow = CashflowHistory.GetCashflowHistory(_apiUrl, frxaccount.AccountId.ToString(), _accessToken);
            var deposit = 0.00;
            var withdraw = 0.00;
            foreach (var c in cashflow)
            {
                if (c.Type == "DEPOSIT")
                    deposit += c.Delta / 100;
                if (c.Type == "WITHDRAW")
                    withdraw += c.Delta / 100;
            }
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
            #region GetHistory
            //DateTime utcnow = DateTime.UtcNow;
            //string fromtimestamp = ConvertJson.DateTimeToStamp(frxaccount.TraderRegistrationTime);
            //string totimestamp = ConvertJson.DateTimeToStamp(utcnow);
            //var deal = Deal.GetDeals(_apiUrl, frxaccount.AccountId.ToString(), _accessToken, fromtimestamp, totimestamp);
            //var deal_history = new List<Deal>();
            //foreach (var d in deal)
            //{
            //    if (d.PositionCloseDetails != null)
            //        deal_history.Add(d);
            //}
            //foreach (var h in deal_history)
            //{
            //    FrxHistory fh = new FrxHistory();
            //    fh.ClosingDealId = h.DealID;
            //    fh.AccountId = frxaccount.AccountId;
            //    fh.Balance = h.PositionCloseDetails.Balance / 100;
            //    fh.BalanceVersion = h.PositionCloseDetails.BalanceVersion;
            //    fh.BaseToUSDConversionRate = h.BaseToUSDConversionRate;
            //    fh.ClosedToDepoitConversionRate = h.PositionCloseDetails.ClosedToDepositConversionRate;
            //    fh.ClosingTime = ConvertJson.StampToDateTime(h.ExecutionTimestamp);
            //    fh.ClosingPrice = h.ExecutionPrice;
            //    fh.Comment = h.Comment;
            //    fh.Commissions = h.PositionCloseDetails.Commission / 100;
            //    fh.EntryPrice = h.PositionCloseDetails.EntryPrice;
            //    long tempstamp = System.Convert.ToInt64(h.ExecutionTimestamp);
            //    foreach (var d in deal)
            //    {
            //        if (d.PositionID == h.PositionID)
            //        {
            //            if (System.Convert.ToInt64(d.ExecutionTimestamp) < tempstamp)
            //                tempstamp = System.Convert.ToInt64(d.ExecutionTimestamp);
            //        }
            //    }
            //    fh.EntryTime = ConvertJson.StampToDateTime(tempstamp.ToString());
            //    fh.Equity = h.PositionCloseDetails.Equity / 100;
            //    fh.EquityBaseRoi = h.PositionCloseDetails.EquityBasedRoi / 100;
            //    fh.GrossProfit = h.PositionCloseDetails.Profit / 100;
            //    fh.Label = h.Label;
            //    fh.MarginRate = h.MarginRate;
            //    fh.Swap = h.PositionCloseDetails.Swap / 100;
            //    fh.NetProfit = fh.GrossProfit + fh.Swap + fh.Commissions;
            //    fh.Pips = h.PositionCloseDetails.ProfitInPips;
            //    fh.PositionId = h.PositionID;
            //    fh.SymbolCode = h.SymbolName;
            //    fh.Volume = h.PositionCloseDetails.ClosedVolume / 100;

            //    var tempvolume = Convert.ToDouble(fh.Volume);
            //    double tempsub = 100000;
            //    if (fh.SymbolCode == "XBRUSD" || fh.SymbolCode == "XTIUSD")
            //        tempsub = 100;
            //    if (fh.SymbolCode == "XAGUSD" || fh.SymbolCode == "XAGEUR")
            //        tempsub = 1000;
            //    if (fh.SymbolCode == "XAUUSD" || fh.SymbolCode == "XAUEUR")
            //        tempsub = 100;
            //    fh.Quantity = tempvolume / tempsub;
            //    fh.QuoteToDepositConversionRate = h.PositionCloseDetails.QuoteToDepositConversionRate;
            //    fh.Roi = h.PositionCloseDetails.Roi;
            //    fh.TradeType = h.TradeSide == "BUY" ? TradeType.Sell : TradeType.Buy;
            //    var result = _context.FrxHistory.Find(fh.ClosingDealId);
            //    if (result == null)
            //    {
            //        _context.Add(fh);
            //        await _context.SaveChangesAsync();
            //    }
            //}
            var frxhistories = _context.FrxHistory.Where(x => x.AccountId == frxaccount.AccountId);
            #endregion
            #region GetAccountInfo
            var maxbalance = frxhistories.Select(x => x.Balance).Max();
            DateTime maxtime = frxhistories.SingleOrDefault(x => x.Balance == maxbalance).ClosingTime.Date;
            var maxdrawdown = frxhistories.Select(x => Math.Round((x.Balance - x.Equity) / x.Balance, 4)).Max();
            DateTime maxdrawtime = frxhistories.SingleOrDefault(x => Math.Round((x.Balance - x.Equity) / x.Balance, 4) == maxdrawdown).ClosingTime.Date;
            var totalswap = frxhistories.Select(x => x.Swap).Sum();
            DateTime lasttradetime;
            if (frxpositions.Count() > 0)
                lasttradetime = frxpositions.OrderByDescending(x => x.EntryTime).ToList()[0].EntryTime;
            else if (frxhistories.Count() > 0)
                lasttradetime = frxhistories.OrderByDescending(x => x.ClosingTime).ToList()[0].ClosingTime;
            else
                lasttradetime = frxaccount.TraderRegistrationTime;
            var accountinfo = new AccountInfo
            {
                Gain = Math.Round((frxaccount.Balance - deposit + withdraw) / frxaccount.Equity, 4) * 100,
                AbsGain = Math.Round((frxaccount.Balance - deposit + withdraw) / deposit, 4) * 100,
                MaxDraw = maxdrawdown * 100,
                MaxDrawTime = maxdrawtime,
                Deposit = deposit,
                Withdraw = withdraw,
                Balance = frxaccount.Balance,
                Equity = frxaccount.Equity,
                MaxBalance = maxbalance,
                MaxBalanceTime = maxtime,
                TotalProfit = frxaccount.Balance - deposit + withdraw,
                TotalSwap = Math.Round(totalswap, 2),
                LastTradeTime = lasttradetime,
                RigistrationTime = frxaccount.TraderRegistrationTime
            };
            #endregion

            return View(Tuple.Create<FrxAccount, List<FrxAccount>, AccountInfo>(frxaccount, frxaccounts.ToList(), accountinfo));
        }

        public JsonResult GetAnalyzeData(int? acId)
        {
            var histories = _context.FrxHistory.Where(x => x.AccountId == acId).OrderByDescending(x => x.ClosingTime).ToList();
            #region Get MonthBaseData
            var lastHistory = histories[0];
            var lastHisTime = lastHistory.ClosingTime;
            var lastHisTimeYear = lastHisTime.Year;
            var lastHisTimeMonth = lastHisTime.Month;
            List<XData> xDatas = new List<XData>();
            var xTime = new DateTime(lastHisTimeYear, lastHisTimeMonth, 1);
            for (int i = 0; i < 12; i++)
            {
                var xt = xTime.AddMonths(i - 11);
                var xn = xt.Month.ToString() + "月份";
                XData xData = new XData(xn,xt);
                xDatas.Add(xData);
            }
            List<MonthBaseData> mDatas = new List<MonthBaseData>();
            for (int i = 0; i < 12; i++)
            {
                MonthBaseData mdata = new MonthBaseData();
                mdata.xData = xDatas[i];
                var tempHis = histories.Where(x => x.ClosingTime > mdata.xData.XTime && x.ClosingTime < mdata.xData.XTime.AddMonths(1)).OrderBy(y => y.ClosingTime);
                if (tempHis.Count() != 0)
                {
                    var list_pie = tempHis.GroupBy(g => new
                    {
                        g.SymbolCode
                    }).Select(s => new PieData
                    {
                        SymbolCode = s.Key.SymbolCode,
                        Lots = s.Sum(z => z.Quantity)
                    }).ToList();
                    var list_bsRate = tempHis.GroupBy(g => new
                    {
                        g.SymbolCode
                    }).Select(s => new BuySellRateData
                    {
                        SymbolCode = s.Key.SymbolCode,
                        BuyRate = s.Where(t => t.TradeType == TradeType.Buy).Count() == 0 ? 0 
                        : (s.Where(t => t.TradeType == TradeType.Sell).Count() == 0 ? 1
                        :Math.Round(s.Where(t => t.TradeType == TradeType.Buy).Sum(a => a.Quantity) / s.Sum(b => b.Quantity), 2)),
                        SellRate = s.Where(t => t.TradeType == TradeType.Buy).Count() == 0 ? 1
                        : (s.Where(t => t.TradeType == TradeType.Sell).Count() == 0 ? 0
                        : (1-Math.Round(s.Where(t => t.TradeType == TradeType.Buy).Sum(a => a.Quantity) / s.Sum(b => b.Quantity), 2))),
                    }).ToList();
                    var initBalance = tempHis.ToList()[0].Balance - tempHis.ToList()[0].NetProfit;
                    var net = tempHis.Select(x => x.NetProfit).Sum();
                    var gain = net / initBalance;
                    var swap = tempHis.Select(x => x.Swap).Sum();
                    var pips = tempHis.Select(x => x.Pips).Sum();
                    var lots = tempHis.Select(x => x.Quantity).Sum();
                    mdata.pieDatas = list_pie;
                    mdata.buySellRateDatas = list_bsRate;
                    mdata.Net = Math.Round(net, 2);
                    mdata.Gain = Math.Round(gain, 2);
                    mdata.Swap = Math.Round(swap, 2);
                    mdata.Pips = Math.Round(pips, 2);
                    mdata.Lots = Math.Round(lots, 2);
                    mDatas.Add(mdata);
                }
                else
                {
                    mdata.pieDatas = new List<PieData>();
                    mdata.buySellRateDatas = new List<BuySellRateData>();
                    mdata.Net = 0;
                    mdata.Gain = 0;
                    mdata.Swap = 0;
                    mdata.Pips = 0;
                    mdata.Lots = 0;
                    mDatas.Add(mdata);
                }
            }
            var monthBaseData = mDatas;
            #endregion
            #region Get YearSourceData
            var yearBeginTime = new DateTime(lastHisTimeYear, 1, 1);
            var frxaccount = _context.FrxAccount.SingleOrDefault(x => x.AccountId == acId);
            var totalBeginTime = frxaccount.TraderRegistrationTime;
            var endTime = lastHisTime;
            var yearXData = new XData("全年",yearBeginTime);
            var totalXData = new XData("总计",totalBeginTime);
            List<MonthBaseData> yearSourceData = new List<MonthBaseData>();
            List<XData> xDatasTemp = new List<XData>();
            xDatasTemp.Add(yearXData);
            xDatasTemp.Add(totalXData);
            foreach(var t in xDatasTemp )
            {
                MonthBaseData mdata = new MonthBaseData();
                mdata.xData = t;
                var tempHis = histories.Where(x => x.ClosingTime > t.XTime && x.ClosingTime < endTime).OrderBy(o => o.ClosingTime);
                if (tempHis.Count() != 0)
                {
                    var list_pie = tempHis.GroupBy(g => new
                    {
                        g.SymbolCode
                    }).Select(s => new PieData
                    {
                        SymbolCode = s.Key.SymbolCode,
                        Lots = s.Sum(z => z.Quantity)
                    }).ToList();
                    var list_bsRate = tempHis.GroupBy(g => new
                    {
                        g.SymbolCode
                    }).Select(s => new BuySellRateData
                    {
                        SymbolCode = s.Key.SymbolCode,
                        BuyRate = s.Where(z => z.TradeType == TradeType.Buy).Count() == 0 ? 0
                        : (s.Where(z => z.TradeType == TradeType.Sell).Count() == 0 ? 1
                        : Math.Round(s.Where(z => z.TradeType == TradeType.Buy).Sum(a => a.Quantity) / s.Sum(b => b.Quantity), 2)),
                        SellRate = s.Where(z => z.TradeType == TradeType.Buy).Count() == 0 ? 1
                        : (s.Where(z => z.TradeType == TradeType.Sell).Count() == 0 ? 0
                        : (1 - Math.Round(s.Where(z => z.TradeType == TradeType.Buy).Sum(a => a.Quantity) / s.Sum(b => b.Quantity), 2))),
                    }).ToList();
                    var initBalance = tempHis.ToList()[0].Balance - tempHis.ToList()[0].NetProfit;
                    var net = tempHis.Select(x => x.NetProfit).Sum();
                    var gain = net / initBalance;
                    var swap = tempHis.Select(x => x.Swap).Sum();
                    var pips = tempHis.Select(x => x.Pips).Sum();
                    var lots = tempHis.Select(x => x.Quantity).Sum();
                    mdata.pieDatas = list_pie;
                    mdata.buySellRateDatas =list_bsRate;
                    mdata.Net = Math.Round(net, 2);
                    mdata.Gain = Math.Round(gain, 2);
                    mdata.Swap = Math.Round(swap, 2);
                    mdata.Pips = Math.Round(pips, 2);
                    mdata.Lots = Math.Round(lots, 2);
                    yearSourceData.Add(mdata);
                }
                else
                {
                    mdata.pieDatas = new List<PieData>();
                    mdata.buySellRateDatas = new List<BuySellRateData>();
                    mdata.Net = 0;
                    mdata.Gain = 0;
                    mdata.Swap = 0;
                    mdata.Pips = 0;
                    mdata.Lots = 0;
                    yearSourceData.Add(mdata);
                }
            }
            #endregion
            return Json(new { monthBaseData,yearSourceData });
        }
    }

    public class XData
    {
        public XData(string xn,DateTime xt)
        {
            XName = xn;
            XTime = xt;
        }
        public string XName { get; set; }
        public DateTime XTime { get; set; }
    }

    public class MonthBaseData
    {
        public XData xData { get; set; }
        public List<PieData> pieDatas { get; set; }
        public List<BuySellRateData> buySellRateDatas { get; set; }
        public double Net { get; set; }
        public double Gain { get; set; }
        public double Swap { get; set; }
        public double Pips { get; set; }
        public double Lots { get; set; }
    }

    public class PieData
    {
        public string SymbolCode { get; set; }
        public double Lots { get; set; }
    }

    public class BuySellRateData
    {
        public string SymbolCode { get; set; }
        public double BuyRate { get; set; }
        public double SellRate { get; set; }
    }
}
