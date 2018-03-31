using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using LeeInfo.Web.Areas.AppIdentity.Models;
using LeeInfo.Data.AppIdentity;
using Microsoft.AspNetCore.Http;

namespace LeeInfo.Web.Areas.AppIdentity.Controllers
{
    [Area("AppIdentity")]
    [Authorize(Roles = "Admins")]
    public class RoleAdminController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppIdentityUser> userManager;

        public RoleAdminController(RoleManager<IdentityRole> roleMgr,
                                   UserManager<AppIdentityUser> userMrg)
        {
            roleManager = roleMgr;
            userManager = userMrg;
        }

        public ViewResult Index() => View(roleManager.Roles);

        public IActionResult Create()
        {
            return PartialView("~/Areas/AppIdentity/Views/RoleAdmin/Create.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result
                    = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return PartialView("~/Areas/AppIdentity/Views/RoleAdmin/Create.cshtml",name);
        }

        public async Task<IActionResult> Edit(string id)
        {

            IdentityRole role = await roleManager.FindByIdAsync(id);
            List<AppIdentityUser> members = new List<AppIdentityUser>();
            List<AppIdentityUser> nonMembers = new List<AppIdentityUser>();
            foreach (AppIdentityUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name)
                    ? members : nonMembers;
                list.Add(user);
            }
            return PartialView("~/Areas/AppIdentity/Views/RoleAdmin/Edit.cshtml",new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    AppIdentityUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user,
                            model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    AppIdentityUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user,
                            model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return PartialView("~/Areas/AppIdentity/Views/RoleAdmin/Edit.cshtml", model.RoleId);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await roleManager.FindByIdAsync(id);
            if (role== null)
            {
                return NotFound();
            }

            return PartialView("~/Areas/AppIdentity/Views/RoleAdmin/Delete.cshtml", role.Name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id,IFormCollection form)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "No role found");
            }
            return View("Index", roleManager.Roles);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
