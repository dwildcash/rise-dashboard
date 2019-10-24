namespace rise.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Rise.Services;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="apiController" />
    /// </summary>
    public class apiController : Controller
    {
        /// <summary>
        /// Defines the _appdb
        /// </summary>
        private readonly ApplicationDbContext _appdb;

        /// <summary>
        /// Telegram Update Service
        /// </summary>
        private readonly IUpdateService _updateService;

        // POST api/WebHook
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> WebHook(string secret, [FromBody]Telegram.Bot.Types.Update update)
        {
            // Return if the secret is not correct
            if (secret != AppSettingsProvider.WebHookSecret)
            {
                return Unauthorized();
            }
            try
            {
                await _updateService.EchoAsync(update);
            }
            catch
            {
                return Ok();
            }

            return Ok();
        }

        /// <summary>
        /// api Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="updateService"></param>
        public apiController(ApplicationDbContext context, IUpdateService updateService)
        {
            _updateService = updateService;
            _appdb = context;
        }

        /// <summary>
        /// Return the latest Quote
        /// </summary>
        /// <returns></returns>
        ///
        [AllowAnonymous]
        public JsonResult GetQuote()
        {
            var obj = _appdb.CoinQuotes.Where(x => x.Exchange == "All").OrderByDescending(x => x.TimeStamp).FirstOrDefault();
            return Json(obj);
        }

        /// <summary>
        /// Return current quot of the day
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public JsonResult GetQuoteOfTheDay()
        {
            return Json(QuoteOfTheDayResult.Current);
        }
    }
}