namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class ChuckNorrisJokeFetcher
    {
        public static async Task<ChuckNorrisJoke> FetchChuckNorrisJoke()
        {
            try
            {
                // Retrieve Quote Of the Day.
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync("https://api.chucknorris.io/jokes/random"));
                    var jokeResult = JsonConvert.DeserializeObject<ChuckNorrisJoke>(result.ToString());

                    return jokeResult != null ? jokeResult : null;
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