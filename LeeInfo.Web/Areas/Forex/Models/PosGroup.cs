using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Web.Areas.Forex.Models
{
    public class PosGroup
    {
        public int SymbolId { get; set; }
        public string SymbolName { get; set; }
        public string TradeSide { get; set; }
        public double EntryPrice { get; set; }
        public long Volume { get; set; }
        public double? Lot { get; set; }
        public double Swap { get; set; }
        public double Pips { get; set; }
        public double? Profit { get; set; }
        public double Gain { get; set; }
        public int PipPosition { get; set; }
        public int AssetClass { get; set; }
        public double MinOrderLot { get; set; }
        public long MinOrderVolume { get; set; }
    }
}
