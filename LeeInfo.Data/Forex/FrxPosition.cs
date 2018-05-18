using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_Position")]
    public partial class FrxPosition
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string SymbolCode { get; set; }
        public TradeType TradeType { get; set; }
        public long Volume { get; set; }
        public double Quantity { get; set; }
        public string Label { get; set; }
        public string Comment { get; set; }
        public double EntryPrice { get; set; }
        public double CurrentPrice { get; set; }
        public long EntryTimestamp { get; set; }
        public double? TakeProfit { get; set; }
        public double? StopLoss { get; set; }
        public double Commissions { get; set; }
        public double Margin { get; set; }
        public double Swap { get; set; }
        public double GrossProfit { get; set; }
        public double NetProfit { get; set; }
        public double Pips { get; set; }
        public string Channel { get; set; }
        public double MarginRate { get; set; }
        public int? Digits { get; set; }

        public FrxAccount FrxAccount { get; set; }
    }
    public enum TradeType
    {
        Buy = 0,
        Sell = 1
    }
}
