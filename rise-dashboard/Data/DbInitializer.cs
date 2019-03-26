namespace rise.Data
{
    /// <summary>
    /// Defines the <see cref="DbInitializer" />
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// The InitializeApplicationDbContext
        /// </summary>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        public static void InitializeApplicationDbContext(ApplicationDbContext context)
        {
            // context.Database.Migrate();
        }
    }
}