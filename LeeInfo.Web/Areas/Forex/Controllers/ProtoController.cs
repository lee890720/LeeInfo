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
using System.Threading;
using LeeInfo.Data.Forex;
using LeeInfo.Lib;
using Newtonsoft.Json;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class ProtoController : Controller
    {
        private string _clientId = "458_ZXTfD6n0yf9WE3kyrfN5fPhnUkGjH7brIGDHaeezPNHV6GvRQQ";
        private string _clientSecret = "D7s28Opo1obNfsHPL6hY14TBAUs0GcHjX7juH67GhZOU9LeaxE";
        private string _accessToken = "CMU_aak6k2uQctZ2UOTZdxGBqA-eeOOtf8rfOpfpOV4";
        private string _refreshToken = "ocpreSmGtEPRubvh8ZVcL_v3SXNVm-PFSEgnMQFHuUM";
        private string _connectUrl = "https://connect.spotware.com/";
        private string _apiUrl = "https://api.spotware.com/";
        private string _apiHost = "tradeapi.spotware.com";
        private int _apiPort = 5032;
        private TcpClient _tcpClient = new TcpClient();
        private SslStream _apiSocket;
        private readonly AppIdentityDbContext _identitycontext;
        private UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        public ProtoController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }
        public async Task<IActionResult> Index(int? acId)
        {
            #region Parameters
            AppIdentityUser _user = await _userManager.FindByNameAsync(User.Identity.Name);
            AppIdentityUser _admin = await _userManager.FindByNameAsync("lee890720");
            _clientId = _user.ClientId;
            _clientSecret = _user.ClientSecret;
            _accessToken = _user.AccessToken;
            _refreshToken = _user.RefreshToken;
            var profile = Profile.GetProfile(_apiUrl, _accessToken);
            var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
            _tcpClient = new TcpClient(_apiHost, _apiPort);
            _apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            _apiSocket.AuthenticateAsClient(_apiHost);
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
                var TAC = new AspNetUserForexAccount();
                var tempAC = useraccounts.SingleOrDefault(x => x.Alive == true);
                if (tempAC == null)
                    TAC = useraccounts[0];
                else
                    TAC = tempAC;
                frxaccount = frxaccounts.SingleOrDefault(x => x.AccountNumber == TAC.AccountNumber);
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
                fp.Digits = symbols.SingleOrDefault(x => x.SymbolName == fp.SymbolCode).PipPosition;
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
            poss = frxpositions.GroupBy(g => new { g.SymbolCode, g.TradeType, g.Digits })
                .Select(s => new PosGroup
                {
                    SymbolCode = s.Key.SymbolCode,
                    TradeType = s.Key.TradeType,
                    Quantity = s.Sum(a => a.Quantity),
                    EntryPrice = s.Sum(a => a.EntryPrice * a.Quantity) / s.Sum(b => b.Quantity),
                    Swap = s.Sum(a => a.Swap),
                    NetProfit = s.Sum(a => a.NetProfit),
                    Pips = s.Sum(a => a.Pips * a.Quantity) / s.Sum(b => b.Quantity),
                    Gain = s.Sum(a => a.NetProfit) / frxaccount.Balance,
                    Digits = s.Key.Digits,
                }).OrderBy(o=>o.NetProfit).ToList();
            #endregion

            return View(Tuple.Create<FrxAccount, List<FrxAccount>, List<PosGroup>>(frxaccount, frxaccounts.ToList(), poss));
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
            poss = frxpositions.GroupBy(g => new { g.SymbolCode, g.TradeType })
                .Select(s => new PosGroup
                {
                    SymbolCode = s.Key.SymbolCode,
                    TradeType = s.Key.TradeType,
                    Quantity = s.Sum(a => a.Quantity),
                    EntryPrice = 0,
                    Swap = 0,
                    NetProfit = 0,
                    Pips = 0,
                    Gain = 0,
                    Digits=0
                }).OrderByDescending(o=>o.Quantity).ToList();
            var data = poss;
            return Json(new { data, data.Count });
        }

        private void SendMarketOrderRequest(int acId, string accesstoken, long accountId, string accessToken, string symbolName, ProtoTradeSide tradeSide, long volume)
        {
            var auth=SendAuthorizationRequest();
            var accountID = acId;
            var token = accesstoken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateMarketOrderRequest(accountID, token, symbolName, tradeSide, volume);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
            //lblResponse.Text += "<br/>";
            Thread.Sleep(1000);
            _message = Listen(_apiSocket);
            protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text += OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SendMarketRangeOrderRequest()
        {
            SendAuthorizationRequest();
            var accountID = "1960408";
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateMarketRangeOrderRequest(Convert.ToInt32(accountID), token, "EURUSD", ProtoTradeSide.BUY, 100000, 1.18, 10);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SendLimitOrderRequest()
        {
            SendAuthorizationRequest();
            var accountID = "1960408";
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateLimitOrderRequest(Convert.ToInt32(accountID), token, "EURUSD", ProtoTradeSide.BUY, 200000, 1.09);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SendAmendLimitOrderRequest()
        {
            SendAuthorizationRequest();
            var accountID = "1960408";
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateAmendLimitOrderRequest(Convert.ToInt32(accountID), token, 69212987, 1.10);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SendStopOrderRequest()
        {
            SendAuthorizationRequest();
            var accountID = "1960408";
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateStopOrderRequest(Convert.ToInt32(accountID), token, "EURUSD", ProtoTradeSide.BUY, 400000, 1.3);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SendClosePositionRequest()
        {
            SendAuthorizationRequest();
            var accountID = "1960408";
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateClosePositionRequest(Convert.ToInt32(accountID), token, 43901148, 100000);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SubscribeForSpotsRequest()
        {
            SendAuthorizationRequest();
            var accountID = "1960408";
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateSubscribeForSpotsRequest(Convert.ToInt32(accountID), token, "EURUSD");
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void RefreshToken()
        {
            var token =_refreshToken;
            var newToken = AccessToken.RefreshAccessToken(_connectUrl, token, _clientId, _clientSecret);

        }

        public JsonResult AmendPosition(int acId,int positionId,double? stopLoss,double? takeProfit)
        {
            AppIdentityUser _user = _identitycontext.Users.SingleOrDefault(x => x.UserName == "lee890720");
            _clientId = _user.ClientId;
            _clientSecret = _user.ClientSecret;
            _accessToken = _user.AccessToken;
            _refreshToken = _user.RefreshToken;
            var auth=SendAuthorizationRequest();
            var accountID =acId;
            var token = _accessToken;
            var PositionId = positionId;
            var StopLoss =0.00;
            var TakeProfit = 0.00;
            if (stopLoss.HasValue)
                StopLoss = (double)stopLoss;
            if (takeProfit.HasValue)
                TakeProfit = (double)takeProfit;
            var msgFactory = new OpenApiMessagesFactory();
            var _msg = ProtoOAAmendPositionStopLossTakeProfitReq.CreateBuilder();
            _msg.SetAccountId(accountID);
            _msg.SetAccessToken(token);
            _msg.SetPositionId(PositionId);
            if (stopLoss.HasValue)
                _msg.SetStopLossPrice(StopLoss);
            if (takeProfit.HasValue)
                _msg.SetTakeProfitPrice(TakeProfit);
            var msg = msgFactory.CreateMessage((uint)_msg.PayloadType, _msg.Build().ToByteString(), PositionId.ToString());
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            var amend = OpenApiMessagesPresentation.ToString(protoMessage);
            List<string> data = new List<string>();
            data.Add(auth);
            data.Add(amend);
            return Json(new {data,data.Count});
        }

        private void SendPingRequest()
        {
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreatePingRequest(DateTime.Now.Ticks);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SubscribeForTradingEvents()
        {
            SendAuthorizationRequest();
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateSubscribeForTradingEventsRequest(89214, token);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void UnsubscribeForTradingEvents()
        {
            SendAuthorizationRequest();
            var accountID = "1960408";
            var token = _accessToken;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateUnsubscribeForTradingEventsRequest(Convert.ToInt32(accountID));
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SendGetAllSubscriptionsForTradingEventsRequest()
        {
            SendAuthorizationRequest();
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateAllSubscriptionsForTradingEventsRequest();
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private void SendGetAllSubscriptionsForSpotEventsRequest()
        {
            SendAuthorizationRequest();
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateGetAllSpotSubscriptionsRequest();
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        }


        private void TradingAccountDetails()
        {
                var accountID = "1960408";
                var token = _accessToken;

                var date = new DateTime(2017, 01, 12);
                var tickDataBid = TickData.GetTickData(_apiUrl, accountID, token, "EURUSD", TickData.TickDataType.Bid, date, 07, 13, 46, 07, 15, 26);
                var tickDataAsk = TickData.GetTickData(_apiUrl, accountID, token, "EURUSD", TickData.TickDataType.Ask, date, 07, 13, 46, 07, 15, 26);
                foreach (var td in tickDataBid)
                {
                    var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(td.Timestamp) / 1000d));
                    //chrTickData.Series[0].Points.Add(new DataPoint(dt.ToOADate(), Convert.ToDouble(td.Tick)));
                }

                foreach (var td in tickDataAsk)
                {
                    var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(td.Timestamp) / 1000d));
                    //chrTickData.Series[1].Points.Add(new DataPoint(dt.ToOADate(), Convert.ToDouble(td.Tick)));
                }
                //chrTickData.ChartAreas[0].AxisY.Minimum = chrTickData.Series[0].Points.Min(x => x.YValues[0]);
                //chrTickData.ChartAreas[0].AxisY.Maximum = chrTickData.Series[0].Points.Max(x => x.YValues[0]);

                var dateFrom = new DateTime(2017, 01, 11, 00, 00, 00);
                var dateTo = new DateTime(2017, 01, 13, 00, 00, 00);
                var trendBarH1 = TrendBar.GetTrendBar(_apiUrl, accountID, token, "EURUSD", TrendBar.TrendBarType.Hour, dateFrom, dateTo);

                dateFrom = new DateTime(2017, 01, 12, 23, 00, 00);
                dateTo = new DateTime(2017, 01, 13, 00, 00, 00);
                var trendBarM1 = TrendBar.GetTrendBar(_apiUrl, accountID, token, "EURUSD", TrendBar.TrendBarType.Minute, dateFrom, dateTo);

                //chrTrendChartH1.Series[0].Points.Clear();
                foreach (var tb in trendBarH1)
                {
                    var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(tb.Timestamp) / 1000d));
                    //chrTrendChartH1.Series[0].Points.Add(new DataPoint(dt.ToOADate(), new double[] { Convert.ToDouble(tb.High), Convert.ToDouble(tb.Low), Convert.ToDouble(tb.Open), Convert.ToDouble(tb.Close) }));
                }
                //chrTrendChartH1.ChartAreas[0].AxisY.Minimum = chrTrendChartH1.Series[0].Points.Min(x => x.YValues.Min());
                //chrTrendChartH1.ChartAreas[0].AxisY.Maximum = chrTrendChartH1.Series[0].Points.Max(x => x.YValues.Max());

                //chrTrendChartM1.Series[0].Points.Clear();
                foreach (var tb in trendBarM1)
                {
                    var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(tb.Timestamp) / 1000d));
                    //chrTrendChartM1.Series[0].Points.Add(new DataPoint(dt.ToOADate(), new double[] { Convert.ToDouble(tb.High), Convert.ToDouble(tb.Low), Convert.ToDouble(tb.Open), Convert.ToDouble(tb.Close) }));
                }
                //chrTrendChartM1.ChartAreas[0].AxisY.Minimum = chrTrendChartM1.Series[0].Points.Min(x => x.YValues.Min());
                //chrTrendChartM1.ChartAreas[0].AxisY.Maximum = chrTrendChartM1.Series[0].Points.Max(x => x.YValues.Max());
        }

        private string SendAuthorizationRequest()
        {
            AppIdentityUser _user = _identitycontext.Users.SingleOrDefault(x => x.UserName == "lee890720");
            _clientId = _user.ClientId;
            _clientSecret = _user.ClientSecret;
            _accessToken = _user.AccessToken;
            _refreshToken = _user.RefreshToken;
            var profile = Profile.GetProfile(_apiUrl, _accessToken);
            var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
            _tcpClient = new TcpClient(_apiHost, _apiPort);
            _apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            _apiSocket.AuthenticateAsClient(_apiHost);

            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateAuthorizationRequest(_clientId, _clientSecret);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            return OpenApiMessagesPresentation.ToString(protoMessage);
        }

        private byte[] Listen(SslStream apiSocket)
        {
            byte[] _length = new byte[sizeof(int)];
            bool cont = true;
            byte[] _message = new byte[0];
            while (cont)
            {
                int readBytes = 0;
                do
                {
                    readBytes += _apiSocket.Read(_length, readBytes, _length.Length - readBytes);
                } while (readBytes < _length.Length);

                int msgLength = BitConverter.ToInt32(_length.Reverse().ToArray(), 0);

                if (msgLength <= 0)
                    continue;
                cont = false;
                _message = new byte[msgLength];
                readBytes = 0;
                do
                {
                    readBytes += _apiSocket.Read(_message, readBytes, _message.Length - readBytes);
                } while (readBytes < msgLength);
            }

            return _message;
        }

        private void Transmit(ProtoMessage msg)
        {

            var msgByteArray = msg.ToByteArray();
            byte[] length = BitConverter.GetBytes(msgByteArray.Length).Reverse().ToArray();
            _apiSocket.Write(length);
            _apiSocket.Write(msgByteArray);
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
