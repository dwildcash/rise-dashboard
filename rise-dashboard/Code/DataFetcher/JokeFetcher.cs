namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using static rise.Models.Joke;

    /// <summary>
    /// Defines the <see cref="JokeFetcher" />
    /// </summary>
    public static class JokeFetcher
    {
        /// <summary>
        /// The FetchPeers
        /// </summary>
        /// <returns>The <see cref="Task{PeersResult}"/></returns>
        public static async Task<JokeResult> FetchJoke()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync("https://icanhazdadjoke.com/slack"));
                    var JokeResult = JsonConvert.DeserializeObject<JokeResult>(result.ToString());
                    return JokeResult;
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