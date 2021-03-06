﻿namespace rise.Code
{
    using Data;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Scheduling;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="SaveQuoteTask" />
    /// </summary>
    public class SaveQuoteTask : IScheduledTask
    {
        /// <summary>
        /// Gets the Schedule
        /// </summary>
        public string Schedule => "*/1 * * * *";

        /// <summary>
        /// Defines the scopeFactory
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var time = DateTime.Now.ToUniversalTime();

                    var XtcomPrice = 0.0;
                    var XtcomVolume = 0.0;

                    try
                    {
                        var quoteXtcom = new CoinQuote
                        {
                            Exchange = "Xt.com",
                            Price = ((XtcomQuote.Current.price * 100000000) / double.Parse(CoinbaseBtcQuote.Current.amount)) / 100000000,
                            Volume = XtcomQuote.Current.moneyVol,
                            TimeStamp = time,
                            USDPrice = XtcomQuote.Current.price
                        };

                        XtcomPrice = quoteXtcom.Price;
                        XtcomVolume = quoteXtcom.Volume;

                        var totalVolume = XtcomVolume;

                        var quoteAllWeighted = new CoinQuote
                        {
                            Exchange = "All",
                            Price = (XtcomPrice * XtcomVolume / totalVolume),
                            Volume = totalVolume,
                            TimeStamp = time,
                            USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * ((XtcomPrice * XtcomVolume / totalVolume))
                        };

                        dbContext.CoinQuotes.Add(quoteXtcom);
                        dbContext.CoinQuotes.Add(quoteAllWeighted);
                        dbContext.SaveChangesAsync().Wait();

                    }
                    catch (Exception ex)
                    {
                        var log = new Log();
                        log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                        dbContext.Logger.Add(log);
                        dbContext.SaveChangesAsync().Wait();
                    }


                    // Load latest all Last 15 days
                    CoinQuoteResult.Current = dbContext.CoinQuotes.AsEnumerable().Where(x => x.TimeStamp.ToUniversalTime() > DateTime.Now.AddDays(-15)).ToList();
                }
            }
            catch (Exception e)
            {
                Console.Write(e.InnerException);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveQuoteTask"/> class.
        /// </summary>
        /// <param name="scopeFactory">The scopeFactory<see cref="IServiceScopeFactory"/></param>
        public SaveQuoteTask(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
    }
}