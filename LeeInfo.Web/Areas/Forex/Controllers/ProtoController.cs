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

namespace LeeInfo.Web.Areas.Forex.Controllers
{
    [Area("Forex")]
    [Authorize(Roles = "Admins,Forex")]
    public class ProtoController : Controller
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

        public ProtoController(AppDbContext identitycontext, UserManager<AppIdentityUser> usermgr, AppDbContext context)
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
            return View();
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