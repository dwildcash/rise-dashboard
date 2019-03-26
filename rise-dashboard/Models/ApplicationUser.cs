namespace rise.Models
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Defines the <see cref="ApplicationUser" />
    /// </summary>
    public partial class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the WalletAddress
        /// Return the wallet Address
        /// </summary>
        public string WalletAddress { get; set; }
    }
}