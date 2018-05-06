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
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }
        public int AccountNumber { get; set; }
        public string Password { get; set; }
        public string BrokerName { get; set; }
        public string Currency { get; set; }
        public DateTime TraderRegistrationTime { get; set; }
        public bool IsLive { get; set; }
        public double Balance { get; set; }
        public double Equity { get; set; }
        public double MarginUsed { get; set; }
        public double FreeMargin { get; set; }
        public double MarginLevel { get; set; }
        public double PreciseLeverage { get; set; }
        public double UnrealizedNetProfit { get; set; }
        public ICollection<FrxPosition> FrxPosition { get; set; }
        public ICollection<FrxHistory> FrxHistory { get; set; }
    }
}