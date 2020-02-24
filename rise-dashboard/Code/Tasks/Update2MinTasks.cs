using rise.Code.DataFetcher;
using rise.Code.Scheduling;
using rise.Models;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Code.Tasks
{
    /// <summary>
    /// Defines the <see cref="Update2MinTasks" />
    /// </summary>
    public class Update2MinTasks : IScheduledTask
    {
        /// <summary>
        /// Gets the Schedule
        /// </summary>
        public string Schedule => "*/2 * * * *";

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var liveCoinQuoteResult = await LiveCoinQuoteFetcher.FetchLiveCoinQuote();

            if (liveCoinQuoteResult != null)
            {
                LiveCoinQuote.Current = liveCoinQuoteResult;
            }

            var vinexQuoteResult = await VinexQuoteFetcher.FetchVinexCoinQuote();

            if (vinexQuoteResult != null)
            {
                VinexQuote.Current = vinexQuoteResult;
            }

            var altillyQuoteResult = await AltillyQuoteFetcher.FetchAltillyQuote();

            if (altillyQuoteResult != null)
            {
                AltillyQuote.Current = altillyQuoteResult;
            }

            var coinbaseBtcQuoteResult = await CoinbaseBtcFetcher.FetchCoinbaseBtcQuote();

            if (coinbaseBtcQuoteResult != null)
            {
                CoinbaseBtcQuote.Current = coinbaseBtcQuoteResult;
            }

            var transactionsResult = await TransactionsFetcher.FetchTransactions();

            if (transactionsResult.success)
            {
                TransactionsResult.Current = transactionsResult;
            }
        }
    }
}