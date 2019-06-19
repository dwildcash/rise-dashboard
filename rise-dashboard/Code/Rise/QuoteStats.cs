namespace rise.Code.Rise
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="QuoteStats" />
    /// </summary>
    public class QuoteStats
    {
        /// <summary>
        /// Gets or sets the coinQuoteCol
        /// Collection of all coins quote
        /// </summary>
        public List<CoinQuote> coinQuoteCol { get; set; }

        /// <summary>
        /// Return the Time From Genesis
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public DateTime FromGenesisTime(long unixTime)
        {
            var epoch = new DateTime(2016, 05, 24, 17, 5, 0, 0).ToLocalTime();
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Return Time to Genesis format
        /// </summary>
        /// <param name="minutesfromNow"></param>
        /// <returns></returns>
        public long ToGenesisTime(int minutesfromNow)
        {
            var epoch = new DateTime(2016, 05, 24, 17, 5, 0, 0).ToLocalTime();
            return Convert.ToInt64((DateTime.Now.AddMinutes(-minutesfromNow) - epoch).TotalSeconds);
        }

        /// <summary>
        /// Return a Collection of Last Quote
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public List<CoinQuote> CoinQuoteByDays(int days)
        {
            var query = from o in coinQuoteCol
                        join d in coinQuoteCol.Where(x => x.TimeStamp >= DateTime.Now.AddDays(-days)).GroupBy(x => new { y = x.TimeStamp.Day, m = x.TimeStamp.Month, d = x.TimeStamp.Day, x.Exchange }).Select(x => new CoinQuote { Exchange = x.Key.Exchange, TimeStamp = x.Max(i => i.TimeStamp) })
            on new { o.Exchange, o.TimeStamp } equals new { d.Exchange, d.TimeStamp }
                        select new CoinQuote { Exchange = o.Exchange, Price = o.Price, Volume = o.Volume, TimeStamp = o.TimeStamp, USDPrice = o.USDPrice };

            return query.ToList();
        }

        /// <summary>
        /// Return the highest price in Xday
        /// </summary>
        /// <param name="days">The days<see cref="int"/></param>
        /// <returns></returns>
        public double DaysHigh(int days)
        {
            var quoteXdays = coinQuoteCol.Where(x => x.Exchange == "All" && x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-days)).OrderByDescending(i => i.Price).FirstOrDefault();

            if (quoteXdays == null)
            {
                return 0;
            }

            return quoteXdays.Price;
        }

        /// <summary>
        /// Return the highest price in Xday
        /// </summary>
        /// <param name="days">The days<see cref="int"/></param>
        /// <returns></returns>
        public double DaysLow(int days)
        {
            var quoteXdays = coinQuoteCol.Where(x => x.Exchange == "All" && x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-days)).OrderBy(i => i.Price).FirstOrDefault();

            if (quoteXdays == null)
            {
                return 0;
            }

            return quoteXdays.Price;
        }

        /// <summary>
        /// Return the Price % Change vs X days
        /// </summary>
        /// <param name="days">The days<see cref="int"/></param>
        /// <returns></returns>
        public double PercentChange(int days)
        {
            var quoteXdays = coinQuoteCol.Where(x => x.Exchange == "All" && x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-days)).OrderBy(x => x.TimeStamp).FirstOrDefault();
            var quoteLast = LastAllQuote();

            if (quoteXdays == null)
            {
                return 0;
            }

            if (quoteXdays.Price > quoteLast.Price)
            {
                return Math.Round(-(100 - (quoteLast.Price / quoteXdays.Price * 100)), 2);
            }
            else
            {
                return Math.Round(100 - (quoteXdays.Price / quoteLast.Price * 100), 2);
            }
        }

        /// <summary>
        /// Return the Volume % Change vs X days
        /// </summary>
        /// <param name="days">The days<see cref="int"/></param>
        /// <returns></returns>
        public double VolumePercentChange(int days)
        {
            var quoteXdays = coinQuoteCol.Where(x => x.Exchange == "All" && x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-days)).OrderBy(x => x.TimeStamp).FirstOrDefault();
            var quoteLast = LastAllQuote();

            if (quoteXdays == null)
            {
                return 0;
            }

            if (quoteXdays.Volume > quoteLast.Volume)
            {
                return Math.Round(-(100 - (quoteLast.Volume / quoteXdays.Volume * 100)), 2);
            }
            else
            {
                return Math.Round(100 - (quoteXdays.Volume / quoteLast.Volume * 100), 2);
            }
        }

        /// <summary>
        /// Return the Volume % Change vs X days
        /// </summary>
        /// <param name="days">The days<see cref="int"/></param>
        /// <returns></returns>
        public double USDPricePercentChange(int days)
        {
            var quoteXdays = coinQuoteCol.Where(x => x.Exchange == "All" && x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-days)).OrderBy(x => x.TimeStamp).FirstOrDefault();
            var quoteLast = LastAllQuote();

            if (quoteXdays == null)
            {
                return 0;
            }

            if (quoteXdays.USDPrice > quoteLast.USDPrice)
            {
                return Math.Round(-(100 - (quoteLast.USDPrice / quoteXdays.USDPrice * 100)), 2);
            }
            else
            {
                return Math.Round(100 - (quoteXdays.USDPrice / quoteLast.USDPrice * 100), 2);
            }
        }

        /// <summary>
        /// Return The Last Quote for all Exchanges
        /// </summary>
        /// <returns></returns>
        public CoinQuote LastAllQuote()
        {
            return coinQuoteCol.Where(x => x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-1) && x.Exchange == "All").OrderByDescending(x => x.TimeStamp).FirstOrDefault();
        }


        /// <summary>
        /// return Last Vinex Quote
        /// </summary>
        /// <returns></returns>
        public CoinQuote LastVinexQuote()
        {
            return coinQuoteCol.Where(x => x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-1) && x.Exchange == "Vinex").OrderByDescending(x => x.TimeStamp).FirstOrDefault();
        }


        /// <summary>
        /// Return last Altilly Quote
        /// </summary>
        /// <returns></returns>
        public CoinQuote LastAltillyQuote()
        {
            return coinQuoteCol.Where(x => x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-1) && x.Exchange == "Altilly").OrderByDescending(x => x.TimeStamp).FirstOrDefault();
        }


        /// <summary>
        /// Return last LiveCoin Quote
        /// </summary>
        /// <returns></returns>
        public CoinQuote LastLiveCoinQuote()
        {
            return coinQuoteCol.Where(x => x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-1) && x.Exchange == "LiveCoin").OrderByDescending(x => x.TimeStamp).FirstOrDefault();
        }

        public CoinQuote LastooobtcCoinQuote()
        {
            return coinQuoteCol.Where(x => x.TimeStamp >= DateTime.Now.ToUniversalTime().AddDays(-1) && x.Exchange == "ooobtc").OrderByDescending(x => x.TimeStamp).FirstOrDefault();
        }
    }
}