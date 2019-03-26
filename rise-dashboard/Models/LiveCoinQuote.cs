namespace rise.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// rise Quote Object from LiveCoin.net
    /// </summary>
    public class LiveCoinQuote
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static LiveCoinQuote Current { get; set; }

        /// <summary>
        /// Gets or sets the Cur
        /// </summary>
        [JsonProperty("cur")]
        public string Cur { get; set; }

        /// <summary>
        /// Gets or sets the Symbol
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the Last
        /// </summary>
        [JsonProperty("last")]
        public double Last { get; set; }

        /// <summary>
        /// Gets or sets the High
        /// </summary>
        [JsonProperty("high")]
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the Low
        /// </summary>
        [JsonProperty("low")]
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the Volume
        /// </summary>
        [JsonProperty("volume")]
        public double Volume { get; set; }

        /// <summary>
        /// Gets or sets the Vwap
        /// </summary>
        [JsonProperty("vwap")]
        public double Vwap { get; set; }

        /// <summary>
        /// Gets or sets the MaxBid
        /// </summary>
        [JsonProperty("max_bid")]
        public double MaxBid { get; set; }

        /// <summary>
        /// Gets or sets the MinAsk
        /// </summary>
        [JsonProperty("min_ask")]
        public double MinAsk { get; set; }

        /// <summary>
        /// Gets or sets the BestBid
        /// </summary>
        [JsonProperty("best_bid")]
        public double BestBid { get; set; }

        /// <summary>
        /// Gets or sets the BestAsk
        /// </summary>
        [JsonProperty("best_ask")]
        public double BestAsk { get; set; }
    }
}