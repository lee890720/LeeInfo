using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LeeInfo.Data.AppIdentity;
using Microsoft.AspNetCore.Identity;

namespace LeeInfo.Web.Areas.Identity.Models.Account
{
    public class RoleEditModel
    {
        public IdentityRole Role { get; set; }

        public IEnumerable<AppIdentityUser> Members { get; set; }

        public IEnumerable<AppIdentityUser> NonMembers { get; set; }
    }
}
