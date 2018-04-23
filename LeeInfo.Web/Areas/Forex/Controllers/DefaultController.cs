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

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class DefaultController : Controller
    {
        private string _clientId = "";
        private string _clientSecret = "";
        private string _accessToken = "";
        private string _refreshToken = "";
        private string _connectUrl = "https://connect.spotware.com/";
        private string _apiUrl = "https://api.spotware.com/";
        private string _apiHost = "tradeapi.spotware.com";
        private int _apiPort = 5032;
        private TcpClient _tcpClient = new TcpClient();
        private SslStream _apiSocket;
        private readonly AppDbContext _identitycontext;
        UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        public DefaultController(AppDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
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
            var profile = Profile.GetProfile(_apiUrl, _accessToken);
            var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
            _tcpClient = new TcpClient(_apiHost, _apiPort);
            _apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            _apiSocket.AuthenticateAsClient(_apiHost);

            string accountID = null;
            string accountStartTimeStamp = null;
            TradingAccount account = new TradingAccount();
            foreach (var a in accounts)
            {
                a.Balance = (System.Convert.ToDouble(a.Balance) / 100).ToString();
                if (a.Live == "true")
                {
                    accountID = a.AccountId;
                    accountStartTimeStamp = a.TraderRegistrationTimestamp;
                    account = a;
                }
            }
            DateTime utcnow = DateTime.UtcNow;
            string fromtimestamp = accountStartTimeStamp;
            string totimestamp = ConvertJson.DateTimeToStamp(utcnow);
            var deal = Deal.GetDeals(_apiUrl, accountID, _accessToken, fromtimestamp, totimestamp);
            var cashflowhistory = CashflowHistory.GetCashflowHistory(_apiUrl, accountID, _accessToken);
            var pendingorder = PendingOrder.GetPendingOrders(_apiUrl, accountID, _accessToken);
            var position = Position.GetPositions(_apiUrl, accountID, _accessToken);
            var symbol = Symbols.GetSymbols(_apiUrl, accountID, _accessToken);
            var deal_history = new List<Deal>();
            foreach (var p in position)
            {
                p.Volume = (System.Convert.ToInt64(p.Volume) / 100).ToString();
                p.Profit = (System.Convert.ToDouble(p.Profit) / 100).ToString();
                p.Commission = (System.Convert.ToDouble(p.Commission) / 100).ToString();
                p.Swap = (System.Convert.ToDouble(p.Swap) / 100).ToString();
            }
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





            DateTime thetime = new DateTime(utcnow.Year, utcnow.Month, utcnow.Day, 0, 0, 0);               
            var week = new List<string>();
            var line1 = new List<double>();
            var line2 = new List<double>();
            for (int i = 1; i <= 7; i++)
            {
                week.Add(utcnow.AddDays(-i).ToShortDateString());
                var listtemp = deal_history.Select(x => new
                {
                    balance = Convert.ToDouble(x.PositionCloseDetails.Balance),
                    equity = Convert.ToDouble(x.PositionCloseDetails.Equity),
                    closingtime = ConvertJson.StampToDateTime(x.ExecutionTimestamp)
                }).OrderByDescending(y => y.closingtime).ToList();
                foreach (var h in listtemp)
                {
                    if (h.closingtime < thetime.AddDays(1 - i))
                    {
                        line1.Add(Math.Round(System.Convert.ToDouble((double)h.balance)));
                        line2.Add(Math.Round(System.Convert.ToDouble((double)h.equity)));
                        break;
                    }
                }
            }
            week.Reverse();
            line1.Reverse();
            line2.Reverse();
            {
                Chart chart = new Chart();

                chart.Type = "line";

                ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
                data.Labels = week;

                LineDataset dataset1 = new LineDataset()
                {
                    Label = "My First dataset",
                    Data = line1,
                    Fill = "false",
                    LineTension = 0.1,
                    BackgroundColor = "rgba(75, 192, 192, 0.4)",
                    BorderColor = "rgba(75,192,192,1)",
                    BorderCapStyle = "butt",
                    BorderDash = new List<int> { },
                    BorderDashOffset = 0.0,
                    BorderJoinStyle = "miter",
                    PointBorderColor = new List<string>() { "rgba(75,192,192,1)" },
                    PointBackgroundColor = new List<string>() { "#fff" },
                    PointBorderWidth = new List<int> { 1 },
                    PointHoverRadius = new List<int> { 5 },
                    PointHoverBackgroundColor = new List<string>() { "rgba(75,192,192,1)" },
                    PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                    PointHoverBorderWidth = new List<int> { 2 },
                    PointRadius = new List<int> { 1 },
                    PointHitRadius = new List<int> { 10 },
                    SpanGaps = false,
                };
                LineDataset dataset2 = new LineDataset()
                {
                    Label = "My First dataset2",
                    Data = line2,
                    Fill = "false",
                    LineTension = 0.1,
                    BackgroundColor = "rgba(75, 192, 192, 0.4)",
                    BorderColor = "rgba(75,192,192,1)",
                    BorderCapStyle = "butt",
                    BorderDash = new List<int> { },
                    BorderDashOffset = 0.0,
                    BorderJoinStyle = "miter",
                    PointBorderColor = new List<string>() { "rgba(75,192,192,1)" },
                    PointBackgroundColor = new List<string>() { "#fff" },
                    PointBorderWidth = new List<int> { 1 },
                    PointHoverRadius = new List<int> { 5 },
                    PointHoverBackgroundColor = new List<string>() { "rgba(75,192,192,1)" },
                    PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                    PointHoverBorderWidth = new List<int> { 2 },
                    PointRadius = new List<int> { 1 },
                    PointHitRadius = new List<int> { 10 },
                    SpanGaps = false
                };

                data.Datasets = new List<Dataset>();
                data.Datasets.Add(dataset1);
                data.Datasets.Add(dataset2);

                chart.Data = data;

                Options options = new Options()
                {
                    Scales = new Scales()
                };

                Scales scales = new Scales()
                {
                    YAxes = new List<Scale>()
                    {
                        new CartesianScale()
                        {
                            Ticks = new CartesianLinearTick()
                            {
                                BeginAtZero = true
                            }
                        }
                    }
                };
                options.Scales = scales;

                chart.Options = options;

                ViewData["chart"] = chart;
            }
            return View(Tuple.Create(account, position, deal_history, _context.FrxCbotset.ToList()));
        }
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }
    }
}