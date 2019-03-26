namespace rise.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Represent the Delegate Object ex: https://wallet.rise.vision/api/delegates/get?publicKey=1403dfc0d8e5ffc09a4d3fd34a25f5c385103f28bda840dea9824f5c7dfc6136
    /// </summary>
    public class Delegate
    {
        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Add a Chance on max 5000
        /// </summary>
        public double AddChance { get; set; }

        public double Chance2 { get; set; }

        /// <summary>
        /// Return The forging Probability based on SimulatedRound
        /// </summary>
        public double ForgingChance
        {
            get { return AddChance / (double)AppSettingsProvider.SimulateRoundCount * (double)100; }
        }

        /// <summary>
        /// Gets or sets the Address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the CMB
        /// </summary>
        [JsonProperty("cmb")]
        public int CMB { get; set; }

        /// <summary>
        /// Gets or sets the PublicKey
        /// </summary>
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the Vote
        /// </summary>
        [JsonProperty("vote")]
        public long Vote { get; set; }

        /// <summary>
        /// Gets or sets the Producedblocks
        /// </summary>
        [JsonProperty("producedblocks")]
        public int Producedblocks { get; set; }

        /// <summary>
        /// Gets or sets the Missedblocks
        /// </summary>
        [JsonProperty("missedblocks")]
        public int Missedblocks { get; set; }

        /// <summary>
        /// Gets or sets the Rate
        /// </summary>
        [JsonProperty("rate")]
        public int Rate { get; set; }

        /// <summary>
        /// Gets or sets the VotesWeight
        /// </summary>
        [JsonProperty("votesWeight")]
        public long VotesWeight { get; set; }

        /// <summary>
        /// Gets or sets the Rank
        /// </summary>
        [JsonProperty("rank")]
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the Approval
        /// </summary>
        [JsonProperty("approval")]
        public double Approval { get; set; }

        /// <summary>
        /// Gets or sets the Productivity
        /// </summary>
        [JsonProperty("productivity")]
        public double Productivity { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="DelegateResult" />
    /// </summary>
    public class DelegateResult
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static DelegateResult Current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Success
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the Delegates
        /// </summary>
        [JsonProperty("delegates")]
        public IList<Delegate> Delegates { get; set; }

        /// <summary>
        /// Gets or sets the TotalCount
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
}