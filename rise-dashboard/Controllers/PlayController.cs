
using System;
using System.Linq;
using System.Net;
using rise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using rise.Code.DataFetcher;
using rise.Data;
using rise_lib;


namespace rise.Controllers
{

    [Authorize]
    public class PlayController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public PlayController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieve Current Context Logged User
        /// </summary>
        /// <returns></returns>
        //private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(this.User);
        }


        /// <summary>
        /// Create the Draw Here.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Member")]
        public async Task<IActionResult> CreateDrawAsync()
        {
            var user = await GetCurrentUserAsync();

            // Verify if user have enough fund.
            if (await RiseManager.AccountBalanceAsync(user.Address) >= 1)
            {
                var dealer = new Dealer();
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(dealer);

                await DoPayment(dealer.AmountToPay);
                return new JsonResult(json);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Process with the Payment
        /// </summary>
        private async Task DoPayment(int amount)
        {
            // Get the current user
            var user = await GetCurrentUserAsync();

            // Proceed the payment.
            if (amount > 0)
            {

               
            }
            else
            {
                // User lost...
               
            };
        }

        /// <summary>
        /// Return user balance in Realtime from Wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Member")]
        public async Task<IActionResult> GetUserBalance()
        {
            var user = await GetCurrentUserAsync();

            return new JsonResult(new { balance = RiseManager.AccountBalanceAsync(user.Address) });
        }


        /// <summary>
        /// Get Transactions from Wallet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Member")]
        public async Task<PartialViewResult> GetUserListTransactionsPartialView()
        {
            var user = await GetCurrentUserAsync();

            var transac = await TransactionsFetcher.FetchAllUserTransactions(user.Address);

            if (transac.success)
            {
                ViewBag.Transactions = transac.transactions.OrderByDescending(x => x.timestamp).Take(30);
            }

            return PartialView("_UserListTransactionsPartial");
        }


        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            // Get the current user
            var user = await GetCurrentUserAsync();

            // Assign user info.
            if (user != null)
            {
                ViewBag.OnlineWallet = user.Address;
                ViewBag.Balance = RiseManager.AccountBalanceAsync(user.Address);
            }

            return View("Index", user);
        }
    }
}