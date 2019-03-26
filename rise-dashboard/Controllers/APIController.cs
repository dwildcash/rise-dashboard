namespace rise.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using rise.Data;
    using rise.Models;
    using System.Linq;

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
        /// Initializes a new instance of the <see cref="apiController"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        /// <param name="userManager">The userManager<see cref="UserManager{ApplicationUser}"/></param>
        public apiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _appdb = context;
        }

        /// <summary>
        /// Return the latest Quote
        /// </summary>
        /// <returns></returns>
        public JsonResult getQuote()
        {
            var obj = _appdb.CoinQuotes.Where(x => x.Exchange == "All").OrderByDescending(x => x.TimeStamp).FirstOrDefault();
            return Json(obj);
        }

        /// <summary>
        /// Return current quot of the day
        /// </summary>
        /// <returns></returns>
        public JsonResult getQuoteOfTheDay()
        {
            return Json(QuoteOfTheDayResult.Current);
        }
    }
}