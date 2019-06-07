namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="PeersFetcher" />
    /// </summary>
    public static class PeersFetcher
    {
        /// <summary>
        /// The FetchPeers
        /// </summary>
        /// <returns>The <see cref="Task{PeersResult}"/></returns>
        public static async Task<PeersResult> FetchPeers()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/peers"));
                    var peersResult = JsonConvert.DeserializeObject<PeersResult>(result.ToString());
                    return peersResult.Success ? peersResult : null;
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