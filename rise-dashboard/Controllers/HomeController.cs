namespace rise.Controllers
{
    using Code.DataFetcher;
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels;

    /// <summary>
    /// Defines the <see cref="HomeController" />
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Defines the _userManager
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Defines the _appdb
        /// </summary>
        private readonly ApplicationDbContext _appdb;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        /// <param name="userManager">The userManager<see cref="UserManager{ApplicationUser}"/></param>
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _appdb = context;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Return the Quote of the day
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetQuoteOfTheDay()
        {
            if (QuoteOfTheDayResult.Current == null)
            {
                return null;
            }

            return Json(QuoteOfTheDayResult.Current);
        }

        /// <summary>
        /// Return the list of Delegates
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetDelegates()
        {
            if (DelegateResult.Current == null || DelegateResult.Current.Delegates == null)
            {
                return null;
            }

            return Json(DelegateResult.Current.Delegates.OrderBy(y => y.Username).Select(x => x.Username.TrimEnd().TrimStart()).ToList());
        }

        /// <summary>
        /// Payments Chart
        /// </summary>
        /// <param name="address"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<PartialViewResult> PaymentsChartAsync(string address, int minutes)
        {
            var accountSummaryViewModel = new AccountSummaryViewModel
            {
                liveCoinQuoteResult = LiveCoinQuote.Current,
                coinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                transactionsResult = TransactionsResult.Current,
                delegateResult = DelegateResult.Current,
                walletAccountResult = await WalletAccountFetcher.FetchRiseWalletAccount(address),
                delegateVotesResult = await DelegateVotesFetcher.FetchRiseDelegateVotes(address),
                coinReceivedByAccount = await TransactionsFetcher.FetchTransactions(address)
            };

            // Show Payments from currentTime - minutes
            ViewBag.Minutes = minutes;
            return PartialView("_PaymentsChartPartial", accountSummaryViewModel);
        }

        /// <summary>
        /// Payments Table
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<PartialViewResult> PaymentsTableAsync(string address)
        {
            var accountSummaryViewModel = new AccountSummaryViewModel
            {
                liveCoinQuoteResult = LiveCoinQuote.Current,
                coinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                transactionsResult = TransactionsResult.Current,
                delegateResult = DelegateResult.Current,
                walletAccountResult = await WalletAccountFetcher.FetchRiseWalletAccount(address),
                delegateVotesResult = await DelegateVotesFetcher.FetchRiseDelegateVotes(address),
                coinReceivedByAccount = await TransactionsFetcher.FetchTransactions(address)
            };

            return PartialView("_PaymentsTablePartial", accountSummaryViewModel);
        }

        /// <summary>
        /// Delegate Payments Chart
        /// </summary>
        /// <param name="address"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<PartialViewResult> DelegatePaymentsChartAsync(string address, int minutes)
        {
            var mydelegate = DelegateResult.Current.Delegates.Where(x => x.Address == address).FirstOrDefault();

            var quoteInfoViewModel = new DelegateStatsViewModel
            {
                DelegateResult = DelegateResult.Current,
                VotersResult = await DelegateVotersFetcher.FetchVoters(mydelegate.PublicKey),
                TransactionsResult = await TransactionsFetcher.FetchOutgoingTransactions(address)
            };

            // Show Payments from cuurentTime - minutes
            ViewBag.Minutes = minutes;
            return PartialView("_DelegatePaymentsChartPartial", quoteInfoViewModel);
        }

        /// <summary>
        /// The AccountSummaryAsync
        /// </summary>
        /// <param name="address">The address<see cref="string"/></param>
        /// <returns>The <see cref="Task{PartialViewResult}"/></returns>
        [AllowAnonymous]
        public async Task<PartialViewResult> AccountSummaryAsync(string address)
        {
            AccountSummaryViewModel accountSummaryViewModel;

            if (address != null)
            {
                var delegate_account = DelegateResult.Current.Delegates.Where(x => x.Username.Contains(address.ToLower()) || x.Address == address).OrderBy(j => j.Username.Length).FirstOrDefault();

                if (delegate_account != null)
                {
                    address = delegate_account.Address;
                    accountSummaryViewModel = new AccountSummaryViewModel
                    {
                        liveCoinQuoteResult = LiveCoinQuote.Current,
                        coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now).ToList(),
                        coinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                        transactionsResult = TransactionsResult.Current,
                        delegateResult = DelegateResult.Current,
                        walletAccountResult = await WalletAccountFetcher.FetchRiseWalletAccount(address),
                        delegateVotesResult = await DelegateVotesFetcher.FetchRiseDelegateVotes(address),
                        delegateVotersResult = await DelegateVotersFetcher.FetchVoters(delegate_account.PublicKey),
                        forgedByAccount = await ForgedByAccountFetcher.FetchForgedByAccount(delegate_account.PublicKey),
                        coinReceivedByAccount = await TransactionsFetcher.FetchTransactions(address),
                        coinSentByAccount = await TransactionsFetcher.FetchOutgoingTransactions(address)
                    };
                }
                else
                {
                    accountSummaryViewModel = new AccountSummaryViewModel
                    {
                        liveCoinQuoteResult = LiveCoinQuote.Current,
                        coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now).ToList(),
                        coinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                        transactionsResult = TransactionsResult.Current,
                        delegateResult = DelegateResult.Current,
                        walletAccountResult = await WalletAccountFetcher.FetchRiseWalletAccount(address),
                        coinReceivedByAccount = await TransactionsFetcher.FetchTransactions(address),
                        coinSentByAccount = await TransactionsFetcher.FetchOutgoingTransactions(address)
                    };
                }

                if (accountSummaryViewModel.walletAccountResult == null)
                {
                    return null;
                }
                else
                {
                    return PartialView("_AccountSummaryPartial", accountSummaryViewModel);
                }
            }

            return null;
        }

        /// <summary>
        /// Set Cookie value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        private void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
            {
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            }
            else
            {
                option.Expires = DateTime.Now.AddMilliseconds(10);
            }

            Response.Cookies.Append(key, value, option);
        }

        /// <summary>
        /// Return the current logged user.
        /// </summary>
        /// <returns></returns>
        private ApplicationUser GetCurrentUserAsync() => _appdb.Users.FirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);

        /// <summary>
        /// Return View for Servers Update
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<PartialViewResult> AccountPartialViewAsync(string address)
        {
            try
            {
                var accountSummaryViewModel = new AccountSummaryViewModel
                {
                    liveCoinQuoteResult = LiveCoinQuote.Current,
                    coinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                    transactionsResult = TransactionsResult.Current,
                    delegateResult = DelegateResult.Current,
                    walletAccountResult = await WalletAccountFetcher.FetchRiseWalletAccount(address),
                    delegateVotesResult = await DelegateVotesFetcher.FetchRiseDelegateVotes(address),
                    coinReceivedByAccount = await TransactionsFetcher.FetchTransactions(address)
                };

                return PartialView("_AccountPartial", accountSummaryViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from QuotePartial {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// The TransactionsTablePartialView
        /// </summary>
        /// <param name="address">The address<see cref="string"/></param>
        /// <returns>The <see cref="Task{PartialViewResult}"/></returns>
        [AllowAnonymous]
        public async Task<PartialViewResult> TransactionsTablePartialView(string address = "")
        {
            try
            {
                TransactionsViewModel transactionsViewModel = new TransactionsViewModel
                {
                    coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now).ToList(),
                    LiveCoinQuoteResult = LiveCoinQuote.Current,
                    CoinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                    DelegateResult = DelegateResult.Current
                };

                if (address?.Length == 0)
                {
                    transactionsViewModel.TransactionsResult = TransactionsResult.Current;
                }
                else
                {
                    var account = DelegateResult.Current.Delegates.Where(x => x.Username.Contains(address.ToLower()) || x.Address == address).OrderBy(j => j.Username.Length).FirstOrDefault();

                    if (account != null)
                    {
                        transactionsViewModel.TransactionsResult = await TransactionsFetcher.FetchAllUserTransactions(account.Address);
                    }
                    else
                    {
                        transactionsViewModel.TransactionsResult = await TransactionsFetcher.FetchAllUserTransactions(address);
                    }
                }

                return PartialView("_TransactionsTablePartial", transactionsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from QuotePartial {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Render Quote Partial View
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<PartialViewResult> AccountSummaryTipBotPartialView()
        {
            try
            {
                var address = GetCurrentUserAsync().Address;
                ViewBag.userpic = GetCurrentUserAsync().Photo_Url;

                var tipAccountSummaryViewModel = new AccountSummaryViewModel
                {
                    liveCoinQuoteResult = LiveCoinQuote.Current,
                    coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now).ToList(),
                    coinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                    transactionsResult = TransactionsResult.Current,
                    delegateResult = DelegateResult.Current,
                    walletAccountResult = await WalletAccountFetcher.FetchRiseWalletAccount(address),
                    coinReceivedByAccount = await TransactionsFetcher.FetchTransactions(address),
                    coinSentByAccount = await TransactionsFetcher.FetchOutgoingTransactions(address)
                };

                return PartialView("_AccountSummaryTipBotPartial", tipAccountSummaryViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from QuotePartial {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Render Quote Partial View
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public PartialViewResult QuotePartialView()
        {
            try
            {
                var quoteViewModel = new QuoteViewModel
                {
                    //liveCoinTx = await TransactionsFetcher.FetchAllUserTransactions(AppSettingsProvider.LiveCoinWalletAddress),
                    transactionsResult = TransactionsResult.Current,
                    liveCoinQuoteResult = LiveCoinQuote.Current,
                    coinbaseBtcQuoteResult = CoinbaseBtcQuoteResult.Current,
                    coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp.ToUniversalTime() > DateTime.Now.AddDays(-15)).ToList()
                };

                return PartialView("_QuotePartial", quoteViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from QuotePartial {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Render the Error Page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Error() => View();

        /// <summary>
        /// Render Index Page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Index(string theme, string address = "")
        {
            if (!String.IsNullOrEmpty(address))
            {
                SetCookie("addr", address, 100000000);
                ViewBag.SearchText = address;
            }

            if (!String.IsNullOrEmpty(theme))
            {
                SetCookie("theme", theme, 100000000);
                ViewBag.Theme = theme;
            }

            ViewBag.Info = _appdb.TgPinnedMsgs.OrderByDescending(x => x.Date).FirstOrDefault()?.Message;

            return View("Index");
        }

        /// <summary>
        /// Render Transactions Page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Transactions(string address = "")
        {
            ViewBag.Address = address;

            return View("Transactions");
        }

        /// <summary>
        /// Render DelegateMap
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult PeersMap()
        {
            return View(PeersResult.Current);
        }


        /// <summary>
        /// Render About Page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult TipBotStats() => View();

        /// <summary>
        /// Render About Page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult About() => View();

        /// <summary>
        /// Render the Delegates
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Delegates() => View(DelegateResult.Current);

        /// <summary>
        /// Return DelegateStats
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult DelegateStats(string UserName)
        {
            var mydelegate = DelegateResult.Current.Delegates.Where(x => x.Username == UserName).FirstOrDefault();

            if (mydelegate != null)
            {
                ViewBag.Address = mydelegate.Address;
            }

            return View(mydelegate);
        }
    }
}