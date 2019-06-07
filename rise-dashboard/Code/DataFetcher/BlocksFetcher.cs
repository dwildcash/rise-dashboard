namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="BlocksFetcher" />
    /// </summary>
    public static class BlocksFetcher
    {
        /// <summary>
        /// The FetchBlocks
        /// </summary>
        /// <returns>The <see cref="Task{BlocksResult}"/></returns>
        public static async Task<BlocksResult> FetchBlocks()
        {
            try
            {
                // Retrieve Quote
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/blocks/"));
                    var blocksResult = JsonConvert.DeserializeObject<BlocksResult>(result.ToString());

                    return blocksResult.Success ? blocksResult : null;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.InnerException);
                return null;
            }
        }
    }
}