namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="DelegateFetcher" />
    /// </summary>
    public static class DelegateFetcher
    {
        /// <summary>
        /// The FetchDelegates
        /// </summary>
        /// <returns>The <see cref="Task{DelegateResult}"/></returns>
        public static async Task<DelegateResult> FetchDelegates()
        {
            try
            {
                // Retreive Quote
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates"));
                    var delegateResult = JsonConvert.DeserializeObject<DelegateResult>(result.ToString());

                    var result200to398 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?offset=199"));
                    var delegate200to398 = JsonConvert.DeserializeObject<DelegateResult>(result200to398.ToString());

                    // Merge Delegates 200 to 399
                    foreach (var o in delegate200to398.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    return delegateResult.Success && delegate200to398.Success ? delegateResult : null;
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