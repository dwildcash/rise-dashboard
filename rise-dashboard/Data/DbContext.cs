namespace rise.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using rise.Models;

    /// <summary>
    /// Defines the <see cref="ApplicationDbContext" />
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="DbContextOptions{ApplicationDbContext}"/></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Add Index to speedup
            builder.Entity<CoinQuote>().HasIndex(x => new { x.Exchange, x.TimeStamp });
        }

        /// <summary>
        /// Gets or sets the ApplicationUsers
        /// </summary>
        public DbSet<rise.Models.ApplicationUser> ApplicationUsers { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationRoles
        /// </summary>
        public DbSet<rise.Models.ApplicationRole> ApplicationRoles { get; set; }

        /// <summary>
        /// Gets or sets the IPData
        /// </summary>
        public DbSet<rise.Models.IPData> IPData { get; set; }

        /// <summary>
        /// Gets or sets the Locations
        /// </summary>
        public DbSet<rise.Models.Location> Locations { get; set; }

        /// <summary>
        /// Gets or sets the Languages
        /// </summary>
        public DbSet<rise.Models.Language> Languages { get; set; }

        /// <summary>
        /// Gets or sets the CoinQuotes
        /// </summary>
        public DbSet<rise.Models.CoinQuote> CoinQuotes { get; set; }

        /// <summary>
        /// Gets or sets the Contributors
        /// </summary>
        public DbSet<rise.Models.DelegateForm> DelegateForms { get; set; }
    }
}