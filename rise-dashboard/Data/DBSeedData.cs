namespace rise.Data
{
    using Microsoft.AspNetCore.Identity;
    using Models;

    /// <summary>
    /// Defines the <see cref="DbSeedData" />
    /// </summary>
    public static class DbSeedData
    {
        /// <summary>
        /// The SeedData
        /// </summary>
        /// <param name="userManager">The userManager<see cref="UserManager{ApplicationUser}"/></param>
        /// <param name="roleManager">The roleManager<see cref="RoleManager{ApplicationRole}"/></param>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        /// <summary>
        /// Seed Roles
        /// </summary>
        /// <param name="roleManager"></param>
        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            // Create Administrator Group
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                ApplicationRole role = new ApplicationRole
                {
                    Name = "Administrator",
                    Description = "Application Administrator"
                };

                var roleResult = roleManager.CreateAsync(role).Result;
            }

            // Create Member Group
            if (!roleManager.RoleExistsAsync("Member").Result)
            {
                ApplicationRole role = new ApplicationRole
                {
                    Name = "Member",
                    Description = "Member User"
                };

                var roleResult = roleManager.CreateAsync(role).Result;
            }
        }

        /// <summary>
        /// Seed User
        /// </summary>
        /// <param name="userManager"></param>
        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            // Create a default user for me
            if (userManager.FindByNameAsync("dwildcash").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "dwildcash"
                };

                IdentityResult result = userManager.CreateAsync(user).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }
    }
}