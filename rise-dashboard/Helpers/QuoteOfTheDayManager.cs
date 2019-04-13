using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace rise.Helpers
{
    public static class QuoteOfTheDayManager
    {
        public static async Task<string> GetQuoteOfTheDay()
        {
            try
            {
                // Retreive Quote
                using (var hc = new HttpClient())
                {
                    return await hc.GetStringAsync("https://geek-jokes.sameerkumar.website/api");
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