using System;
using System.Collections.Generic;

namespace LeeInfo.Data.CreditCard
{
    public partial class CbotSet
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public double Initvolume { get; set; }
        public int Tmr { get; set; }
        public double Brk { get; set; }
        public double Distance { get; set; }
        public bool Istrade { get; set; }
        public bool Isbreak { get; set; }
        public int Resultperiods { get; set; }
        public int Averageperiods { get; set; }
        public double Magnify { get; set; }
        public double Sub { get; set; }
        public int? Cr { get; set; }
        public int? Ca { get; set; }
        public int? Sr { get; set; }
        public int? Sa { get; set; }
        public string Signal { get; set; }
        public bool Breakfirst { get; set; }
        public double? Slippage { get; set; }
        public string Alike { get; set; }
    }
}
