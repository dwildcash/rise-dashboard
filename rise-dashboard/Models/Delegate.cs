namespace rise.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Delegate Infos
    /// </summary>
    public class DelegateInfos
    {
        public int rankV1 { get; set; }
        public int rankV2 { get; set; }
        public double approval { get; set; }
        public double productivity { get; set; }
    }

    /// <summary>
    /// Represent the Delegate Object ex: https://wallet.rise.vision/api/delegates/get?publicKey=1403dfc0d8e5ffc09a4d3fd34a25f5c385103f28bda840dea9824f5c7dfc6136
    /// </summary>
    public class Delegate
    {

        /// <summary>
        /// Add a Chance on max 5000
        /// </summary>
        public double AddChance { get; set; }

        public double Chance2 { get; set; }

        /// <summary>
        /// Return The forging Probability based on SimulatedRound
        /// </summary>
        public double ForgingChance => AddChance / AppSettingsProvider.SimulateRoundCount * 100;
        
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// </summary>
        [JsonProperty("cmb")]
        public int Cmb { get; set; }

        /// </summary>
        [JsonProperty("forgingPK")]
        public string ForgingPK { get; set; }

        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// </summary>
        [JsonProperty("vote")]
        public long Vote { get; set; }

        /// </summary>
        [JsonProperty("votesWeight")]
        public long VotesWeight { get; set; }

        /// </summary>
        [JsonProperty("producedblocks")]
        public int Producedblocks { get; set; }

        /// </summary>
        [JsonProperty("missedblocks")]
        public int Missedblocks { get; set; }

        /// </summary>
        [JsonProperty("infos")]
        public DelegateInfos Infos { get; set; }

        /// <summary>
        /// Get or set Number of voters
        /// </summary>
        public int? Voters { get; set; }
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