using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Web.Areas.Forex.Models
{
    public class Analyze
    {
    }

    public class XData
    {
        public XData(string xn, DateTime xt)
        {
            XName = xn;
            XTime = xt;
        }
        public string XName { get; set; }
        public DateTime XTime { get; set; }
    }

    public class MonthBaseData
    {
        public XData XData { get; set; }
        public List<BuySellData> BuySellData { get; set; }
        public double Net { get; set; }
        public double Gain { get; set; }
        public double Swap { get; set; }
        public double Pips { get; set; }
        public double Lots { get; set; }
    }

    public class BuySellData
    {
        public string SymbolCode { get; set; }
        public int Count { get; set; }
        public double Lots { get; set; }
        public double Pips { get; set; }
        public double Profit { get; set; }
        public int BuyCount { get; set; }
        public double BuyLots { get; set; }
        public double BuyPips { get; set; }
        public double BuyProfit { get; set; }
        public double BuyRate { get; set; }
        public int SellCount { get; set; }
        public double SellLots { get; set; }
        public double SellPips { get; set; }
        public double SellProfit { get; set; }
        public double SellRate { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public double WinRate { get; set; }
        public double LossRate { get; set; }
    }

    public class AccountInfo
    {
        public double Gain { get; set; }
        public double AbsGain { get; set; }
        public double MaxDraw { get; set; }
        public DateTime MaxDrawTime { get; set; }
        public double Deposit { get; set; }
        public double Withdraw { get; set; }
        public double Balance { get; set; }
        public double Equity { get; set; }
        public double MaxBalance { get; set; }
        public DateTime MaxBalanceTime { get; set; }
        public double TotalProfit { get; set; }
        public double TotalSwap { get; set; }
        public DateTime LastTradeTime { get; set; }
        public DateTime RigistrationTime { get; set; }
    }
}
