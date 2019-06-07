namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="QuoteOfTheDayFetcher" />
    /// </summary>
    public static class QuoteOfTheDayFetcher
    {
        /// <summary>
        /// The FetchQuoteOfTheDay
        /// </summary>
        /// <returns>The <see cref="Task{QuoteOfTheDayResult}"/></returns>
        public static async Task<QuoteOfTheDayResult> FetchQuoteOfTheDay()
        {
            try
            {
                // Retrieve Quote Of the Day.
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync("http://quotes.rest/qod.json?category=inspire"));
                    var quoteOfTheDayResult = JsonConvert.DeserializeObject<QuoteOfTheDayResult>(result.ToString());

                    return quoteOfTheDayResult != null ? quoteOfTheDayResult : null;
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