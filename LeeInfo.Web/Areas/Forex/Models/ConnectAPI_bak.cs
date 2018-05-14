using Connect_API.Accounts;
using LeeInfo.Data;
using LeeInfo.Data.AppIdentity;
using LeeInfo.Data.Forex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Web.Areas.Forex.Models
{
    public class ConnectAPI_bak
    {
        public int AccountId { get; set; }
        public double Balance { get; set; }
        public double PreciseLeverage { get; set; }
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ConnectUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ApiHost { get; set; }
        public int ApiPort { get; set; }

        public static ConnectAPI GetConnectAPI(AppIdentityDbContext identitycontext, AppDbContext context, string userName, int? acId)
        {
            ConnectAPI connectAPI = new ConnectAPI();
            AppIdentityUser user = identitycontext.Users.SingleOrDefault(x => x.UserName == userName);
            AppIdentityUser admin = identitycontext.Users.SingleOrDefault(x => x.UserName == "lee890720");
            if (user.ConnectAPI)
            {
                connectAPI.AccountId = 0;
                connectAPI.Balance = 0;
                connectAPI.PreciseLeverage = 1;
                connectAPI.UserId = user.Id;
                connectAPI.ClientId = user.ClientId;
                connectAPI.ClientSecret = user.ClientSecret;
                connectAPI.AccessToken = user.AccessToken;
                connectAPI.RefreshToken = user.RefreshToken;
                connectAPI.ConnectUrl = user.ConnectUrl;
                connectAPI.ApiUrl = user.ApiUrl;
                connectAPI.ApiHost = user.ApiHost;
                connectAPI.ApiPort = user.ApiPort;
            }
            else
            {
                connectAPI.AccountId = 0;
                connectAPI.Balance = 0;
                connectAPI.PreciseLeverage = 1;
                connectAPI.UserId = user.Id;
                connectAPI.ClientId = admin.ClientId;
                connectAPI.ClientSecret = admin.ClientSecret;
                connectAPI.AccessToken = admin.AccessToken;
                connectAPI.RefreshToken = admin.RefreshToken;
                connectAPI.ConnectUrl = admin.ConnectUrl;
                connectAPI.ApiUrl = admin.ApiUrl;
                connectAPI.ApiHost = admin.ApiHost;
                connectAPI.ApiPort = admin.ApiPort;
            }
            #region GetAccount
            var useraccounts = identitycontext.AspNetUserForexAccount.Where(u => u.AppIdentityUserId == user.Id).ToList();
            var frxaccounts = context.FrxAccount.Where(x => useraccounts.SingleOrDefault(s => s.AccountNumber == x.AccountNumber && s.Password == x.Password) != null).ToList();
            if (frxaccounts.Count == 0)
                connectAPI.AccountId = 0;
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
            var account = TradingAccount.GetTradingAccounts(connectAPI.ApiUrl, connectAPI.AccessToken).SingleOrDefault(x => x.AccountId == frxaccount.AccountId);
            if (account != null)
            {
                frxaccount.Balance = account.Balance / 100;
                context.Update(frxaccount);
                context.SaveChanges();
            }
            else
                connectAPI.AccountId = 0;
            #endregion
            connectAPI.AccountId = frxaccount.AccountId;
            connectAPI.Balance = frxaccount.Balance;
            connectAPI.PreciseLeverage = frxaccount.PreciseLeverage;
            return connectAPI;
        }
    }
}
