namespace rise.Models
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Application Roles
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; set; }
    }
}