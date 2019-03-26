namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using rise.Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="RightBtcQuoteFetcher" />
    /// </summary>
    public static class RightBtcQuoteFetcher
    {
        /// <summary>
        /// The FetchRightBtcQuote
        /// </summary>
        /// <returns>The <see cref="Task{RightBtcQuoteResult}"/></returns>
        public static async Task<RightBtcQuoteResult> FetchRightBtcQuote()
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://www.rightbtc.com/api/public/ticker/" + AppSettingsProvider.RightBtcMarket));
                    var rightBtcQuoteResult = JsonConvert.DeserializeObject<RightBtcQuoteResult>(quote.ToString());

                    return rightBtcQuoteResult.Status.Success == 1 ? rightBtcQuoteResult : null;
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