﻿namespace rise.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// rise Quote Object from ooobtc
    /// </summary>

    public class ooobtcQuote
    {
        public static ooobtcQuote Current { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("lastprice")]
        public string Lastprice { get; set; }

        [JsonProperty("bid")]
        public string Bid { get; set; }

        [JsonProperty("ask")]
        public long Ask { get; set; }

        [JsonProperty("volume")]
        public long Volume { get; set; }
    }

    public class ooobtcQuoteResult
    {
  
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public ooobtcQuote Data { get; set; }
    }
}