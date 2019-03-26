using Newtonsoft.Json;

namespace rise.Models
{
    /// <summary>
    /// Defines the <see cref="ForgedByAccount" />
    /// </summary>
    public class ForgedByAccount
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("fees")]
        public long Fees { get; set; }

        [JsonProperty("forged")]
        public long Forged { get; set; }

        [JsonProperty("rewards")]
        public long Rewards { get; set; }
    }
}