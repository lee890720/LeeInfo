using LeeInfo.Data.CreditCard;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeeInfo.Web.Areas.CreditCard.Models
{
    public class RecordGroupViewModel
    {
        [Required]
        [Display(Name = "信用卡ID")]
        public int CreditCardId { get; set; }

        [Required]
        [Display(Name = "卡号")]
        public string CreditCardNumber { get; set; }

        [Required]
        [Display(Name = "持卡人")]
        public string PersonName { get; set; }

        [Required]
        [Display(Name = "发卡行")]
        public BankType IssuingBank { get; set; }

        [Required]
        [Display(Name = "账单金额")]
        public double BillAmount { get; set; }

        [Required]
        [Display(Name = "未还款金额")]
        public double OutstandingAmount { get; set; }

        [Required]
        [Display(Name = "存入次数")]
        public int DepositCount { get; set; }

        [Required]
        [Display(Name = "存入总额")]
        public double DepositSum { get; set; }

        [Required]
        [Display(Name = "支出次数")]
        public int ExpendCount { get; set; }

        [Required]
        [Display(Name = "支出总额")]
        public double ExpendSum { get; set; }

        [Required]
        [Display(Name = "卡剩余总额")]
        public double Total { get; set; }

        [Required]
        [Display(Name = "推荐")]
        public int PosId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM月dd日}")]
        [Display(Name = "还款日")]
        public DateTime RepaymentDate { get; set; }
    }
}
