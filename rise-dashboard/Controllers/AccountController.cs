﻿namespace rise.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using rise.Helpers;
    using rise.Models;
    using rise.Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Telegram.Bot.Extensions.LoginWidget;

    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAppUsersManagerService _appUsersManagerService;

        public AccountController(SignInManager<ApplicationUser> signInManager, IAppUsersManagerService appUsersManagerService)
        {
            _signInManager = signInManager;
            _appUsersManagerService = appUsersManagerService;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SyncUser(int TelegramId, string UserName, string Secret, string Address, string PublickKey)
        {
            var aspnetuser = await _appUsersManagerService.GetUserAsync(UserName, TelegramId);

            if (aspnetuser != null)
            {
                aspnetuser.TelegramId = TelegramId;
                aspnetuser.Secret = CryptoManager.EncryptStringAES(Secret.Replace("\r", ""), AppSettingsProvider.EncryptionKey);
                aspnetuser.Address = Address;
                aspnetuser.PublicKey = PublickKey;
                await _appUsersManagerService.UpdateApplicationUser(aspnetuser);
            }

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCallback(int id, string first_name, string username, string photo_url, string auth_date, string hash)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "id", id.ToString() },
                { "first_name", first_name },
                { "username", username},
                { "photo_url", photo_url },
                { "auth_date", auth_date },
                { "hash", hash }
            };

            LoginWidget loginWidget = new LoginWidget(AppSettingsProvider.BotApiKey);

            if (loginWidget.CheckAuthorization(fields) == Authorization.Valid)
            {
                var aspnetuser = await _appUsersManagerService.GetUserAsync(fields["username"], long.Parse(fields["id"]));

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