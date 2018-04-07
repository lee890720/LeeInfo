using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.CreditCard
{
    [Table("Ccd_Record")]
    public partial class CcdRecord
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "记录ID")]
        public int RecordId { get; set; }
        [Required]
        [Display(Name = "信用卡ID")]
        public int CreditCardId { get; set; }
        [Required]
        [Display(Name = "记账日期")]
        [DisplayFormat(DataFormatString = "{0:MM月dd日}")]
        public DateTime RecordDate { get; set; }
        [Display(Name = "POS编号")]
        public int? PosId { get; set; }
        [Display(Name = "存入")]
        public double? Deposit { get; set; }
        [Display(Name = "支出")]
        public double? Expend { get; set; }

        public CcdData CcdData { get; set; }
        public CcdPos CcdPos { get; set; }
    }
}
