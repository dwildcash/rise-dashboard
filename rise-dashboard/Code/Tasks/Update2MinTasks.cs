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

            var XtcomQuoteResult = await XtcomQuoteFetcher.FetchXtcomQuote();

            if (XtcomQuoteResult != null)
            {
                XtcomQuoteResult.Current = XtcomQuoteResult;
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