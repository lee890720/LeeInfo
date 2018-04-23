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
using LeeInfo.Library;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class FrxHistoryController : Controller
    {
        private string _clientId = "";
        private string _clientSecret = "";
        private string _accessToken = "";
        private string _refreshToken = "";
        private string _apiUrl = "https://api.spotware.com/";
        private TcpClient _tcpClient = new TcpClient();
        private readonly AppDbContext _identitycontext;
        UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        public FrxHistoryController(AppDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            _clientId = _user.ClientId;
            _clientSecret = _user.ClientSecret;
            _accessToken = _user.AccessToken;
            _refreshToken = _user.RefreshToken;
            var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
            ViewData["AccountId"] = new SelectList(accounts, "AccountId", "AccountNumber");

            string accountId = null;
            TradingAccount account = new TradingAccount();
            string accountStartTimeStamp = null;
            foreach (var a in accounts)
            {
                a.Balance = (System.Convert.ToDouble(a.Balance) / 100).ToString();
                if (a.Live == "true")
                {
                    accountId = a.AccountId;
                    account = a;
                    accountStartTimeStamp = a.TraderRegistrationTimestamp;
                }
            }
            DateTime utcnow = DateTime.UtcNow;
            string fromtimestamp = accountStartTimeStamp;
            string totimestamp = ConvertJson.DateTimeToStamp(utcnow);
            var deal = Deal.GetDeals(_apiUrl, accountId, _accessToken, fromtimestamp, totimestamp);
            var deal_history = new List<Deal>();
            foreach (var d in deal)
            {
                if (d.PositionCloseDetails != null)
                    deal_history.Add(d);
            }
            for (int i = deal_history.Count - 1; i >= 0; i--)
            {
                deal_history[i].TradeSide = deal_history[i].TradeSide == "BUY" ? "SELL" : "BUY";
                deal_history[i].PositionCloseDetails.Profit = (System.Convert.ToDouble(deal_history[i].PositionCloseDetails.Profit) / 100).ToString();
                deal_history[i].PositionCloseDetails.Swap = (System.Convert.ToDouble(deal_history[i].PositionCloseDetails.Swap) / 100).ToString();
                deal_history[i].PositionCloseDetails.Commission = (System.Convert.ToDouble(deal_history[i].PositionCloseDetails.Commission) / 100).ToString();
                deal_history[i].PositionCloseDetails.Balance = (System.Convert.ToDouble(deal_history[i].PositionCloseDetails.Balance) / 100).ToString();
                deal_history[i].PositionCloseDetails.Equity = (System.Convert.ToDouble(deal_history[i].PositionCloseDetails.Equity) / 100).ToString();
                deal_history[i].PositionCloseDetails.ClosedVolume = (System.Convert.ToDouble(deal_history[i].PositionCloseDetails.ClosedVolume) / 100).ToString();
                long temp = System.Convert.ToInt64(deal_history[i].ExecutionTimestamp);
                foreach (var d in deal)
                {
                    if (d.PositionID == deal_history[i].PositionID)
                    {
                        if (System.Convert.ToInt64(d.ExecutionTimestamp) < temp)
                            temp = System.Convert.ToInt64(d.ExecutionTimestamp);
                    }
                }
                deal_history[i].CreateTimestamp = temp.ToString();
            }
            return View(deal_history);
        }
    }
}