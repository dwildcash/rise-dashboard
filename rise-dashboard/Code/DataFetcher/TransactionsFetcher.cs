namespace rise.Code.DataFetcher
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using rise.Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TransactionsFetcher" />
    /// </summary>
    public static class TransactionsFetcher
    {
        /// <summary>
        /// The FetchTransactions
        /// </summary>
        /// <param name="address">The address<see cref="string"/></param>
        /// <returns>The <see cref="Task{TransactionsResult}"/></returns>
        public static async Task<TransactionsResult> FetchTransactions(string address = "")
        {
            try
            {
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
                {
                    JObject result = null;

                    if (address?.Length == 0)
                    {
                        result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/transactions?orderBy=timestamp:desc&limit=" + AppSettingsProvider.MaxTransactionsToFetch));
                    }
                    else
                    {
                        result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/transactions?orderBy=timestamp:desc&limit=" + AppSettingsProvider.MaxTransactionsToFetch + "&recipientId=" + address));
                    }

                    var transactionsResult = JsonConvert.DeserializeObject<TransactionsResult>(result.ToString());

                    return transactionsResult.success ? transactionsResult : null;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.InnerException);
                return null;
            }
        }

        /// <summary>
        /// The FetchAllUserTransactions
        /// </summary>
        /// <param name="address">The address<see cref="string"/></param>
        /// <returns>The <see cref="Task{TransactionsResult}"/></returns>
        public static async Task<TransactionsResult> FetchAllUserTransactions(string address)
        {
            try
            {
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
                {
                    var result1 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/transactions?limit=" + AppSettingsProvider.MaxTransactionsToFetch + "&orderBy=timestamp:desc&recipientId=" + address));
                    var transactionsResult = JsonConvert.DeserializeObject<TransactionsResult>(result1.ToString());

                    var result2 = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/transactions?limit=" + AppSettingsProvider.MaxTransactionsToFetch + "&orderBy=timestamp:desc&senderId=" + address));
                    var transactionsResult2 = JsonConvert.DeserializeObject<TransactionsResult>(result2.ToString());

                    // Merge Incoming and Outgoin Transaction
                    foreach (var o in transactionsResult2.transactions)
                    {
                        transactionsResult.transactions.Add(o);
                    }

                    return transactionsResult.success ? transactionsResult : null;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.InnerException);
                return null;
            }
        }

        /// <summary>
        /// The FetchOutgoingTransactions
        /// </summary>
        /// <param name="address">The address<see cref="string"/></param>
        /// <returns>The <see cref="Task{TransactionsResult}"/></returns>
        public static async Task<TransactionsResult> FetchOutgoingTransactions(string address = "")
        {
            TransactionsResult transactionsResult = null;

            try
            {
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
                {
                    JObject result;

                    if (address?.Length == 0)
                    {
                        result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/transactions?limit=" + AppSettingsProvider.MaxTransactionsToFetch + "&orderBy=timestamp:desc"));
                    }
                    else
                    {
                        result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/transactions?limit=" + AppSettingsProvider.MaxTransactionsToFetch + "&orderBy=timestamp:desc&senderId=" + address));
                    }

                    transactionsResult = JsonConvert.DeserializeObject<TransactionsResult>(result.ToString());

                    return transactionsResult.success ? transactionsResult : null;
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