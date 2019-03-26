﻿namespace rise.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using rise.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Web;
    using Telegram.Bot.Extensions.LoginWidget;

    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public IDictionary<string, string> ToDictionary(NameValueCollection col)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }
            return dict;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TGLoginCallback()
        {
            var fields = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());

            LoginWidget loginWidget = new LoginWidget(AppSettingsProvider.BotKey);
            if (loginWidget.CheckAuthorization(ToDictionary(fields)) == Authorization.Valid)
            {
                var aspnetuser = await _userManager.FindByNameAsync(fields["username"]);

                // User doesnt exit in aspnetdb let create it
                if (aspnetuser == null)
                {
                    aspnetuser = new ApplicationUser { UserName = fields["username"] };
                    IdentityResult result = await _userManager.CreateAsync(aspnetuser);

                    // By default add user to Guest
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(aspnetuser, "Member");
                    }
                }

                //sign the user and go to home
                await _signInManager.SignInAsync(aspnetuser, isPersistent: false);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }

            // Authenticate User if twitter say its ok.
            if (info.Principal.Identity.IsAuthenticated)
            {
                var aspnetuser = await _userManager.FindByNameAsync(info.Principal.Identity.Name);

                // User doesnt exit in aspnetdb let create it
                if (aspnetuser == null)
                {
                    aspnetuser = new ApplicationUser { UserName = info.Principal.Identity.Name };
                    IdentityResult result = await _userManager.CreateAsync(aspnetuser);

                    // By default add user to Guest
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(aspnetuser, "Member");
                    }
                }

                //sign the user and go to home
                await _signInManager.SignInAsync(aspnetuser, isPersistent: false);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}