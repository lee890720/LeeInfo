using System;
using System.Collections.Generic;

namespace LeeInfo.Data.CreditCard
{
    public partial class CreditCardAccount
    {
        public CreditCardAccount()
        {
            CreditCardBill = new HashSet<CreditCardBill>();
            CreditCardRecord = new HashSet<CreditCardRecord>();
        }

        public int CreditCardId { get; set; }
        public int PersonId { get; set; }
        public int IssuingBank { get; set; }
        public string CreditCardNumber { get; set; }
        public double Limit { get; set; }
        public double? Temporary { get; set; }
        public DateTime? TempDate { get; set; }
        public DateTime AccountBill { get; set; }
        public DateTime RepaymentDate { get; set; }
        public double BillAmount { get; set; }
        public string ValidThru { get; set; }
        public string Cvv { get; set; }
        public string TransactionPw { get; set; }
        public string InquriyPw { get; set; }
        public string OnlineBankingPw { get; set; }
        public double HasPayment { get; set; }
        public double PrePayment { get; set; }

        public Person Person { get; set; }
        public ICollection<CreditCardBill> CreditCardBill { get; set; }
        public ICollection<CreditCardRecord> CreditCardRecord { get; set; }
    }
}
