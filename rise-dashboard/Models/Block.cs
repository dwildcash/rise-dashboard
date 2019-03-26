namespace rise.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Block" />
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the RowId
        /// </summary>
        [JsonProperty("rowId")]
        public long RowId { get; set; }

        /// <summary>
        /// Gets or sets the Version
        /// </summary>
        [JsonProperty("version")]
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the Timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        [JsonProperty("height")]
        public long Height { get; set; }

        /// <summary>
        /// Gets or sets the PreviousBlock
        /// </summary>
        [JsonProperty("previousBlock")]
        public string PreviousBlock { get; set; }

        /// <summary>
        /// Gets or sets the NumberOfTransactions
        /// </summary>
        [JsonProperty("numberOfTransactions")]
        public long NumberOfTransactions { get; set; }

        /// <summary>
        /// Gets or sets the TotalAmount
        /// </summary>
        [JsonProperty("totalAmount")]
        public long TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the TotalFee
        /// </summary>
        [JsonProperty("totalFee")]
        public long TotalFee { get; set; }

        /// <summary>
        /// Gets or sets the Reward
        /// </summary>
        [JsonProperty("reward")]
        public long Reward { get; set; }

        /// <summary>
        /// Gets or sets the PayloadLength
        /// </summary>
        [JsonProperty("payloadLength")]
        public long PayloadLength { get; set; }

        /// <summary>
        /// Gets or sets the PayloadHash
        /// </summary>
        [JsonProperty("payloadHash")]
        public string PayloadHash { get; set; }

        /// <summary>
        /// Gets or sets the GeneratorPublicKey
        /// </summary>
        [JsonProperty("generatorPublicKey")]
        public string GeneratorPublicKey { get; set; }

        /// <summary>
        /// Gets or sets the BlockSignature
        /// </summary>
        [JsonProperty("blockSignature")]
        public string BlockSignature { get; set; }

        /// <summary>
        /// Gets or sets the Transactions
        /// </summary>
        [JsonProperty("transactions")]
        public object[] Transactions { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="BlocksResult" />
    /// </summary>
    public class BlocksResult
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static BlocksResult Current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Success
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the Blocks
        /// </summary>
        [JsonProperty("blocks")]
        public Block[] Blocks { get; set; }

        /// <summary>
        /// Gets or sets the Count
        /// </summary>
        [JsonProperty("count")]
        public long Count { get; set; }
    }
}