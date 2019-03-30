using rise_tgbot.Helpers;
using System;

namespace rise_tgbot.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int TelegramId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Secret { get; set; }
        public string PublicKey { get; set; }
        public int MessageCount { get; set; }
        public string Role { get; set; }
        public DateTime LastMessage { get; set; }

        /// <summary>
        /// Return Unencrypted secret;
        /// </summary>
        /// <returns></returns>
        public string GetSecret()
        {
            return CryptoManager.DecryptStringAES(Secret, AppSettingsProvider.EncryptionKey);
        }
    }
}