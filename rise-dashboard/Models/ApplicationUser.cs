namespace rise.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;

    /// <summary>
    /// Defines the <see cref="ApplicationUser" />
    /// </summary>
    public partial class ApplicationUser : IdentityUser
    {
        public int TelegramId { get; set; }
        public string Address { get; set; }
        public string Photo_Url { get; set; }
        public string EncryptedBip39 { get; set; }
        public string PublicKey { get; set; }
        public int MessageCount { get; set; }
        public string Role { get; set; }
        public DateTime LastMessage { get; set; }
    }
}