namespace rise.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="WalletAccount" />
    /// </summary>
    public class WalletAccount
    {
        /// <summary>
        /// Gets or sets the Address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Get or Set the secret
        /// </summary>
        public string secret { get; set; }

        /// <summary>
        /// Gets or sets the Balance
        /// </summary>
        [JsonProperty("balance")]
        public string Balance { get; set; }


        /// <summary>
        /// Gets or sets the Multisignatures
        /// </summary>
        [JsonProperty("multisignatures")]
        public List<object> Multisignatures { get; set; }

        /// <summary>
        /// Gets or sets the PublicKey
        /// </summary>
        [JsonProperty("forgingPK")]
        public string ForgingPK { get; set; }

        /// <summary>
        /// Gets or sets the SecondPublicKey
        /// </summary>
        [JsonProperty("secondPublicKey")]
        public string SecondPublicKey { get; set; }

        /// <summary>
        /// Gets or sets the SecondSignature
        /// </summary>
        [JsonProperty("secondSignature")]
        public bool SecondSignature { get; set; }

        /// <summary>
        /// Gets or sets the UnconfirmedBalance
        /// </summary>
        [JsonProperty("unconfirmedBalance")]
        public string UnconfirmedBalance { get; set; }

        /// <summary>
        /// Gets or sets the UnconfirmedSignature
        /// </summary>
        [JsonProperty("unconfirmedSignature")]
        public bool UnconfirmedSignature { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="WalletAccountResult" />
    /// </summary>
    public class WalletAccountResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether success
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// Gets or sets the account
        /// </summary>
        public WalletAccount account { get; set; }
    }
}