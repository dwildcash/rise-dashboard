namespace rise.Models
{
    /// <summary>
    /// Defines the <see cref="CoinbaseBtcQuote" />
    /// </summary>
    public class CoinbaseBtcQuote
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static CoinbaseBtcQuote Current { get; set; }

        /// <summary>
        /// Gets or sets the base
        /// </summary>
        public string @base { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public string amount { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="CoinbaseBtcQuoteResult" />
    /// </summary>
    public class CoinbaseBtcQuoteResult
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public CoinbaseBtcQuote data { get; set; }
    }
}