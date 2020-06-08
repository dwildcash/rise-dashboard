﻿namespace rise.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Newtonsoft.Json;
    using rise.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

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

        /// <summary>
        /// Defines the scopeFactory
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        // POST api/WebHook
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [EnableCors("CorsPolicy")]

        public async Task<IActionResult> Webhook(string secret, [FromBody]dynamic jsonresult)
        {
            try
            {
                Update w = JsonConvert.DeserializeObject<Update>(jsonresult.ToString());

                // Return if the secret is not correct
                if (secret != AppSettingsProvider.WebHookSecret)
                {
                    return Unauthorized();
                }

                await _updateService.EchoAsync(w);
            }
            catch (Exception ex)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var log = new Log();
                    log.LogMessage(ex.Message);
                    dbContext.Logger.Add(log);
                    dbContext.SaveChangesAsync().Wait();
                }
            }

            return Ok();
        }

        /// <summary>
        /// api Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="updateService"></param>
        public apiController(ApplicationDbContext context, IUpdateService updateService, IServiceScopeFactory scopeFactory)
        {
            _updateService = updateService;
            _appdb = context;
            _scopeFactory = scopeFactory;
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
