using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_Cashflow")]
    public partial class FrxCashflow
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Type { get; set; }
        public double Delta { get; set; }
        public double Balance { get; set; }
        public long BalanceVersion { get; set; }
        public long ChangeTimestamp { get; set; }
        public double Equity { get; set; }

        public FrxAccount FrxAccount { get; set; }
    }
}
