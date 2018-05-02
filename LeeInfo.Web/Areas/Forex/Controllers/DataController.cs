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
using LeeInfo.Lib;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeeInfo.Data.Forex;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class DataController : Controller
    {
        private readonly AppDbContext _identitycontext;
        UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        public DataController(AppDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            #region GetAccount
            string _clientId = "";
            string _clientSecret = "";
            string _accessToken = "";
            string _refreshToken = "";
            string _apiUrl = "https://api.spotware.com/";
            TcpClient _tcpClient = new TcpClient();
            AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            _clientId = _user.ClientId;
            _clientSecret = _user.ClientSecret;
            _accessToken = _user.AccessToken;
            _refreshToken = _user.RefreshToken;
            var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
            var temp = _context.FrxAccount.Where(x => x.UserName == User.Identity.Name);
            _context.RemoveRange(temp);
            await _context.SaveChangesAsync();
            foreach (var a in accounts)
            {
                var sql_accounts = _context.FrxAccount.Where(x => x.AccountId == Convert.ToInt32(a.AccountId));
                if (sql_accounts.Count() == 0)
                {
                    FrxAccount fa = new FrxAccount();
                    fa.AccountId = a.AccountId;
                    fa.AccountNumber = a.AccountNumber;
                    fa.Balance = a.Balance / 100;
                    fa.BrokerName = a.BrokerTitle;
                    fa.Currency = a.DepositCurrency;
                    fa.IsLive = a.Live;
                    fa.PreciseLeverage = a.Leverage;
                    fa.TraderRegistrationTime = ConvertJson.StampToDateTime(a.TraderRegistrationTimestamp);
                    fa.UserName = User.Identity.Name;
                    _context.Add(fa);
                    await _context.SaveChangesAsync();
                }
            }
            var frxaccount = _context.FrxAccount.FirstOrDefault(x => x.IsLive == true);
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
                _context.Add(fp);
                await _context.SaveChangesAsync();
            }
            var frxpositions = _context.FrxPosition.Where(x => x.AccountId == frxaccount.AccountId);
            #endregion
            #region GetHistory
            DateTime utcnow = DateTime.UtcNow;
            string fromtimestamp = ConvertJson.DateTimeToStamp(frxaccount.TraderRegistrationTime);
            string totimestamp = ConvertJson.DateTimeToStamp(utcnow);
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
                var result = _context.FrxHistory.Find(fh.ClosingDealId);
                if (result == null)
                {
                    _context.Add(fh);
                    await _context.SaveChangesAsync();
                }
            }
            var frxhistories = _context.FrxHistory.Where(x => x.AccountId == frxaccount.AccountId);
            #endregion

            var frxdata = new FrxData();
            frxdata.Account = frxaccount;
            frxdata.Positions = frxpositions.ToList();
            frxdata.Histories = frxhistories.ToList();
            return View(frxdata);
        }

        public JsonResult GetPosition()
        {
            var data = _context.FrxPosition.ToList();
            return Json(new { data, data.Count });
        }

        public JsonResult GetHistory()
        {
            var data = _context.FrxHistory.ToList();
            return Json(new { data, data.Count });
        }
    }

    public class FrxData
    {
        public FrxAccount Account { get; set; }
        public List<FrxPosition> Positions { get; set; }
        public List<FrxHistory> Histories { get; set; }
    }
}