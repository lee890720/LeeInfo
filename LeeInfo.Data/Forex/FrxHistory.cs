using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_History")]
    public partial class FrxHistory
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int ClosingDealId { get; set; }
        public int AccountId { get; set; }
        public string SymbolCode { get; set; }
        public TradeType TradeType { get; set; }
        public long Volume { get; set; }
        public double Quantity { get; set; }
        public string Label { get; set; }
        public string Comment { get; set; }
        public double EntryPrice { get; set; }
        public double ClosingPrice { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ClosingTime { get; set; }
        public double Commissions { get; set; }
        public double Swap { get; set; }
        public double GrossProfit { get; set; }
        public double NetProfit { get; set; }
        public double Pips { get; set; }
        public double Balance { get; set; }
        public int PositionId { get; set; }
        public double BaseToUSDConversionRate { get; set; }
        public double MarginRate { get; set; }
        public int BalanceVersion { get; set; }
        public double? QuoteToDepositConversionRate { get; set; }
        public double? ClosedToDepoitConversionRate { get; set; }
        public double Equity { get; set; }
        public double Roi { get; set; }
        public double EquityBaseRoi { get; set; }
        public int? Digits { get; set; }

        public FrxAccount FrxAccount { get; set; }
    }
}
