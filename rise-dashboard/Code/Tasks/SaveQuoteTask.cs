namespace rise.Code
{
    using Microsoft.Extensions.DependencyInjection;
    using Scheduling;
    using Data;
    using Models;
    using System;
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
        public string Schedule => "*/5 * * * *";

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
                        USDPrice = double.Parse(CoinbaseBtcQuoteResult.Current.data.amount) * LiveCoinQuote.Current.Last
                    };

                    var quoteAltilly = new CoinQuote
                    {
                        Exchange = "Altilly",
                        Price = double.Parse(AltillyQuote.Current.last),
                        Volume = double.Parse(AltillyQuote.Current.volume),
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuoteResult.Current.data.amount) * (double.Parse(AltillyQuote.Current.last) / 100000000)
                    };

                    var quoteVinex = new CoinQuote
                    {
                        Exchange = "Vinex",
                        Price = VinexQuote.Current.lastPrice,
                        Volume = VinexQuote.Current.volume,
                        TimeStamp = time,
                        USDPrice = Double.Parse(CoinbaseBtcQuoteResult.Current.data.amount) * VinexQuote.Current.lastPrice
                    };
                    var totalVolume = quoteLivecoin.Volume + quoteAltilly.Volume + quoteVinex.Volume;

                    var quoteAllWeighted = new CoinQuote
                    {
                        Exchange = "All",
                        Price = (quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume) + (quoteAltilly.Price * quoteAltilly.Volume / totalVolume) + (quoteVinex.Price * quoteVinex.Volume / totalVolume),
                        Volume = totalVolume,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuoteResult.Current.data.amount) *  ((quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume) + (quoteAltilly.Price * quoteAltilly.Volume / totalVolume) + (quoteVinex.Price * quoteVinex.Volume / totalVolume))
                    };

                    dbContext.CoinQuotes.Add(quoteLivecoin);
                    dbContext.CoinQuotes.Add(quoteAltilly);
                    dbContext.CoinQuotes.Add(quoteVinex);
                    dbContext.CoinQuotes.Add(quoteAllWeighted);

                    // Save Context in Database
                    await dbContext.SaveChangesAsync();
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