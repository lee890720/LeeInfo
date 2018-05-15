using Connect_API.Accounts;
using Connect_API.Trading;
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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class ProtoController : Controller
    {
        private readonly AppIdentityDbContext _identitycontext;
        private UserManager<AppIdentityUser> _userManager;
        private readonly AppDbContext _context;

        private TcpClient _tcpClient = new TcpClient();
        private SslStream _apiSocket;

        public ProtoController(AppIdentityDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
        {
            _identitycontext = identitycontext;
            _userManager = usermgr;
            _context = context;
        }
        public IActionResult Index(int? acId)
        {
            ConnectAPI connect = ConnectAPI.GetConnectAPI(_identitycontext, _context, User.Identity.Name, acId);
            if (connect.AccountId == 0)
                return Redirect("/");

            var useraccounts = _identitycontext.AspNetUserForexAccount.Where(u => u.AppIdentityUserId == connect.UserId).ToList();
            var accounts = _context.FrxAccount.Where(x => useraccounts.SingleOrDefault(s => s.AccountNumber == x.AccountNumber && s.Password == x.Password) != null).ToList();
            return View(Tuple.Create<ConnectAPI, List<FrxAccount>>(connect, accounts));
        }

        public JsonResult GetAsk([FromBody]Params param)
        {
            DateTime date = DateTime.UtcNow;
            string dateString = String.Format("{0:0000}", date.Year) + String.Format("{0:00}", date.Month) + String.Format("{0:00}", date.Day);
            string timeFrom = String.Format("{0:00}", date.Hour) + String.Format("{0:00}", date.Minute) + String.Format("{0:00}", 0);
            string timeTo = String.Format("{0:00}", date.Hour) + String.Format("{0:00}", date.Minute) + String.Format("{0:00}", date.Second);
            var client = new RestClient(param.ApiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + param.AccountId + "/symbols/" + param.SymbolName + "/ask/?oauth_token=" + param.AccessToken + "&date=" + dateString + "&from=" + timeFrom + "&to=" + timeTo);
            var responseTickData= client.Execute<TickData>(request);
            return Json(JObject.Parse(responseTickData.Content));
        }

        public JsonResult GetBid([FromBody]Params param)
        {
            DateTime date = DateTime.UtcNow;
            string dateString = String.Format("{0:0000}", date.Year) + String.Format("{0:00}", date.Month) + String.Format("{0:00}", date.Day);
            string timeFrom = String.Format("{0:00}", date.Hour) + String.Format("{0:00}", date.Minute) + String.Format("{0:00}", 0);
            string timeTo = String.Format("{0:00}", date.Hour) + String.Format("{0:00}", date.Minute) + String.Format("{0:00}", date.Second);
            var client = new RestClient(param.ApiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + param.AccountId + "/symbols/" + param.SymbolName + "/bid/?oauth_token=" + param.AccessToken + "&date=" + dateString + "&from=" + timeFrom + "&to=" + timeTo);
            var responseTickData = client.Execute<TickData>(request);
            return Json(JObject.Parse(responseTickData.Content));
        }

        public JsonResult GetPosition([FromBody]Params param)
        {
            var client = new RestClient(param.ApiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + param.AccountId.ToString() + "/positions?oauth_token=" + param.AccessToken);
            var responsePosition = client.Execute<Position>(request);
            return Json(JObject.Parse(responsePosition.Content));
        }

        public JsonResult GetSymbol()
        {
            string[] bases = { "XAU", "XAG", "XBR", "XTI" };
            var data = _context.FrxSymbol.Where(x => (x.AssetClass == 1 || bases.Contains(x.BaseAsset)) && x.TradeEnabled).OrderBy(x => x.SymbolId).ToList();
            return Json(new { data, data.Count });
        }

        public JsonResult GetAccountInfo([FromBody]Params param)
        {
            var positions = Position.GetPositions(param.ApiUrl, param.AccountId.ToString(), param.AccessToken);
            var accountinfo = new
            {
                Balance = param.Balance,
                Equity = (double?)param.Balance,
                UnrNet = (double?)0.00,
                MarginUsed =0.00,
                FreeMargin = (double?)param.Balance,
                MarginLevel = (double?)0.00,
            };
            if (positions.Count != 0)
            {
                var posgroup = positions.GroupBy(g => new { g.SymbolName })
                    .Select(s => new
                    {
                        Symbol = s.Key.SymbolName,
                        Margin = s.Where(b => b.TradeSide == "BUY").Sum(a => a.Volume / 100 * a.MarginRate / param.PreciseLeverage)
                        > s.Where(b => b.TradeSide == "SELL").Sum(a => a.Volume / 100 * a.MarginRate / param.PreciseLeverage)
                        ? s.Where(b => b.TradeSide == "BUY").Sum(a => a.Volume / 100 * a.MarginRate / param.PreciseLeverage)
                        : s.Where(b => b.TradeSide == "SELL").Sum(a => a.Volume / 100 * a.MarginRate / param.PreciseLeverage),
                        UnrNet = s.Sum(a => a.Swap + a.Commission + a.Profit) / 100,
                    }).ToList();
                accountinfo = new
                {
                    Balance = param.Balance,
                    Equity = param.Balance + posgroup.Sum(s => s.UnrNet),
                    UnrNet = posgroup.Sum(s => s.UnrNet),
                    MarginUsed = posgroup.Sum(s => s.Margin),
                    FreeMargin = param.Balance + posgroup.Sum(s => s.UnrNet) - posgroup.Sum(s => s.Margin),
                    MarginLevel = (param.Balance + posgroup.Sum(s => s.UnrNet)) / posgroup.Sum(s => s.Margin) * 100,
                };
            }
            return Json(new { accountinfo, positions });
        }

        //#region Proto
        public JsonResult SendMarketOrder([FromBody]Params param)
        {
            _tcpClient = new TcpClient(param.ApiHost, param.ApiPort); ;
            _apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            _apiSocket.AuthenticateAsClient(param.ApiHost);
            SendAuthorizationRequest(param);
            List<string> data = new List<string>();

            ProtoTradeSide tradeType = new ProtoTradeSide();
            if (param.TradeSide.ToUpper() == "BUY")
                tradeType = ProtoTradeSide.BUY;
            if (param.TradeSide.ToUpper() == "SELL")
                tradeType = ProtoTradeSide.SELL;
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateMarketOrderRequest(param.AccountId, param.AccessToken, param.SymbolName, tradeType, param.Volume * 100,
                param.StopLossInPips, param.TakeProfitInPips, param.Comment);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            data.Add(OpenApiMessagesPresentation.ToString(protoMessage));

            Thread.Sleep(1000);
            _message = Listen(_apiSocket);
            protoMessage = msgFactory.GetMessage(_message);
            data.Add(OpenApiMessagesPresentation.ToString(protoMessage));
            return Json(new { data });
        }

        public JsonResult SendResetPosition([FromBody]Params param)
        {
            _tcpClient = new TcpClient(param.ApiHost, param.ApiPort); ;
            _apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            _apiSocket.AuthenticateAsClient(param.ApiHost);
            SendAuthorizationRequest(param);
            List<string> data = new List<string>();
            var msgFactory = new OpenApiMessagesFactory();

            if (param.SelectedType == "double")
            {
                foreach (var p in param.SelectedPositions)
                {
                    ProtoTradeSide tradeType = new ProtoTradeSide();
                    if (p.TradeSide.ToUpper() == "BUY")
                        tradeType = ProtoTradeSide.BUY;
                    if (p.TradeSide.ToUpper() == "SELL")
                        tradeType = ProtoTradeSide.SELL;
                    var msg = msgFactory.CreateMarketOrderRequest(param.AccountId, param.AccessToken, p.SymbolName, tradeType, p.Volume,null, null,null ,p.PositionId);
                    Transmit(msg);
                }
            }

            if (param.SelectedType == "reverse")
            {
                foreach (var p in param.SelectedPositions)
                {
                    ProtoTradeSide tradeType = new ProtoTradeSide();
                    if (p.TradeSide.ToUpper() == "BUY")
                        tradeType = ProtoTradeSide.SELL;
                    if (p.TradeSide.ToUpper() == "SELL")
                        tradeType = ProtoTradeSide.BUY;
                    var msg = msgFactory.CreateMarketOrderRequest(param.AccountId, param.AccessToken, p.SymbolName, tradeType, p.Volume*2,null, null,null, p.PositionId);
                    Transmit(msg);
                }
            }

            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            data.Add(OpenApiMessagesPresentation.ToString(protoMessage));

            Thread.Sleep(1000);
            _message = Listen(_apiSocket);
            protoMessage = msgFactory.GetMessage(_message);
            data.Add(OpenApiMessagesPresentation.ToString(protoMessage));
            return Json(new { data });
        }

        public JsonResult SendClosePosition([FromBody]Params param)
        {
            _tcpClient = new TcpClient(param.ApiHost, param.ApiPort); ;
            _apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            _apiSocket.AuthenticateAsClient(param.ApiHost);
            SendAuthorizationRequest(param);
            List<string> data = new List<string>();

            var msgFactory = new OpenApiMessagesFactory();

            if (!string.IsNullOrEmpty(param.SelectedType))
            {
                foreach (var p in param.SelectedPositions)
                {
                    var msg = msgFactory.CreateClosePositionRequest(param.AccountId, param.AccessToken, p.PositionId, p.Volume);
                    Transmit(msg);
                }
            }
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            data.Add(OpenApiMessagesPresentation.ToString(protoMessage));
            return Json(new { data });
        }

        public JsonResult SendAmendPosition([FromBody]Params param)
        {
            _tcpClient = new TcpClient(param.ApiHost, param.ApiPort); ;
            _apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            _apiSocket.AuthenticateAsClient(param.ApiHost);
            SendAuthorizationRequest(param);
            List<string> data = new List<string>();

            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateAmendPositionProtectionRequest(param.AccountId,param.AccessToken.ToString(),param.PositionId,param.StopLossPrice,param.TakeProfitPrice);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            data.Add(OpenApiMessagesPresentation.ToString(protoMessage));
            return Json(new { data });
        }

        //private void SendLimitOrderRequest()
        //{
        //    SendAuthorizationRequest();
        //    var accountID = "1960408";
        //    var token = _accessToken;
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var msg = msgFactory.CreateLimitOrderRequest(Convert.ToInt32(accountID), token, "EURUSD", ProtoTradeSide.BUY, 200000, 1.09);
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        //}

        //private void SendAmendLimitOrderRequest()
        //{
        //    SendAuthorizationRequest();
        //    var accountID = "1960408";
        //    var token = _accessToken;
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var msg = msgFactory.CreateAmendLimitOrderRequest(Convert.ToInt32(accountID), token, 69212987, 1.10);
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        //}

        //private void SendStopOrderRequest()
        //{
        //    SendAuthorizationRequest();
        //    var accountID = "1960408";
        //    var token = _accessToken;
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var msg = msgFactory.CreateStopOrderRequest(Convert.ToInt32(accountID), token, "EURUSD", ProtoTradeSide.BUY, 400000, 1.3);
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        //}

        //private void SubscribeForSpotsRequest()
        //{
        //    SendAuthorizationRequest();
        //    var accountID = "1960408";
        //    var token = _accessToken;
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var msg = msgFactory.CreateSubscribeForSpotsRequest(Convert.ToInt32(accountID), token, "EURUSD");
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        //}

        //private void RefreshToken()
        //{
        //    var token = _refreshToken;
        //    var newToken = AccessToken.RefreshAccessToken(_connectUrl, token, _clientId, _clientSecret);

        //}

        //public JsonResult AmendPosition(int acId, int positionId, double? stopLoss, double? takeProfit)
        //{
        //    AppIdentityUser _user = _identitycontext.Users.SingleOrDefault(x => x.UserName == "lee890720");
        //    _clientId = _user.ClientId;
        //    _clientSecret = _user.ClientSecret;
        //    _accessToken = _user.AccessToken;
        //    _refreshToken = _user.RefreshToken;
        //    var auth = SendAuthorizationRequest();
        //    var accountID = acId;
        //    var token = _accessToken;
        //    var PositionId = positionId;
        //    var StopLoss = 0.00;
        //    var TakeProfit = 0.00;
        //    if (stopLoss.HasValue)
        //        StopLoss = (double)stopLoss;
        //    if (takeProfit.HasValue)
        //        TakeProfit = (double)takeProfit;
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var _msg = ProtoOAAmendPositionStopLossTakeProfitReq.CreateBuilder();
        //    _msg.SetAccountId(accountID);
        //    _msg.SetAccessToken(token);
        //    _msg.SetPositionId(PositionId);
        //    if (stopLoss.HasValue)
        //        _msg.SetStopLossPrice(StopLoss);
        //    if (takeProfit.HasValue)
        //        _msg.SetTakeProfitPrice(TakeProfit);
        //    var msg = msgFactory.CreateMessage((uint)_msg.PayloadType, _msg.Build().ToByteString(), PositionId.ToString());
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    var amend = OpenApiMessagesPresentation.ToString(protoMessage);
        //    List<string> data = new List<string>();
        //    data.Add(auth);
        //    data.Add(amend);
        //    return Json(new { data, data.Count });
        //}



        //private void UnsubscribeForTradingEvents()
        //{
        //    SendAuthorizationRequest();
        //    var accountID = "1960408";
        //    var token = _accessToken;
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var msg = msgFactory.CreateUnsubscribeForTradingEventsRequest(Convert.ToInt32(accountID));
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        //}

        //private void SendGetAllSubscriptionsForTradingEventsRequest()
        //{
        //    SendAuthorizationRequest();
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var msg = msgFactory.CreateAllSubscriptionsForTradingEventsRequest();
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        //}

        //private void SendGetAllSubscriptionsForSpotEventsRequest()
        //{
        //    SendAuthorizationRequest();
        //    var msgFactory = new OpenApiMessagesFactory();
        //    var msg = msgFactory.CreateGetAllSpotSubscriptionsRequest();
        //    Transmit(msg);
        //    byte[] _message = Listen(_apiSocket);
        //    var protoMessage = msgFactory.GetMessage(_message);
        //    //lblResponse.Text = OpenApiMessagesPresentation.ToString(protoMessage);
        //}


        //private void TradingAccountDetails()
        //{
        //    var accountID = "1960408";
        //    var token = _accessToken;

        //    var date = new DateTime(2017, 01, 12);
        //    var tickDataBid = TickData.GetTickData(_apiUrl, accountID, token, "EURUSD", TickData.TickDataType.Bid, date, 07, 13, 46, 07, 15, 26);
        //    var tickDataAsk = TickData.GetTickData(_apiUrl, accountID, token, "EURUSD", TickData.TickDataType.Ask, date, 07, 13, 46, 07, 15, 26);
        //    foreach (var td in tickDataBid)
        //    {
        //        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(td.Timestamp) / 1000d));
        //        //chrTickData.Series[0].Points.Add(new DataPoint(dt.ToOADate(), Convert.ToDouble(td.Tick)));
        //    }

        //    foreach (var td in tickDataAsk)
        //    {
        //        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(td.Timestamp) / 1000d));
        //        //chrTickData.Series[1].Points.Add(new DataPoint(dt.ToOADate(), Convert.ToDouble(td.Tick)));
        //    }
        //    //chrTickData.ChartAreas[0].AxisY.Minimum = chrTickData.Series[0].Points.Min(x => x.YValues[0]);
        //    //chrTickData.ChartAreas[0].AxisY.Maximum = chrTickData.Series[0].Points.Max(x => x.YValues[0]);

        //    var dateFrom = new DateTime(2017, 01, 11, 00, 00, 00);
        //    var dateTo = new DateTime(2017, 01, 13, 00, 00, 00);
        //    var trendBarH1 = TrendBar.GetTrendBar(_apiUrl, accountID, token, "EURUSD", TrendBar.TrendBarType.Hour, dateFrom, dateTo);

        //    dateFrom = new DateTime(2017, 01, 12, 23, 00, 00);
        //    dateTo = new DateTime(2017, 01, 13, 00, 00, 00);
        //    var trendBarM1 = TrendBar.GetTrendBar(_apiUrl, accountID, token, "EURUSD", TrendBar.TrendBarType.Minute, dateFrom, dateTo);

        //    //chrTrendChartH1.Series[0].Points.Clear();
        //    foreach (var tb in trendBarH1)
        //    {
        //        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(tb.Timestamp) / 1000d));
        //        //chrTrendChartH1.Series[0].Points.Add(new DataPoint(dt.ToOADate(), new double[] { Convert.ToDouble(tb.High), Convert.ToDouble(tb.Low), Convert.ToDouble(tb.Open), Convert.ToDouble(tb.Close) }));
        //    }
        //    //chrTrendChartH1.ChartAreas[0].AxisY.Minimum = chrTrendChartH1.Series[0].Points.Min(x => x.YValues.Min());
        //    //chrTrendChartH1.ChartAreas[0].AxisY.Maximum = chrTrendChartH1.Series[0].Points.Max(x => x.YValues.Max());

        //    //chrTrendChartM1.Series[0].Points.Clear();
        //    foreach (var tb in trendBarM1)
        //    {
        //        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(tb.Timestamp) / 1000d));
        //        //chrTrendChartM1.Series[0].Points.Add(new DataPoint(dt.ToOADate(), new double[] { Convert.ToDouble(tb.High), Convert.ToDouble(tb.Low), Convert.ToDouble(tb.Open), Convert.ToDouble(tb.Close) }));
        //    }
        //    //chrTrendChartM1.ChartAreas[0].AxisY.Minimum = chrTrendChartM1.Series[0].Points.Min(x => x.YValues.Min());
        //    //chrTrendChartM1.ChartAreas[0].AxisY.Maximum = chrTrendChartM1.Series[0].Points.Max(x => x.YValues.Max());
        //}
        private void SendAuthorizationRequest(Params param)
        {
            var msgFactory = new OpenApiMessagesFactory();
            var msg = msgFactory.CreateAuthorizationRequest(param.ClientId, param.ClientSecret);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
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
