namespace rise.Code
{
    using rise.Code.DataFetcher;
    using rise.Code.Scheduling;
    using rise.Models;
    using System.Threading;
    using System.Threading.Tasks;

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
            LiveCoinQuote liveCoinQuoteResult = await LiveCoinQuoteFetcher.FetchLiveCoinQuote();

            if (liveCoinQuoteResult != null)
            {
                LiveCoinQuote.Current = liveCoinQuoteResult;
            }

            CoinbaseBtcQuoteResult coinbaseBtcQuoteResult = await CoinbaseBtcFetcher.FetchCoinbaseBtcQuote();

            if (coinbaseBtcQuoteResult != null)
            {
                CoinbaseBtcQuoteResult.Current = coinbaseBtcQuoteResult;
            }

            RightBtcQuoteResult RightBtcQuoteResult = await RightBtcQuoteFetcher.FetchRightBtcQuote();

            if (RightBtcQuoteResult != null)
            {
                RightBtcQuoteResult.Current = RightBtcQuoteResult;
            }

            TransactionsResult transactionsResult = await TransactionsFetcher.FetchTransactions();

            if (transactionsResult.success)
            {
                TransactionsResult.Current = transactionsResult;
            }
        }
    }
}