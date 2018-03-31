using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Forex
{
    public partial class FrxCbotset
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public int Initvolume { get; set; }
        public int Tmr { get; set; }
        public double Brk { get; set; }
        public double Distance { get; set; }
        public bool Istrade { get; set; }
        public bool Isbreak { get; set; }
        public bool Isbrkfirst { get; set; }
        public int Result { get; set; }
        public int Average { get; set; }
        public double Magnify { get; set; }
        public double Sub { get; set; }
        public double? Cr { get; set; }
        public double? Ca { get; set; }
        public double? Sr { get; set; }
        public double? Sa { get; set; }
        public string Signal { get; set; }
        public string Alike { get; set; }
    }
}
