namespace rise.ViewModels
{
    using rise.Code.Rise;
    using rise.Models;

    /// <summary>
    /// Defines the <see cref="AccountSummaryViewModel" />
    /// </summary>
    public class AccountSummaryViewModel : QuoteStats
    {
        /// <summary>
        /// Gets or sets the liveCoinQuoteResult
        /// </summary>
        public LiveCoinQuote liveCoinQuoteResult { get; set; }

        /// <summary>
        /// Gets or sets the coinbaseBtcQuoteResult
        /// </summary>
        public CoinbaseBtcQuote coinbaseBtcQuoteResult { get; set; }

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
        /// Gets or sets the coinReceivedByAccount
        /// </summary>
        public TransactionsResult coinReceivedByAccount { get; set; }

        /// <summary>
        /// Gets or sets the coinSentByAccount
        /// </summary>
        public TransactionsResult coinSentByAccount { get; set; }

        /// <summary>
        /// Gets or sets the Forged by Account
        /// </summary>
        public ForgedByAccount forgedByAccount { get; set; }
    }
}