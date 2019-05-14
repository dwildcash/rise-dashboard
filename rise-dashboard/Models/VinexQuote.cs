namespace rise.Models
{
    /// <summary>
    /// rise Quote Object ex:https://api.altilly.com/api/public/Ticker/RISEBTC
    /// </summary>
    public class VinexQuote
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static VinexQuote Current { get; set; }

        public string symbol { get; set; }

        public double lastPrice { get; set; }

        public double highPrice { get; set; }

        public double baseVolume { get; set; }

        public double lowPrice { get; set; }

        public double volume { get; set; }

        public double quoteVolume { get; set; }

        public double change24h { get; set; }

        public double threshold { get; set; }

        public double bidPrice { get; set; }

        public double askPrice { get; set; }
    }

    public class VinexQuoteResult
    {
        public int status { get; set; }
        public VinexQuote data { get; set; }
    }
}