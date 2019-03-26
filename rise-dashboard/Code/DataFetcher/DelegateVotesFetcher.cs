﻿namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using rise.Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="DelegateVotesFetcher" />
    /// </summary>
    public static class DelegateVotesFetcher
    {
        /// <summary>
        /// The FetchRiseDelegateVotes
        /// </summary>
        /// <param name="address">The address<see cref="string"/></param>
        /// <returns>The <see cref="Task{DelegateResult}"/></returns>
        public static async Task<DelegateResult> FetchRiseDelegateVotes(string address)
        {
            try
            {
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/accounts/delegates?address=" + address));
                    var delegateResult = JsonConvert.DeserializeObject<DelegateResult>(result.ToString());

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