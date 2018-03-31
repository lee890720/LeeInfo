using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.CreditCard
{
    public partial class CcdBill
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Bill_ID")]
        public int BillId { get; set; }
        [Required]
        [Display(Name = "信用卡ID")]
        public int CreditCardId { get; set; }
        [Required]
        [Display(Name = "记账日期")]
        [DisplayFormat(DataFormatString = "{0:MM月dd日}")]
        public DateTime BillDate { get; set; }
        [Required]
        [Display(Name = "账单金额")]
        public double BillAmount { get; set; }

        public CcdData CcdData { get; set; }
    }
}
