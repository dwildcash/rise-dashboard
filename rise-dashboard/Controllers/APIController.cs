namespace rise.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Rise.Services;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    /// <summary>
    /// Defines the <see cref="APIController" />
    /// </summary>
    [Route("api/[controller]")]
    public class APIController : Controller
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
        public async Task<IActionResult> Post(HttpRequestMessage update)
        {
            var message = update.Content.ReadAsStringAsync();
            
            /*
            try
            {
                await _updateService.EchoAsync(update);
            }
            catch
            {
                return Ok();
            }*/

            return Ok();
        }

        /// <summary>
        /// api Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="updateService"></param>
        public APIController(ApplicationDbContext context, IUpdateService updateService)
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