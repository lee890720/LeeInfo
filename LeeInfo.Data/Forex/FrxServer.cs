using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_Server")]
    public partial class FrxServer
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ServerName { get; set;  }
        public DateTime ServerTime { get; set; }
        public int AccountNumber { get; set; }
    }
}
