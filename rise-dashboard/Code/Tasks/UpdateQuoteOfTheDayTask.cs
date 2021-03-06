﻿using rise.Code.DataFetcher;
using rise.Code.Scheduling;
using rise.Models;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Code.Tasks
{
    /// <summary>
    /// Defines the <see cref="UpdateQuoteOfTheDayTask" />
    /// </summary>
    public class UpdateQuoteOfTheDayTask : IScheduledTask
    {
        /// <inheritdoc />
        /// <summary>
        /// Gets the Schedule
        /// </summary>
        public string Schedule => "*/60 * * * *";

        /// <summary>
        /// Fetch IP Localisation once a day
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var quoteOfTheDayResult = await QuoteOfTheDayFetcher.FetchQuoteOfTheDay();

            if (quoteOfTheDayResult != null)
            {
                QuoteOfTheDayResult.Current = quoteOfTheDayResult;
            }
        }
    }
}