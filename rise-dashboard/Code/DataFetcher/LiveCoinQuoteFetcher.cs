namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using rise.Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="LiveCoinQuoteFetcher" />
    /// </summary>
    public static class LiveCoinQuoteFetcher
    {
        /// <summary>
        /// The FetchLiveCoinQuote
        /// </summary>
        /// <returns>The <see cref="Task{LiveCoinQuote}"/></returns>
        public static async Task<LiveCoinQuote> FetchLiveCoinQuote()
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://api.livecoin.net/exchange/ticker?currencyPair=" + AppSettingsProvider.LiveCoinMarket));
                    var LiveCoinQuoteResult = JsonConvert.DeserializeObject<LiveCoinQuote>(quote.ToString());

                    return LiveCoinQuoteResult.Cur != null ? LiveCoinQuoteResult : null;
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