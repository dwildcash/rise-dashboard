namespace rise.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using rise.Helpers;
    using rise.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SyncUser(int TelegramId, string UserName, string Secret, string Address, string PublickKey)
        {
            var aspnetuser = await _userManager.FindByNameAsync(UserName);

            // User doesnt exit in aspnetdb let create it
            if (aspnetuser == null)
            {
                aspnetuser = new ApplicationUser { UserName = UserName, TelegramId = TelegramId, EncryptedBip39 = Secret, Address= Address, PublicKey = PublickKey };
                IdentityResult result = await _userManager.CreateAsync(aspnetuser);

                // By default add user to Guest
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(aspnetuser, "Member");
                }
            }
            else
            {
                aspnetuser.TelegramId = TelegramId;
                aspnetuser.EncryptedBip39 = Secret;
                aspnetuser.Address = Address;
                aspnetuser.PublicKey = PublickKey;
                await _userManager.UpdateAsync(aspnetuser);
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
                var aspnetuser = await _userManager.FindByNameAsync(fields["username"]);

                if (aspnetuser.Photo_Url == null)
                {
                    aspnetuser.Photo_Url = fields["photo_url"];
                }

                // User doesnt exit in aspnetdb let create it
                if (aspnetuser == null)
                {
                    aspnetuser = new ApplicationUser { UserName = fields["username"], Photo_Url = fields["photo_url"] };
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