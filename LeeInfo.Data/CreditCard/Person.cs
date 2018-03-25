using System;
using System.Collections.Generic;

namespace LeeInfo.Data.CreditCard
{
    public partial class Person
    {
        public Person()
        {
            CreditCardAccount = new HashSet<CreditCardAccount>();
            CreditCardPos = new HashSet<CreditCardPos>();
            PersonDebt = new HashSet<PersonDebt>();
        }

        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string IdcardNumber { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PersonNote { get; set; }

        public ICollection<CreditCardAccount> CreditCardAccount { get; set; }
        public ICollection<CreditCardPos> CreditCardPos { get; set; }
        public ICollection<PersonDebt> PersonDebt { get; set; }
    }
}
