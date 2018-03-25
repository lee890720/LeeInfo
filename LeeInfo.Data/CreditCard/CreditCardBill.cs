using System;
using System.Collections.Generic;

namespace LeeInfo.Data.CreditCard
{
    public partial class CreditCardBill
    {
        public int BillId { get; set; }
        public int CreditCardId { get; set; }
        public DateTime BillDate { get; set; }
        public double BillAmount { get; set; }

        public CreditCardAccount CreditCard { get; set; }
    }
}
