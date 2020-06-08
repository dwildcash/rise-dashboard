namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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

                    var result200to399 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?offset=199"));
                    var delegate200to399 = JsonConvert.DeserializeObject<DelegateResult>(result200to399.ToString());

                    var result399to599 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?offset=399"));
                    var delegate399to599 = JsonConvert.DeserializeObject<DelegateResult>(result399to599.ToString());

                    var result499to699 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?offset=499"));
                    var delegate499to699 = JsonConvert.DeserializeObject<DelegateResult>(result399to599.ToString());

                    // Merge Delegates 200 to 399
                    foreach (var o in delegate200to399.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    foreach (var o in delegate399to599.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    foreach (var o in delegate499to699.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    return delegateResult.Success && delegate200to399.Success && delegate399to599.Success ? delegateResult : null;
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