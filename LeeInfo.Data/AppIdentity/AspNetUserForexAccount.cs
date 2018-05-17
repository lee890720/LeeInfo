using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.AppIdentity
{
    [Table("AspNetUserForexAccount")]
    public partial class AspNetUserForexAccount
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "交易账号")]
        public int AccountNumber { get; set; }
        [Display(Name = "密码")]
        public string Password { get; set; }
        [Display(Name = "是否作为默认显示账号")]
        public bool Alive { get; set; }
        public string AppIdentityUserId { get; set; }
        public AppIdentityUser AppIdentityUser { get; set; }
    }
}
