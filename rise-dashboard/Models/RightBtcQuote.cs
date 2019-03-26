namespace rise.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// rise Quote Object ex:https://www.cryptopia.co.nz/api/GetMarket/rise_BTC
    /// </summary>
    public class RightBtcQuote
    {
        /// <summary>
        /// Gets or sets the Market
        /// </summary>
        [JsonProperty("market")]
        public string Market { get; set; }

        /// <summary>
        /// Gets or sets the Date
        /// </summary>
        [JsonProperty("date")]
        public long Date { get; set; }

        /// <summary>
        /// Gets or sets the Sell
        /// </summary>
        [JsonProperty("sell")]
        public long Sell { get; set; }

        /// <summary>
        /// Gets or sets the Buy
        /// </summary>
        [JsonProperty("buy")]
        public long Buy { get; set; }

        /// <summary>
        /// Gets or sets the High
        /// </summary>
        [JsonProperty("high")]
        public long High { get; set; }

        /// <summary>
        /// Gets or sets the Low
        /// </summary>
        [JsonProperty("low")]
        public long Low { get; set; }

        /// <summary>
        /// Gets or sets the Last
        /// </summary>
        [JsonProperty("last")]
        public long Last { get; set; }

        /// <summary>
        /// Gets or sets the Vol
        /// </summary>
        [JsonProperty("vol")]
        public long Vol { get; set; }

        /// <summary>
        /// Gets or sets the Last24H
        /// </summary>
        [JsonProperty("last24h")]
        public long Last24H { get; set; }

        /// <summary>
        /// Gets or sets the High24H
        /// </summary>
        [JsonProperty("high24h")]
        public long High24H { get; set; }

        /// <summary>
        /// Gets or sets the Low24H
        /// </summary>
        [JsonProperty("low24h")]
        public long Low24H { get; set; }

        /// <summary>
        /// Gets or sets the Vol24H
        /// </summary>
        [JsonProperty("vol24h")]
        public long Vol24H { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="RightBtcQuoteResult" />
    /// </summary>
    public class RightBtcQuoteResult
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static RightBtcQuoteResult Current { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        [JsonProperty("status")]
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the Result
        /// </summary>
        [JsonProperty("result")]
        public RightBtcQuote Result { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Status" />
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Gets or sets the Success
        /// </summary>
        [JsonProperty("success")]
        public long Success { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        [JsonProperty("message")]
        public object Message { get; set; }
    }
}