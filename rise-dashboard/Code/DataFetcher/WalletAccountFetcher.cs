namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="WalletAccountFetcher" />
    /// </summary>
    public static class WalletAccountFetcher
    {
        /// <summary>
        /// The FetchRiseWalletAccount
        /// </summary>
        /// <param name="walletAddress">The walletAddress<see cref="string"/></param>
        /// <returns>The <see cref="Task{WalletAccountResult}"/></returns>
        public static async Task<WalletAccountResult> FetchRiseWalletAccount(string walletAddress)
        {
            try
            {
                // Retreive Quote
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/accounts?address=" + walletAddress));
                    var walletAccountResult = JsonConvert.DeserializeObject<WalletAccountResult>(result.ToString());

                    return walletAccountResult.success ? walletAccountResult : null;
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