namespace rise.Code
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
                    var liveCoinPrice = 0.0;
                    var liveCoinVolume = 0.0;
                    var XtcomPrice = 0.0;
                    var XtcomVolume = 0.0;

                    try
                    {
                        var quoteLivecoin = new CoinQuote
                        {
                            Exchange = "LiveCoin",
                            Price = LiveCoinQuote.Current.Last,
                            Volume = LiveCoinQuote.Current.Volume,
                            TimeStamp = time,
                            USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * LiveCoinQuote.Current.Last
                        };

                        liveCoinVolume = quoteLivecoin.Volume;
                        liveCoinPrice = quoteLivecoin.Price;
                        dbContext.CoinQuotes.Add(quoteLivecoin);
                        dbContext.SaveChangesAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        var log = new Log();
                        log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                        dbContext.Logger.Add(log);
                        dbContext.SaveChangesAsync().Wait();
                    }


                    try
                    {
                        var quoteXtcom = new CoinQuote
                        {
                            Exchange = "Xt.com",
                            Price = XtcomQuote.Current.price / 10000,
                            Volume = XtcomQuote.Current.moneyVol,
                            TimeStamp = time,
                            USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * XtcomQuote.Current.price / 10000,
                        };

                        XtcomPrice = quoteXtcom.Price;
                        XtcomVolume = quoteXtcom.Volume;
                        dbContext.CoinQuotes.Add(quoteXtcom);
                        dbContext.SaveChangesAsync().Wait();

                    }
                    catch (Exception ex)
                    {
                        var log = new Log();
                        log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                        dbContext.Logger.Add(log);
                        dbContext.SaveChangesAsync().Wait();
                    }


                    try
                    {
                        var totalVolume = liveCoinVolume;

                        var quoteAllWeighted = new CoinQuote
                        {
                            Exchange = "All",
                            Price = (liveCoinPrice * liveCoinVolume / totalVolume),
                            Volume = totalVolume,
                            TimeStamp = time,
                            USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * ((liveCoinPrice * liveCoinVolume / totalVolume))
                        };

                        dbContext.CoinQuotes.Add(quoteAllWeighted);

                        // Save Context in Database
                        await dbContext.SaveChangesAsync();
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