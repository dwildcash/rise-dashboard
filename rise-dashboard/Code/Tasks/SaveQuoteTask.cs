﻿namespace rise.Code
{
    using Data;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Scheduling;
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

                    var quoteOoobtc = new CoinQuote
                    {
                        Exchange = "ooobtc",
                        Price = double.Parse(ooobtcQuote.Current.Lastprice),
                        Volume = ooobtcQuote.Current.Volume,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * double.Parse(ooobtcQuote.Current.Lastprice),
                    };

                    var quoteLivecoin = new CoinQuote
                    {
                        Exchange = "LiveCoin",
                        Price = LiveCoinQuote.Current.Last,
                        Volume = LiveCoinQuote.Current.Volume,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * LiveCoinQuote.Current.Last
                    };

                    var quoteBitker = new CoinQuote
                    {
                        Exchange = "Bitker",
                        Price = BitkerQuote.Current.close,
                        Volume = BitkerQuote.Current.amount,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * (double.Parse(AltillyQuote.Current.last) / 100000000)
                    };

                    var quoteAltilly = new CoinQuote
                    {
                        Exchange = "Altilly",
                        Price = double.Parse(AltillyQuote.Current.last),
                        Volume = double.Parse(AltillyQuote.Current.volume),
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * (double.Parse(AltillyQuote.Current.last) / 100000000)
                    };

                    var quoteVinex = new CoinQuote
                    {
                        Exchange = "Vinex",
                        Price = VinexQuote.Current.lastPrice,
                        Volume = VinexQuote.Current.baseVolume,
                        TimeStamp = time,
                        USDPrice = Double.Parse(CoinbaseBtcQuote.Current.amount) * VinexQuote.Current.lastPrice
                    };
                    var totalVolume = quoteLivecoin.Volume + quoteAltilly.Volume + quoteVinex.Volume + quoteOoobtc.Volume + quoteBitker.Volume;

                    var quoteAllWeighted = new CoinQuote
                    {
                        Exchange = "All",
                        Price = (quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume) + (quoteAltilly.Price * quoteAltilly.Volume / totalVolume) + (quoteVinex.Price * quoteVinex.Volume / totalVolume) + (quoteOoobtc.Price * quoteOoobtc.Volume / totalVolume) + (quoteBitker.Price * quoteBitker.Volume / totalVolume),
                        Volume = totalVolume,
                        TimeStamp = time,
                        USDPrice = double.Parse(CoinbaseBtcQuote.Current.amount) * ((quoteLivecoin.Price * quoteLivecoin.Volume / totalVolume) + (quoteAltilly.Price * quoteAltilly.Volume / totalVolume) + (quoteVinex.Price * quoteVinex.Volume / totalVolume) + (quoteBitker.Price * quoteBitker.Volume / totalVolume) + (quoteOoobtc.Price * quoteOoobtc.Volume / totalVolume))
                    };

                    dbContext.CoinQuotes.Add(quoteLivecoin);
                    dbContext.CoinQuotes.Add(quoteAltilly);
                    dbContext.CoinQuotes.Add(quoteVinex);
                    dbContext.CoinQuotes.Add(quoteOoobtc);
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