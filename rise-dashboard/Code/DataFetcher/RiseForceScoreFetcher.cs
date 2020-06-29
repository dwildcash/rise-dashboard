namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using rise.Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="RiseForceScoreFetcher" />
    /// </summary>
    public static class RiseForceScoreFetcher
    {

        /// <summary>
        /// FetchRiseForceStats
        /// </summary>
        /// <returns></returns>
        public static async Task<RiseForceApplication> FetchRiseForceStats()
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    var riseforestats = JObject.Parse(await hc.GetStringAsync("https://riseforce.rise.vision/ext/stats?action=seasonStats"));
                    var RiseForce = JsonConvert.DeserializeObject<RiseForceApplication>(riseforestats.ToString());

                    return RiseForce;
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