namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using rise.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IPsFetcher" />
    /// </summary>
    public static class IPsFetcher
    {
        /// <summary>
        /// The FetchIPGeoLocation
        /// </summary>
        /// <param name="Ip">The Ip<see cref="string"/></param>
        /// <returns>The <see cref="Task{IPData}"/></returns>
        public static async Task<IPData> FetchIPGeoLocation(string Ip)
        {
            // Make an api call and get response.
            try
            {
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync("http://api.ipstack.com/" + Ip + "?access_key=" + AppSettingsProvider.IPStackApiKey));
                    return JsonConvert.DeserializeObject<IPData>(result.ToString());
                }
            }
            catch (Exception e)
            {
                Console.Write(e.InnerException);
                return null;
            }
        }

        /// <summary>
        /// The GroupElements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullList">The fullList<see cref="List{T}"/></param>
        /// <param name="batchSize">The batchSize<see cref="int"/></param>
        /// <returns>The <see cref="IEnumerable{IEnumerable{T}}"/></returns>
        private static IEnumerable<IEnumerable<T>> GroupElements<T>(List<T> fullList, int batchSize)
        {
            int total = 0;
            while (total < fullList.Count)
            {
                yield return fullList.Skip(total).Take(batchSize);
                total += batchSize;
            }
        }
    }
}