using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Web.Areas.Forex.Models
{
    public class Params:ConnectAPI
    {
        public string SymbolName { get; set; }
        public string TradeSide { get; set; }
        public long Volume { get; set; }
        public double? StopLossPrice { get; set; }
        public int? StopLossInPips { get; set; }
        public double? TakeProfitPrice { get; set; }
        public int? TakeProfitInPips { get; set; }
        public long? RangePips { get; set; }
        public string Comment { get; set; }
        public long PositionId { get; set; }
        public string SelectedType { get; set; }
        public List<SelectedPosition> SelectedPositions { get; set; }
    }

    public class SelectedPosition
    {
        public long PositionId { get; set; }
        public string SymbolName { get; set; }
        public long Volume { get; set; }
        public string TradeSide { get; set; }
        public double NetProfit { get; set; }
    }
}
