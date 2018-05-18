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
    [Authorize(Roles = "Admins,Trader")]
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

        public JsonResult GetPrice([FromBody]Params param)
        {
            var client = new RestClient(param.ApiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + param.AccountId.ToString() + "/symbols?oauth_token=" + param.AccessToken);
            var responseSymbols = client.Execute<Symbols>(request);
            return Json(JObject.Parse(responseSymbols.Content)); 
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
            var data = _context.FrxSymbol.OrderBy(x => x.SymbolId).ToList();
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
                        UnrNet = s.Sum(a => a.Swap + a.Commission*2 + a.Profit) / 100,
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

        #region Proto
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
                    var msg = msgFactory.CreateMarketOrderRequest(param.AccountId, param.AccessToken, p.SymbolName, tradeType, p.Volume, null, null, null, p.PositionId);
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
                    var msg = msgFactory.CreateMarketOrderRequest(param.AccountId, param.AccessToken, p.SymbolName, tradeType, p.Volume * 2, null, null, null, p.PositionId);
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
            var msg = msgFactory.CreateAmendPositionProtectionRequest(param.AccountId, param.AccessToken.ToString(), param.PositionId, param.StopLossPrice, param.TakeProfitPrice);
            Transmit(msg);
            byte[] _message = Listen(_apiSocket);
            var protoMessage = msgFactory.GetMessage(_message);
            data.Add(OpenApiMessagesPresentation.ToString(protoMessage));
            return Json(new { data });
        }

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
        #endregion
    }
}
