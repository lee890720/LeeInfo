using System;
using System.Collections.Generic;

namespace LeeInfo.Data.CreditCard
{
    public partial class CreditCardRecord
    {
        public int RecordId { get; set; }
        public int CreditCardId { get; set; }
        public DateTime RecordDate { get; set; }
        public int? Posid { get; set; }
        public double? Deposit { get; set; }
        public double? Expend { get; set; }

        public CreditCardAccount CreditCard { get; set; }
        public CreditCardPos Pos { get; set; }
    }
}
