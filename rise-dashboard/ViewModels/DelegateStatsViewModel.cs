namespace rise.ViewModels
{
    using rise.Code.Rise;
    using rise.Models;

    /// <summary>
    /// Defines the <see cref="DelegateStatsViewModel" />
    /// </summary>
    public class DelegateStatsViewModel : QuoteStats
    {
        /// <summary>
        /// Gets or sets the DelegateResult
        /// </summary>
        public DelegateResult DelegateResult { get; set; }

        /// <summary>
        /// Gets or sets the VotersResult
        /// </summary>
        public VotersResult VotersResult { get; set; }

        /// <summary>
        /// Gets or sets the TransactionsResult
        /// </summary>
        public TransactionsResult TransactionsResult { get; set; }
    }
}