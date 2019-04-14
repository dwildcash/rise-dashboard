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
                using (HttpClient hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?orderBy=rank:asc"));
                    var delegateResult = JsonConvert.DeserializeObject<DelegateResult>(result.ToString());

                    var result200to399 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?orderBy=rank:asc&offset=199"));
                    var delegate200to399 = JsonConvert.DeserializeObject<DelegateResult>(result200to399.ToString());

                    var result400to599 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?orderBy=rank:asc&offset=398"));
                    var delegate400to599 = JsonConvert.DeserializeObject<DelegateResult>(result400to599.ToString());

                    var result600to799 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?orderBy=rank:asc&offset=597"));
                    var delegate600to799 = JsonConvert.DeserializeObject<DelegateResult>(result600to799.ToString());

                    var result800to999 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates?orderBy=rank:asc&offset=796"));
                    var delegate800to999 = JsonConvert.DeserializeObject<DelegateResult>(result800to999.ToString());

                    // Merge Delegates 200 to 399
                    foreach (var o in delegate200to399.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    // Merge Delegates 400 to 599
                    foreach (var o in delegate400to599.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    // Merge Delegates 600 to 799
                    foreach (var o in delegate600to799.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    // Merge Delegates 800 to 999
                    foreach (var o in delegate800to999.Delegates)
                    {
                        delegateResult.Delegates.Add(o);
                    }

                    return delegateResult.Success ? delegateResult : null;
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