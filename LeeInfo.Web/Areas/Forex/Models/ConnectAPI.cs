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
    public class ConnectAPI:FrxAccount
    {
        public string UserId { get; set; }

        public static ConnectAPI GetConnectAPI(AppIdentityDbContext identitycontext, AppDbContext context, string userName, int? acId)
        {
            ConnectAPI connectAPI = new ConnectAPI();
            var useraccounts = identitycontext.AspNetUserForexAccount.Where(u => u.AppIdentityUser.UserName == userName).ToList();
            var frxaccounts = new List<FrxAccount>();
            if (useraccounts.Count != 0)
            {
                frxaccounts = context.FrxAccount.Where(x => useraccounts.SingleOrDefault(s => s.AccountNumber == x.AccountNumber && s.Password == x.Password) != null).ToList();
            }
            if (frxaccounts.Count == 0)
            {
                connectAPI.AccountId = 0;
                return connectAPI;
            }
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
            var account = TradingAccount.GetTradingAccounts(frxaccount.ApiUrl, frxaccount.AccessToken).SingleOrDefault(x => x.AccountId == frxaccount.AccountId);
            if (account != null)
            {
                frxaccount.Balance = account.Balance / 100;
                //context.Update(frxaccount);
                //context.SaveChanges();
            }
            else
            {
                connectAPI.AccountId = 0;
                return connectAPI;
            }
            connectAPI.UserId = useraccounts[0].AppIdentityUserId;
            connectAPI.AccountId = frxaccount.AccountId;
            connectAPI.Balance = frxaccount.Balance;
            connectAPI.PreciseLeverage = frxaccount.PreciseLeverage;
            connectAPI.ClientId = frxaccount.ClientId;
            connectAPI.ClientSecret = frxaccount.ClientSecret;
            connectAPI.AccessToken = frxaccount.AccessToken;
            connectAPI.RefreshToken = frxaccount.RefreshToken;
            connectAPI.ConnectUrl = frxaccount.ConnectUrl;
            connectAPI.ApiUrl = frxaccount.ApiUrl;
            connectAPI.ApiHost = frxaccount.ApiHost;
            connectAPI.ApiPort = frxaccount.ApiPort;
            connectAPI.TraderRegistrationTime = frxaccount.TraderRegistrationTime;
            return connectAPI;
        }
    }
}
