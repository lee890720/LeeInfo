using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_Account")]
    public partial class FrxAccount
    {
        public FrxAccount()
        {
            FrxPosition = new HashSet<FrxPosition>();
            FrxHistory = new HashSet<FrxHistory>();
            FrxCashflow = new HashSet<FrxCashflow>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }
        public int AccountNumber { get; set; }
        public string Password { get; set; }
        public string BrokerName { get; set; }
        public string Currency { get; set; }
        public long TraderRegistrationTimestamp { get; set; }
        public bool IsLive { get; set; }
        public double Balance { get; set; }
        public double Equity { get; set; }
        public double MarginUsed { get; set; }
        public double FreeMargin { get; set; }
        public double MarginLevel { get; set; }
        public double PreciseLeverage { get; set; }
        public double UnrealizedNetProfit { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ConnectUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ApiHost { get; set; }
        public int ApiPort { get; set; }
        public ICollection<FrxPosition> FrxPosition { get; set; }
        public ICollection<FrxHistory> FrxHistory { get; set; }
        public ICollection<FrxCashflow> FrxCashflow { get; set; }
    }
}