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
        [Display(Name = "性别")]
        public SexType Sex { get; set; }
        [Display(Name = "注册时间")]
        public DateTime RegisterDate { get; set; }
        [Display(Name = "头像")]
        public string UserImage { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get;set; }
    }
    public enum SexType
    {
        [Display(Name = "男")]
        男,
        [Display(Name = "女")]
        女
    }
}
