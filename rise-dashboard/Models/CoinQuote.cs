namespace rise.Models
{
    using System;
    using System.Collections.Generic;

    public class CoinQuoteResult
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static List<CoinQuote> Current { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="CoinQuote" />
    /// </summary>
    public class CoinQuote
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the TimeStamp
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the Exchange
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// Gets or sets the Price
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets the Volume
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Gets or sets the USDPrice
        /// </summary>
        public double USDPrice { get; set; }
    }
}