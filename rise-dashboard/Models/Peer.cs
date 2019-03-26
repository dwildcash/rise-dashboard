namespace rise.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Peer" />
    /// </summary>
    public class Peer
    {
        /// <summary>
        /// Gets or sets the Ip
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets the Port
        /// </summary>
        [JsonProperty("port")]
        public long Port { get; set; }

        /// <summary>
        /// Gets or sets the State
        /// </summary>
        [JsonProperty("state")]
        public long State { get; set; }

        /// <summary>
        /// Gets or sets the Os
        /// </summary>
        [JsonProperty("os")]
        public string Os { get; set; }

        /// <summary>
        /// Gets or sets the Version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the Broadhash
        /// </summary>
        [JsonProperty("broadhash")]
        public string Broadhash { get; set; }

        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        [JsonProperty("height")]
        public long Height { get; set; }

        /// <summary>
        /// Gets or sets the Clock
        /// </summary>
        [JsonProperty("clock")]
        public object Clock { get; set; }

        /// <summary>
        /// Gets or sets the Updated
        /// </summary>
        [JsonProperty("updated")]
        public long Updated { get; set; }

        /// <summary>
        /// Gets or sets the Nonce
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the Lattitude
        /// </summary>
        public double Lattitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Contry
        /// </summary>
        public string Contry { get; set; }

        /// <summary>
        /// Gets or sets the ISP
        /// </summary>
        public string ISP { get; set; }

        /// <summary>
        /// Gets or sets the City
        /// </summary>
        public string City { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="PeersResult" />
    /// </summary>
    public class PeersResult
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static PeersResult Current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Success
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the Peers
        /// </summary>
        [JsonProperty("peers")]
        public Peer[] Peers { get; set; }
    }
}