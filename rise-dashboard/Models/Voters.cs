namespace rise.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Voters" />
    /// </summary>
    public class Voters
    {
        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the PublicKey
        /// </summary>
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the Balance
        /// </summary>
        [JsonProperty("balance")]
        public long Balance { get; set; }
    }


    /// <summary>
    /// Defines the <see cref="VotersResult" />
    /// </summary>
    public class VotersResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether Success
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the Accounts
        /// </summary>
        [JsonProperty("accounts")]
        public IList<Voters> Accounts { get; set; }
    }
}