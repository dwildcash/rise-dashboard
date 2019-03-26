﻿namespace rise.Models
{
    /// <summary>
    /// Global Application Settings
    /// </summary>
    public static class AppSettingsProvider
    {
        /// <summary>
        /// Gets or sets the SimulateRoundCount
        /// Specify the numer of round to do to simulate forging
        /// </summary>
        public static int SimulateRoundCount { get; set; }

        /// <summary>
        /// Define the Telegram Bot Key
        /// </summary>
        public static string BotKey { get; set; }

        /// <summary>
        /// Define the Max Coin that can be forged
        /// </summary>
        public static int CoinRewardDay { get; set; }

        /// <summary>
        /// Gets or sets the CoinName
        /// </summary>
        public static string CoinName { get; set; }

        /// <summary>
        /// Gets or sets the CoinMainSite
        /// </summary>
        public static string CoinMainSite { get; set; }

        /// <summary>
        /// Gets or sets the CoinExplorer
        /// </summary>
        public static string CoinExplorer { get; set; }

        /// <summary>
        /// Gets or set MaxTransactionsToFetch
        /// </summary>
        public static int MaxTransactionsToFetch { get; set; }

        /// <summary>
        /// Gets or sets the CoinFullName
        /// </summary>
        public static string CoinFullName { get; set; }

        /// <summary>
        /// Gets or sets the CoinMarketCapTickerCode
        /// </summary>
        public static string CoinMarketCapTickerCode { get; set; }

        /// <summary>
        /// Gets or sets the LiveCoinMarket
        /// </summary>
        public static string LiveCoinMarket { get; set; }

        /// <summary>
        /// Gets or sets the RightBtcMarket
        /// </summary>
        public static string RightBtcMarket { get; set; }

        /// <summary>
        /// Gets or sets the APIUrl
        /// </summary>
        public static string APIUrl { get; set; }

        /// <summary>
        /// Gets or sets the DonationAddress
        /// </summary>
        public static string DonationAddress { get; set; }

        /// <summary>
        /// Gets or sets the IPStackApiKey
        /// </summary>
        public static string IPStackApiKey { get; set; }

        /// <summary>
        /// Gets or sets the CryptopiaWalletAddress
        /// </summary>
        public static string CryptopiaWalletAddress { get; set; }

        /// <summary>
        /// Gets or sets the SiteAdminKey to Manage Pool List
        /// </summary>
        public static string SiteAdminKey { get; set; }
    }
}