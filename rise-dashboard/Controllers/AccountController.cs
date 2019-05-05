namespace rise.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using rise.Models;
    using rise.Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Telegram.Bot.Extensions.LoginWidget;

    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAppUsersManagerService _appUsersManagerService;
        private readonly ILogger<AppUsersManagerService> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager, IAppUsersManagerService appUsersManagerService, ILogger<AppUsersManagerService> logger)
        {
            _signInManager = signInManager;
            _appUsersManagerService = appUsersManagerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCallback(int id, string first_name, string username, string photo_url, string auth_date, string hash)
        {
            var fields = new Dictionary<string, string>()
            {
                { "id", id.ToString() },
                { "first_name", first_name },
                { "username", username},
                { "photo_url", photo_url },
                { "auth_date", auth_date },
                { "hash", hash }
            };

            var loginWidget = new LoginWidget(AppSettingsProvider.BotApiKey);

            try
            {
                if (loginWidget.CheckAuthorization(fields) == Authorization.Valid)
                {
                    var aspnetuser = await _appUsersManagerService.GetUserAsync(fields["username"], long.Parse(fields["id"]));
                    _appUsersManagerService.Update_Photourl(aspnetuser.TelegramId, fields["photo_url"]);

                    //sign the user and go to home
                    await _signInManager.SignInAsync(aspnetuser, isPersistent: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from loginCallback {0}", ex.Message);
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