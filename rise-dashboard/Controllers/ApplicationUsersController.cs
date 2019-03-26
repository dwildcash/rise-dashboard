namespace rise.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using rise.Data;
    using rise.Models;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ApplicationUsersController" />
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class ApplicationUsersController : Controller
    {
        /// <summary>
        /// Defines the _context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Defines the _userManager
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUsersController"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        /// <param name="userManager">The userManager<see cref="UserManager{ApplicationUser}"/></param>
        public ApplicationUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// The Index
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationUsers.ToListAsync());
        }

        /// <summary>
        /// The Edit
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            var roles = await _userManager.GetRolesAsync(applicationUser);

            var role = await _context.ApplicationRoles.SingleOrDefaultAsync(m => m.Name == roles.FirstOrDefault());

            string roleName;
            if (role == null)
            {
                roleName = "Guest";
            }
            else
            {
                roleName = role.Name;
            }

            // Create a list of available role
            ViewBag.Roles = new SelectList(_context.ApplicationRoles, "Name", "Name", roleName);

            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        /// <summary>
        /// The Edit
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="RoleId">The RoleId<see cref="string"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(string id, string RoleId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(applicationUser);

            await _userManager.RemoveFromRolesAsync(applicationUser, roles.ToArray());
            await _userManager.AddToRoleAsync(applicationUser, RoleId);

            var role = await _context.ApplicationRoles.SingleOrDefaultAsync(m => m.Name == roles.FirstOrDefault());

            // Create a list of available role
            ViewBag.Roles = new SelectList(_context.ApplicationRoles, "Name", "Name", role.Name);

            return View(applicationUser);
        }

        /// <summary>
        /// The Delete
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        /// <summary>
        /// The DeleteConfirmed
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUsers.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// The ApplicationUserExists
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        [Authorize(Roles = "Administrator")]
        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}