using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.CreditCard
{
    [Table("Ccd_Data")]
    public partial class CcdData
    {
        public CcdData()
        {
            CcdBill = new HashSet<CcdBill>();
            CcdRecord = new HashSet<CcdRecord>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "信用卡ID")]
        public int CreditCardId { get; set; }
        [Required]
        [Display(Name = "成员ID")]
        public int PersonId { get; set; }
        [Required]
        [Display(Name = "发卡行")]
        public BankType IssuingBank { get; set; }
        [Required]
        [Display(Name = "卡号")]
        public string CreditCardNumber { get; set; }
        [Required]
        [Display(Name = "固定额度")]
        public double Limit { get; set; }
        [Display(Name = "临时额度")]
        public double? Temporary { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM月dd日}")]
        [Display(Name = "临时有效期")]
        public DateTime? TempDate { get; set; }   
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM月dd日}")]
        [Display(Name = "账单日")]
        public DateTime AccountBill { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM月dd日}")]
        [Display(Name = "还款日")]
        public DateTime RepaymentDate { get; set; }
        [Required]
        [Display(Name = "账单金额")]
        public double BillAmount { get; set; }
        [Required]
        [Display(Name = "有效日期")]
        public string ValidThru { get; set; }
        [Required]
        [Display(Name = "安全码")]
        public string Cvv { get; set; }
        [Required]
        [StringLength(6, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [Display(Name = "交易密码")]
        public string TransactionPw { get; set; }
        [Required]
        [StringLength(6, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [Display(Name = "查询密码")]
        public string InquriyPw { get; set; }
        [Display(Name = "网银密码")]
        public string OnlineBankingPw { get; set; }
        [Required]
        [Display(Name = "已还款额")]
        public double HasPayment { get; set; }
        [Required]
        [Display(Name = "未还款额")]
        public double PrePayment { get; set; }

        public CcdPerson CcdPerson { get; set; }
        public ICollection<CcdBill> CcdBill { get; set; }
        public ICollection<CcdRecord> CcdRecord { get; set; }
    }
    public enum BankType
    {
        工商,
        建设,
        农业,
        中国,
        交通,
        招商,
        广发,
        中信,
        民生,
        光大,
        浦发,
        平安,
        邮政,
        农商
    }
}
