using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_Symbol")]
    public partial class FrxSymbol
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int SymbolId { get; set; }
        public string SymbolName { get; set; }
        public int Digits { get; set; }
        public int PipPosition { get; set; }
        public string MeasurementUnits { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public bool TradeEnabled { get; set; }
        public double TickSize { get; set; }
        public string Description { get; set; }
        public int MaxLeverage { get; set; }
        public double SwapLong { get; set; }
        public double SwapShort { get; set; }
        public string ThreeDaysSwaps { get; set; }
        public long MinOrderVolume { get; set; }
        public long MinOrderStep { get; set; }
        public long MaxOrderVolume { get; set; }
        public int AssetClass { get; set; }
        public double? LastBid { get; set; }
        public double? LastAsk { get; set; }
        public string TradingMode { get; set; }
    }
}
