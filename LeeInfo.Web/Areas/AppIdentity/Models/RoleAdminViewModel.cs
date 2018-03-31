using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using LeeInfo.Data.AppIdentity;

namespace LeeInfo.Web.Areas.AppIdentity.Models
{
    public class RoleEditModel
    {
        public IdentityRole Role { get; set; }

        public IEnumerable<AppIdentityUser> Members { get; set; }

        public IEnumerable<AppIdentityUser> NonMembers { get; set; }
    }
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }

        public string[] IdsToAdd { get; set; }

        public string[] IdsToDelete { get; set; }
    }
}
