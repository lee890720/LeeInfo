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
        public int AccountNumber { get; set; }
        public string Password { get; set; }
        public bool Alive { get; set; }
        public string AppIdentityUserId { get; set; }
        public AppIdentityUser AppIdentityUser { get; set; }
    }
}
