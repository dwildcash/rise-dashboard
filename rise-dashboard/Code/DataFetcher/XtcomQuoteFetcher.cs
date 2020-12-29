namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="XtcomQuoteFetcher" />
    /// </summary>
    public static class XtcomQuoteFetcher
    {
        public static async Task<XtcomQuote> FetchXtcomQuote()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://api.xt.com/data/api/v1/getTicker?market=rise_usdt"));
                    var xtcomQuoteResult = JsonConvert.DeserializeObject<XtcomQuote>(quote.ToString());

                    if (xtcomQuoteResult.moneyVol.ToString().Length > 0)
                    {
                        return xtcomQuoteResult;
                    }

                    return null;
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