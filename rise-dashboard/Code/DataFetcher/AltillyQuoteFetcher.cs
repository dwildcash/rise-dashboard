namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="LiveCoinQuoteFetcher" />
    /// </summary>
    public static class AltillyQuoteFetcher
    {
        /// <summary>
        /// Fetch AltillyQuote
        /// </summary>
        /// <returns></returns>
        public static async Task<AltillyQuote> FetchAltillyQuote()
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://api.altilly.com/api/public/Ticker/" + AppSettingsProvider.RightBtcMarket));
                    var AltillyQuoteResult = JsonConvert.DeserializeObject<AltillyQuote>(quote.ToString());

                    return AltillyQuoteResult;
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