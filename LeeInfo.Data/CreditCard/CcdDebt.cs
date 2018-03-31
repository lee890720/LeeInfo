using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.CreditCard
{
    public partial class CcdDebt
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "DebtID")]
        public int DebtId { get; set; }
        [Required]
        [Display(Name = "成员ID")]
        public int PersonId { get; set; }
        [Required]
        [Display(Name = "平台")]
        public string DebtTitle { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM月dd日}")]
        [Display(Name = "还款日")]
        public DateTime RepaymentDate { get; set; }
        [Required]
        [Display(Name = "金额")]
        public double BillAmount { get; set; }
        [Display(Name = "备注")]
        public string DebtNote { get; set; }
        [Required]
        [Display(Name = "当期金额")]
        public double CurrentAmount { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:P}")]
        [Display(Name = "利率")]
        public double InterestRate { get; set; }

        public CcdPerson CcdPerson { get; set; }
    }
}
