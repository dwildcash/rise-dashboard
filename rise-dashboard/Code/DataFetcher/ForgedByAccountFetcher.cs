namespace rise.Code.DataFetcher
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ForgedByAccountFetcher" />
    /// </summary>
    public static class ForgedByAccountFetcher
    {
        /// <summary>
        /// The FetchDelegates
        /// </summary>
        /// <returns>The <see cref="Task{DelegateResult}"/></returns>
        public static async Task<ForgedByAccount> FetchForgedByAccount(string generatorPublicKey)
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/delegates/forging/getForgedByAccount?generatorPublicKey=" + generatorPublicKey));
                    var forgedByAccountResult = JsonConvert.DeserializeObject<ForgedByAccount>(result.ToString());

                    return forgedByAccountResult;
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