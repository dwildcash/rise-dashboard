using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace rise.Models
{

    using System;

    public partial class RiseForce
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static RiseForce Current { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("result")]
        public RiseForceResult Result { get; set; }
    }


    public partial class RiseForceResult
    {

        [JsonProperty("season")]
        public long Season { get; set; }

        [JsonProperty("recordCount")]
        public long RecordCount { get; set; }

        [JsonProperty("sumScore")]
        public long SumScore { get; set; }

        [JsonProperty("avgScore")]
        public long AvgScore { get; set; }

        [JsonProperty("distinctPlayers")]
        public long DistinctPlayers { get; set; }

        [JsonProperty("winner")]
        public RiseForceWinner Winner { get; set; }

        [JsonProperty("top10")]
        public RiseForceTop10[] Top10 { get; set; }
    }

    public partial class RiseForceTop10
    {
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }
    }

    public partial class RiseForceWinner
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("season")]
        public long Season { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("riseAddress")]
        public string RiseAddress { get; set; }

        [JsonProperty("transId")]
        public string TransId { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("potSize")]
        public long PotSize { get; set; }
    }
}