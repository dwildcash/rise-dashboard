using dotnetstandard_bip39;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rise.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Code.Rise
{
    public class RiseManager
    {
        /// <summary>
        /// Create an Account
        /// </summary>
        /// <returns></returns>
        public async Task<WalletAccountResult> CreateAccount()
        {
            try
            {
                // Generate Secret
                var bip39 = new BIP39();
                var passphrase = bip39.GenerateMnemonic(128, BIP39Wordlist.English);

                // Create Accounts
                using (var hc = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                         { "secret", passphrase }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = await hc.PostAsync("http://localhost:7777/api/accounts/open", content);
                    string responseString = await response.Content.ReadAsStringAsync();
                    var accountResult = JsonConvert.DeserializeObject<WalletAccountResult>(responseString);
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
        public async Task<Tx> CreatePaimentAsync(double amount, string secret, string recipiendId)
        {
            try
            {
                // Create Paiment
                using (var hc = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                        {
                            { "secret", secret },
                            { "amount", Math.Round(amount).ToString() },
                            { "recipientId", recipiendId }
                        };

                    var content = new FormUrlEncodedContent(values);
                    var response = await hc.PutAsync("http://localhost:7777/api/transactions", content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var transaction = JsonConvert.DeserializeObject<Tx>(responseString);

                    return transaction;
                }
            }
            catch (Exception e)
            {
                var tx = new Tx
                {
                    success = false
                };
                return tx;
            }
        }


        /// <summary>
        /// Get account Balance
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public async Task<double> AccountBalanceAsync(string walletAddress)
        {
            WalletAccountResult walletAccountResult;

            try
            {
                // Get Account balance
                using (var hc = new HttpClient())
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