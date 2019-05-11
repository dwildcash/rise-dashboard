using System.Threading.Tasks;

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
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        /// <summary>
        /// Seed Roles
        /// </summary>
        /// <param name="roleManager"></param>
        public static async Task SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            // Create Administrator Group
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                var role = new ApplicationRole
                {
                    Name = "Administrator",
                    Description = "Application Administrator"
                };

                await roleManager.CreateAsync(role);
            }

            // Create Member Group
            if (!roleManager.RoleExistsAsync("Member").Result)
            {
                var role = new ApplicationRole
                {
                    Name = "Member",
                    Description = "Member User"
                };

                await roleManager.CreateAsync(role);
            }
        }

        /// <summary>
        /// Seed User
        /// </summary>
        /// <param name="userManager"></param>
        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            // Create a default user for me
            if (userManager.FindByNameAsync("dwildcash").Result == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "dwildcash"
                };

                var result = userManager.CreateAsync(user).Result;

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
        }
    }
}