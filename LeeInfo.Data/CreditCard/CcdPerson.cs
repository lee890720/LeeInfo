using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.CreditCard
{
    [Table("Ccd_Person")]
    public partial class CcdPerson
    {
        public CcdPerson()
        {
            CcdData = new HashSet<CcdData>();
            CcdDebt = new HashSet<CcdDebt>();
            CcdPos = new HashSet<CcdPos>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "成员ID")]
        public int PersonId { get; set; }
        [Required]
        [Display(Name = "姓名")]
        public string PersonName { get; set; }
        [Required]
        [Display(Name = "性别")]
        public SexType Sex { get; set; }
        [Display(Name = "出生日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "身份证号")]
        public string IdcardNumber { get; set; }
        [Required]
        [Display(Name = "联系电话")]
        public string Mobile { get; set; }
        [Display(Name = "邮箱地址")]
        public string Email { get; set; }
        [Display(Name = "备注")]
        public string PersonNote { get; set; }

        public ICollection<CcdData> CcdData { get; set; }
        public ICollection<CcdDebt> CcdDebt { get; set; }
        public ICollection<CcdPos> CcdPos { get; set; }
    }
    public enum SexType
    {
        [Display(Name = "男")]
        男,
        [Display(Name = "女")]
        女
    }
}
