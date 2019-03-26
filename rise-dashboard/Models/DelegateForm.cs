namespace rise.Models
{
    /// <summary>
    /// Defines the <see cref="DelegateForm" />
    /// </summary>
    public class DelegateForm
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or set Delegate Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the share
        /// </summary>
        public double Share { get; set; }

        /// <summary>
        /// Gets or sets the payout_address
        /// </summary>
        public string Payout_address { get; set; }

        /// <summary>
        /// Gets or sets the min_payout
        /// </summary>
        public double Min_payout { get; set; }

        /// <summary>
        /// Gets or sets the payout_interval
        /// </summary>
        public int Payout_interval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fees_covered
        /// </summary>
        public bool Fees_covered { get; set; }

        /// <summary>
        /// Gets or sets the contact
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// Gets or sets the contact_type
        /// </summary>
        public string Contact_type { get; set; }

        /// <summary>
        /// Reason to votes for the delegate
        /// </summary>
        public string Notes { get; set; }
    }
}