namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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
                using (var hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://api.livecoin.net/exchange/ticker?currencyPair=" + AppSettingsProvider.LiveCoinMarket));
                    var liveCoinQuoteResult = JsonConvert.DeserializeObject<LiveCoinQuote>(quote.ToString());

                    return liveCoinQuoteResult.Cur != null ? liveCoinQuoteResult : null;
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