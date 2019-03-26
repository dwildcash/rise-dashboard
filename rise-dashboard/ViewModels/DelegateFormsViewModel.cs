namespace rise.ViewModels
{
    using rise.Code.Rise;
    using rise.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="DelegateFormsViewModel" />
    /// </summary>
    public class DelegateFormsViewModel : QuoteStats
    {
        /// <summary>
        /// Gets or sets the DelegateResult
        /// </summary>
        public DelegateResult DelegateResult { get; set; }

        /// Gets or sets the DelegateForm
        /// </summary>
        public List<DelegateForm> DelegateForm { get; set; }

        /// <summary>
        /// Gets or sets the walletAccountResult
        /// </summary>
        public WalletAccountResult walletAccountResult { get; set; }
    }
}