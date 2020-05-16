namespace rise.Code
{
    using Data;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Scheduling;
    using System;
    using System.Collections.Generic;
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

                    var quoteLivecoin = new CoinQuote
                    {
                        Exchange = "LiveCoin",
                        Price = LiveCoinQuote.Current.Last,
                        Volume = LiveCoinQuote.Current.Volume,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * LiveCoinQuote.Current.Last
                    };

                    var quoteXtcom = new CoinQuote
                    {
                        Exchange = "Xt.com",
                        Price = double.Parse(XtcomQuoteResult.Current.datas[0]) / double.Parse(CoinbaseBtcQuote.Current.amount) / 10000000,
                        Volume = double.Parse(XtcomQuoteResult.Current.datas[4]),
                        TimeStamp = time,
                        USDPrice = double.Parse(XtcomQuoteResult.Current.datas[0]) * (double.Parse(XtcomQuoteResult.Current.datas[0]) / double.Parse(CoinbaseBtcQuote.Current.amount) / 10000000)
                    };

                    var totalVolume = quoteLivecoin.Volume + quoteXtcom.Volume;

                    var quoteAllWeighted = new CoinQuote
                    {
                        Exchange = "All",
                        Price = (quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume) + (quoteXtcom.Price * quoteXtcom.Volume / totalVolume),
                        Volume = totalVolume,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * ((quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume) + (quoteXtcom.Price * quoteXtcom.Volume / totalVolume))
                    };
         
                    dbContext.CoinQuotes.Add(quoteLivecoin);
                    dbContext.CoinQuotes.Add(quoteXtcom);
                    dbContext.CoinQuotes.Add(quoteAllWeighted);

                    // Save Context in Database
                    await dbContext.SaveChangesAsync();

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