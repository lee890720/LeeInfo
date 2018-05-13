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
using System.Threading;
using LeeInfo.Data.Forex;
using LeeInfo.Lib;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;

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
            if (_user.ConnectAPI)
            {
                _clientId = _user.ClientId;
                _clientSecret = _user.ClientSecret;
                _accessToken = _user.AccessToken;
                _refreshToken = _user.RefreshToken;
            }
            else
            {
                _clientId = _admin.ClientId;
                _clientSecret = _admin.ClientSecret;
                _accessToken = _admin.AccessToken;
                _refreshToken = _admin.RefreshToken;
            }
            //var accounts = TradingAccount.GetTradingAccounts(_apiUrl, _accessToken);
            //_tcpClient = new TcpClient(_apiHost, _apiPort);
            //_apiSocket = new SslStream(_tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            //_apiSocket.AuthenticateAsClient(_apiHost);
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
            return View(Tuple.Create<FrxAccount, List<FrxAccount>>(frxaccount, frxaccounts));
        }

        public JsonResult GetPosition([FromBody]Params param)
        {
            var client = new RestClient(_apiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + param.AcId.ToString() + "/positions?oauth_token=" + _accessToken);
            var responsePosition = client.Execute<Position>(request);
            return Json(JObject.Parse(responsePosition.Content));
        }

        public JsonResult GetSymbol()
        {
            string[] bases = { "XAU", "XAG", "XBR", "XTI" };
            var data = _context.FrxSymbol.Where(x => (x.AssetClass == 1 || bases.Contains(x.BaseAsset)) && x.TradeEnabled).OrderBy(x => x.SymbolId).ToList();
            return Json(new { data, data.Count });
        }

        #region Proto
        private void SendMarketOrderRequest(int acId, string accesstoken, long accountId, string accessToken, string symbolName, ProtoTradeSide tradeSide, long volume)
        {
            var auth = SendAuthorizationRequest();
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
            var token = _refreshToken;
            var newToken = AccessToken.RefreshAccessToken(_connectUrl, token, _clientId, _clientSecret);

        }

        public JsonResult AmendPosition(int acId, int positionId, double? stopLoss, double? takeProfit)
        {
            AppIdentityUser _user = _identitycontext.Users.SingleOrDefault(x => x.UserName == "lee890720");
            _clientId = _user.ClientId;
            _clientSecret = _user.ClientSecret;
            _accessToken = _user.AccessToken;
            _refreshToken = _user.RefreshToken;
            var auth = SendAuthorizationRequest();
            var accountID = acId;
            var token = _accessToken;
            var PositionId = positionId;
            var StopLoss = 0.00;
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
            return Json(new { data, data.Count });
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
        #endregion
    }

    public class Params
    {
        public int AcId { get; set; }
    }
}
