using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rise_tgbot.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace rise_tgbot.Helpers
{
    public static class QuoteManager
    {
        public static async Task<RiseQuote> GetRiseQuote()
        {
            try
            {
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync("https://rise.coinquote.io/api/getQuote"));
                    return JsonConvert.DeserializeObject<RiseQuote>(result.ToString());
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Error fetching Rise Quote " + ex.Message);
                return null;
            }
        }
    }
}