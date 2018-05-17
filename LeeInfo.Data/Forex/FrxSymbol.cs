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
        [Display(Name = "品种ID")]
        public int SymbolId { get; set; }
        [Display(Name = "品种")]
        public string SymbolName { get; set; }
        [Display(Name = "报价精度")]
        public int Digits { get; set; }
        [Display(Name = "最小点精度")]
        public int PipPosition { get; set; }
        [Display(Name = "计量单位")]
        public string MeasurementUnits { get; set; }
        [Display(Name = "基础货币")]
        public string BaseAsset { get; set; }
        [Display(Name = "报价货币")]
        public string QuoteAsset { get; set; }
        [Display(Name = "是否可交易")]
        public bool TradeEnabled { get; set; }
        [Display(Name = "Tick精度")]
        public double TickSize { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
        public int MaxLeverage { get; set; }
        [Display(Name = "多单利息")]
        public double SwapLong { get; set; }
        [Display(Name = "空单利息")]
        public double SwapShort { get; set; }
        [Display(Name = "三日利息时间")]
        public string ThreeDaysSwaps { get; set; }
        [Display(Name = "最小交易数量")]
        public long MinOrderVolume { get; set; }
        [Display(Name = "数量最小尺度")]
        public long MinOrderStep { get; set; }
        [Display(Name = "最大交易数量")]
        public long MaxOrderVolume { get; set; }
        [Display(Name = "货币分类")]
        public int AssetClass { get; set; }
        [Display(Name = "最后卖价")]
        public double? LastBid { get; set; }
        [Display(Name = "最后买价")]
        public double? LastAsk { get; set; }
        public string TradingMode { get; set; }
    }
}
