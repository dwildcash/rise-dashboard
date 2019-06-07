namespace rise.ViewModels
{
    using rise.Code.Rise;
    using rise.Models;

    /// <summary>
    /// Defines the <see cref="QuoteViewModel" />
    /// </summary>
    public class QuoteViewModel : QuoteStats
    {
        /// <summary>
        /// Gets or sets the liveCoinQuoteResult
        /// </summary>
        public LiveCoinQuote liveCoinQuoteResult { get; set; }

        /// <summary>
        /// Gets or sets the walletAccountResult
        /// </summary>
        public WalletAccountResult walletAccountResult { get; set; }

        /// <summary>
        /// Gets or sets the transactionsResult
        /// </summary>
        public TransactionsResult transactionsResult { get; set; }

        /// <summary>
        /// Gets or sets the delegateResult
        /// </summary>
        public DelegateResult delegateResult { get; set; }

        /// <summary>
        /// Gets or sets the delegateVotesResult
        /// </summary>
        public DelegateResult delegateVotesResult { get; set; }

        /// <summary>
        /// Gets or sets the coinbaseBtcQuoteResult
        /// </summary>
        public CoinbaseBtcQuoteResult coinbaseBtcQuoteResult { get; set; }
    }
}