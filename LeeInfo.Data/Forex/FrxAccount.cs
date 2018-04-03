using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_Account")]
    public partial class FrxAccount
    {
        public FrxAccount()
        {
            FrxPosition = new HashSet<FrxPosition>();
            FrxHistory = new HashSet<FrxHistory>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountPassword { get; set; }
        public string Platform { get; set; }
        public string UserName { get; set; }
        public ICollection<FrxPosition> FrxPosition { get; set; }
        public ICollection<FrxHistory> FrxHistory { get; set; }
    }
}