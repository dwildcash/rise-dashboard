using dotnetstandard_bip39;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rise_lib.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace rise_lib
{
    public static class RiseManager
    {
        /// <summary>
        /// Create an Account
        /// </summary>
        /// <returns></returns>
        public static async Task<AccountResult> CreateAccount()
        {
            try
            {
                // Generate Secret
                BIP39 bip39 = new BIP39();
                string passphrase = bip39.GenerateMnemonic(128, BIP39Wordlist.English);

                // Create Accounts
                using (HttpClient hc = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                         { "secret", passphrase }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = await hc.PostAsync("http://localhost:6990/api/accounts/open", content);
                    string responseString = await response.Content.ReadAsStringAsync();
                    var accountResult = JsonConvert.DeserializeObject<AccountResult>(responseString);
                    accountResult.account.secret = passphrase;

                    return accountResult.success ? accountResult : null;
                }
            }
            catch (Exception e)
            {
                Console.Write("Error creating Account Exception " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Create an Account
        /// </summary>
        /// <returns></returns>
        public static async Task<Transaction> CreatePaiment(double amount, string secret, string recipiendId)
        {
            try
            {
                // Create Paiment
                using (HttpClient hc = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                         { "secret", secret },
                         { "amount", amount.ToString() },
                         { "recipientId", recipiendId }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = await hc.PutAsync("http://localhost:6990/api/transactions", content);
                    string responseString = await response.Content.ReadAsStringAsync();
                    var transaction = JsonConvert.DeserializeObject<Transaction>(responseString);

                    return transaction.success ? transaction : null;
                }
            }
            catch (Exception e)
            {
                Console.Write("Error Creating Transaction Amount:" + amount + " recipientId " + recipiendId + " Exception:" + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get account Balance
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static async Task<double> AccountBalanceAsync(string walletAddress)
        {
            WalletAccountResult walletAccountResult;

            try
            {
                // Get Account balance
                using (HttpClient hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync("http://localhost:5555/api/accounts?address=" + walletAddress));
                    walletAccountResult = JsonConvert.DeserializeObject<WalletAccountResult>(result.ToString());

                    return walletAccountResult.success ? double.Parse(walletAccountResult.account.Balance) / 100000000 : 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting balance from " + walletAddress + " Exception:" + e.Message);
                return 0;
            }
        }
    }
}