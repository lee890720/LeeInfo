using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.CreditCard
{
    [Table("Ccd_Pos")]
    public partial class CcdPos
    {
        public CcdPos()
        {
            CcdRecord = new HashSet<CcdRecord>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "POSID")]
        public int PosId { get; set; }
        [Required]
        [Display(Name = "成员ID")]
        public int PersonId { get; set; }
        [Required]
        [Display(Name = "商户名称")]
        public string PosName { get; set; }
        [Display(Name = "备注")]
        public string PosNote { get; set; }

        public CcdPerson CcdPerson { get; set; }
        public ICollection<CcdRecord> CcdRecord { get; set; }
    }
}
