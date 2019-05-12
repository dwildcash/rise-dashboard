namespace rise.ViewModels
{
    using Models;
    using rise.Code.Rise;

    /// <summary>
    /// Defines the <see cref="TransactionsViewModel" />
    /// </summary>
    public class TransactionsViewModel : QuoteStats
    {
        /// <summary>
        /// Gets or sets the LiveCoinQuoteResult
        /// </summary>
        public LiveCoinQuote LiveCoinQuoteResult { get; set; }

        /// <summary>
        /// Gets or sets the CoinbaseBtcQuoteResult
        /// </summary>
        public CoinbaseBtcQuoteResult CoinbaseBtcQuoteResult { get; set; }

        /// <summary>
        /// Gets or sets the TransactionsResult
        /// </summary>
        public TransactionsResult TransactionsResult { get; set; }

        /// <summary>
        /// Gets or sets the DelegateResult
        /// </summary>
        public DelegateResult DelegateResult { get; set; }

        /// <summary>
        /// Gets or sets the DelegateVotesResult
        /// </summary>
        public DelegateResult DelegateVotesResult { get; set; }
    }
}