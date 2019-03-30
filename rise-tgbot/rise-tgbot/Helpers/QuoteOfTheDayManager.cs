using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace rise_tgbot.Helpers
{
    public static class QuoteOfTheDayManager
    {
        public static async Task<string> GetQuoteOfTheDay()
        {
            try
            {
                // Retreive Quote
                using (HttpClient hc = new HttpClient())
                {
                    return await hc.GetStringAsync("https://geek-jokes.sameerkumar.website/api");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Error fetching Quote of the Day " + ex.Message);
                return null;
            }
        }
    }
}