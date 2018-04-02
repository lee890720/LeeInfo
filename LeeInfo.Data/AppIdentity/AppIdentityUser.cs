using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LeeInfo.Data.AppIdentity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class AppIdentityUser : IdentityUser
    {
        [Display(Name = "注册时间")]
        public DateTime RegisterDate { get; set; }
    }
}
