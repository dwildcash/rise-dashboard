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
        /// Xt receive Address
        /// </summary>
        public static string XtDepositAddress { get; set; }


        /// <summary>
        /// Xt Send Address
        /// </summary>
        public static string XtWithdrawalAddress { get; set; }

        /// <summary>
        /// Encryption Salt
        /// </summary>
        public static string Salt { get; set; }

        /// <summary>
        /// Set the EncryptionKey
        /// </summary>
        public static string EncryptionKey { get; set; }

        /// <summary>
        /// Telegram Bot Name
        /// </summary>
        public static string BotName { get; set; }

        /// <summary>
        /// Get or Sets SiteURl
        /// </summary>
        public static string SiteUrl { get; set; }

        /// <summary>
        /// Set the Telegram Channel Id
        /// </summary>
        public static long TelegramChannelId { get; set; }

        /// <summary>
        /// Define the Telegram Bot Key
        /// </summary>
        public static string BotApiKey { get; set; }

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
        /// Secret used for Telegram webhook
        /// </summary>
        public static string WebHookSecret { get; set; }
    }
}