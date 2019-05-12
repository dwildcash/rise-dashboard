namespace rise.Models
{
    using System;

    /// <summary>
    /// rise Quote Object ex:https://api.altilly.com/api/public/Ticker/RISEBTC
    /// </summary>
    public class AltillyQuote
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static AltillyQuote Current { get; set; }

        public string ask { get; set; }
        public string bid { get; set; }
        public string last { get; set; }
        public string open { get; set; }
        public string low { get; set; }
        public string high { get; set; }
        public string volume { get; set; }
        public string volumeQuote { get; set; }
        public DateTime timestamp { get; set; }
        public string symbol { get; set; }
    }
}