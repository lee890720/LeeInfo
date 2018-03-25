using System;
using System.Collections.Generic;

namespace LeeInfo.Data.CreditCard
{
    public partial class PersonDebt
    {
        public int DebtId { get; set; }
        public int PersonId { get; set; }
        public string DebtTitle { get; set; }
        public DateTime RepaymentDate { get; set; }
        public double BillAmount { get; set; }
        public string DebtNote { get; set; }
        public double CurrentAmount { get; set; }
        public double InterestRate { get; set; }

        public Person Person { get; set; }
    }
}
