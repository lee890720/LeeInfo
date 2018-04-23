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
using LeeInfo.Data.Forex;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class FrxPositionController : Controller
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

        public FrxPositionController(AppDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
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
            foreach(var a in accounts)
            {
                var sql_accounts = _context.FrxAccount.Where(x => x.AccountId == Convert.ToInt32(a.AccountId));
                if(sql_accounts.Count()==0)
                {
                    FrxAccount fa = new FrxAccount();
                    fa.AccountId = Convert.ToInt32(a.AccountId);
                }
            }
            string accountId = null;
            TradingAccount account = new TradingAccount();
            foreach(var a in accounts)
            {
                a.Balance = (System.Convert.ToDouble(a.Balance) / 100).ToString();
                if (a.Live == "true")
                {
                    accountId = a.AccountId;
                    account = a;
                }
            }
            var position = Position.GetPositions(_apiUrl, accountId, _accessToken);
            foreach (var p in position)
            {
                p.Volume = (System.Convert.ToInt64(p.Volume) / 100).ToString();
                p.Profit = (System.Convert.ToDouble(p.Profit) / 100).ToString();
                p.Commission = (System.Convert.ToDouble(p.Commission) / 100*2).ToString();
                p.Swap = (System.Convert.ToDouble(p.Swap) / 100).ToString();
            }
            return View(position);
        }
    }
}