namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="DelegateVotersFetcherFetcher" />
    /// </summary>
    public static class DelegateVotersFetcher
    {
        /// <summary>
        /// The FetchVoters
        /// </summary>
        /// <param name="publickey">The publickey<see cref="string"/></param>
        /// <returns>The <see cref="Task{VotersResult}"/></returns>
        public static async Task<VotersResult> FetchVoters(string username)
        {
            VotersResult votersResult;

            try
            {
                // Retreive Quote
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates/voters?username=" + username));
                    votersResult = JsonConvert.DeserializeObject<VotersResult>(result.ToString());

                    return votersResult.Success ? votersResult : null;
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