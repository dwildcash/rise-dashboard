namespace rise.Code
{
    using Microsoft.Extensions.DependencyInjection;
    using rise.Code.Scheduling;
    using rise.Data;
    using rise.Models;
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
        public string Schedule => "* * * * *";

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

                    var quoteRightBtc = new CoinQuote
                    {
                        Exchange = "RightBtc",
                        Price = (double)RightBtcQuoteResult.Current.Result.Last / 100000000,
                        Volume = RightBtcQuoteResult.Current.Result.Vol24H / 100000000,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuoteResult.Current.data.amount) * ((double)RightBtcQuoteResult.Current.Result.Last / 100000000)
                    };

                    var quoteAltilly = new CoinQuote
                    {
                        Exchange = "Altilly",
                        Price = double.Parse(AltillyQuote.Current.last),
                        Volume = double.Parse(AltillyQuote.Current.volume),
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuoteResult.Current.data.amount) * (double.Parse(AltillyQuote.Current.last) / 100000000)
                    };

                    var totalVolume = quoteRightBtc.Volume + quoteLivecoin.Volume + quoteAltilly.Volume;

                    var quoteAllWeighted = new CoinQuote
                    {
                        Exchange = "All",
                        Price = (quoteRightBtc.Price * quoteRightBtc.Volume / totalVolume) + (quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume),
                        Volume = totalVolume,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuoteResult.Current.data.amount) * ((quoteRightBtc.Price * quoteRightBtc.Volume / totalVolume) + (quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume))
                    };

                    dbContext.CoinQuotes.Add(quoteLivecoin);
                    dbContext.CoinQuotes.Add(quoteRightBtc);
                    dbContext.CoinQuotes.Add(quoteAltilly);

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