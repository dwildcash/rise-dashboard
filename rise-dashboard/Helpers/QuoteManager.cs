﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rise.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace rise.Helpers
{
    public static class QuoteManager
    {
        public static async Task<CoinQuote> GetRiseQuote()
        {
            try
            {
                // Retreive Quote
                using (var hc = new HttpClient())
                {
                    var result = JObject.Parse(await hc.GetStringAsync("https://dashboard.rise.vision/api/getQuote"));
                    return JsonConvert.DeserializeObject<CoinQuote>(result.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}