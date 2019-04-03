namespace rise.Models
{
    using Microsoft.AspNetCore.Identity;
    using rise.Helpers;
    using System;

    /// <summary>
    /// Defines the <see cref="ApplicationUser" />
    /// </summary>
    public partial class ApplicationUser : IdentityUser
    {
        public long TelegramId { get; set; }
        public string Address { get; set; }
        public string Photo_Url { get; set; }
        public long ChatId { get; set; }
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