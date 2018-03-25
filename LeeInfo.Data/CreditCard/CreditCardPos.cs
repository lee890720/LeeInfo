using System;
using System.Collections.Generic;

namespace LeeInfo.Data.CreditCard
{
    public partial class CreditCardPos
    {
        public CreditCardPos()
        {
            CreditCardRecord = new HashSet<CreditCardRecord>();
        }

        public int Posid { get; set; }
        public int PersonId { get; set; }
        public string Posname { get; set; }
        public string Posnote { get; set; }

        public Person Person { get; set; }
        public ICollection<CreditCardRecord> CreditCardRecord { get; set; }
    }
}
