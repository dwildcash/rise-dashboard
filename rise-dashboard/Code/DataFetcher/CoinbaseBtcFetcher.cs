﻿namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="CoinbaseBtcFetcher" />
    /// </summary>
    public static class CoinbaseBtcFetcher
    {
        /// <summary>
        /// The FetchCoinbaseBtcQuote
        /// </summary>
        /// <returns>The <see cref="Task{CoinbaseBtcQuoteResult}"/></returns>
        public static async Task<CoinbaseBtcQuote> FetchCoinbaseBtcQuote()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var quote = JObject.Parse(await hc.GetStringAsync("https://api.coinbase.com/v2/prices/BTC-USD/spot"));
                    var coinbaseQuoteResult = JsonConvert.DeserializeObject<CoinbaseBtcQuoteResult>(quote.ToString());

                    return coinbaseQuoteResult.data;
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