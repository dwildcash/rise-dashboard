namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Bitker Quote Fetcher
    /// </summary>
    public static class BitkerQuoteFetcher
    { 
        public static async Task<BitkerQuote> FetBitkerCoinQuote()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://api.bitker.com/market/history/kline?symbol=rise_btc&period=1min&size=2&apikey=" + AppSettingsProvider.BitkerApiKey));
                    var BitkerQuoteResult = JsonConvert.DeserializeObject<BitkerQuoteResult>(quote.ToString());

                    return BitkerQuoteResult.status == "ok" ? BitkerQuoteResult.tick.First(): null;
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