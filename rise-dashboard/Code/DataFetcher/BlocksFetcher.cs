namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using rise.Models;
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
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
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