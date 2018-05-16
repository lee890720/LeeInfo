using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LeeInfo.Web.Areas.AppIdentity.Models;
using LeeInfo.Data.AppIdentity;
using Microsoft.AspNetCore.Http;

namespace LeeInfo.Web.Areas.AppIdentity.Controllers
{
    [Area("AppIdentity")]
    [Authorize(Roles = "Admins")]
    public class UserAdminController : Controller
    {
        private UserManager<AppIdentityUser> userManager;
        private IUserValidator<AppIdentityUser> userValidator;
        private IPasswordValidator<AppIdentityUser> passwordValidator;
        private IPasswordHasher<AppIdentityUser> passwordHasher;

        public UserAdminController(UserManager<AppIdentityUser> usrMgr,
                IUserValidator<AppIdentityUser> userValid,
                IPasswordValidator<AppIdentityUser> passValid,
                IPasswordHasher<AppIdentityUser> passwordHash)
        {
            userManager = usrMgr;
            userValidator = userValid;
            passwordValidator = passValid;
            passwordHasher = passwordHash;
        }

        public ViewResult Index() => View(userManager.Users);

        public IActionResult Create()
        {
            return PartialView("~/Areas/AppIdentity/Views/UserAdmin/Create.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppIdentityUser user = new AppIdentityUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    RegisterDate = DateTime.Now,
                    UserImage="/images/photo2.png"
                };
                IdentityResult result
                    = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return PartialView("~/Areas/AppIdentity/Views/UserAdmin/Create.cshtml",model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return PartialView("~/Areas/AppIdentity/Views/UserAdmin/Delete.cshtml", user.UserName);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id,IFormCollection form)
        {
            AppIdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
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
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", userManager.Users);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Edit(string id)
        {
            AppIdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return PartialView("~/Areas/AppIdentity/Views/UserAdmin/Edit.cshtml", user);

            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(string id, string email,string password,SexType sex,string userImage,bool connectAPI,
            string clientId,string clientSecret,string accessToken,string refreshToken,string connectUrl,string apiUrl,string apiHost,int? apiPort)
        {
            AppIdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                user.Sex = sex;
                user.UserImage = userImage;
                user.ConnectAPI = connectAPI;
                user.ClientId = clientId;
                user.ClientSecret = clientSecret;
                user.AccessToken = accessToken;
                user.RefreshToken = refreshToken;
                user.ConnectUrl = connectUrl;
                user.ApiUrl = apiUrl;
                user.ApiHost = apiHost;
                if (apiPort != null)
                    user.ApiPort = (int)apiPort;
                else
                    user.ApiPort = 0;
                IdentityResult validEmail
                    = await userValidator.ValidateAsync(userManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager,
                        user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user,
                            password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
                if ((validEmail.Succeeded && validPass == null)
                        || (validEmail.Succeeded
                        && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return PartialView("~/Areas/AppIdentity/Views/UserAdmin/Edit.cshtml", user);
        }

        [AllowAnonymous]
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

    }
}
