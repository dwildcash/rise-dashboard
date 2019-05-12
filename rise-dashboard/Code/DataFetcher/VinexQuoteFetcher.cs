namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="VinexQuoteFetcher" />
    /// </summary>
    public static class VinexQuoteFetcher
    {
        public static async Task<VinexQuote> FetchVinexCoinQuote()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://api.vinex.network/api/v2/get-ticker?market=" + AppSettingsProvider.VinexMarket));
                    var VinexQuoteResult = JsonConvert.DeserializeObject<VinexQuoteResult>(quote.ToString());

                    return VinexQuoteResult.data;
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