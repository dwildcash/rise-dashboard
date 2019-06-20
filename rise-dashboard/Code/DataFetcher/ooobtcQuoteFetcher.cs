namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ooobtcQuoteFetcher" />
    /// </summary>
    public static class ooobtcQuoteFetcher
    {
        public static async Task<ooobtcQuote> FetchooobtcCoinQuote()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://openapi.ooobtc.com/public/v1/getticker?kv=rise_btc"));
                    var ooobtcQuoteResult = JsonConvert.DeserializeObject<ooobtcQuoteResult>(quote.ToString());

                    return ooobtcQuoteResult.Status == 200 ? ooobtcQuoteResult.Data : null;
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