namespace rise.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ApplicationRolesController" />
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class ApplicationRolesController : Controller
    {
        /// <summary>
        /// Defines the _context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRolesController"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        public ApplicationRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// The Index
        /// </summary>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationRoles.ToListAsync());
        }

        /// <summary>
        /// The Details
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await _context.ApplicationRoles.SingleOrDefaultAsync(m => m.Id == id);

            if (applicationRole == null)
            {
                return NotFound();
            }

            return View(applicationRole);
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns>The <see cref="IActionResult"/></returns>
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="applicationRole">The applicationRole<see cref="ApplicationRole"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Description,Id,Name,NormalizedName,ConcurrencyStamp")] ApplicationRole applicationRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(applicationRole);
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

            var applicationRole = await _context.ApplicationRoles.SingleOrDefaultAsync(m => m.Id == id);

            if (applicationRole == null)
            {
                return NotFound();
            }

            return View(applicationRole);
        }

        /// <summary>
        /// The Edit
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="applicationRole">The applicationRole<see cref="ApplicationRole"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(string id, [Bind("Description,Id,Name,NormalizedName,ConcurrencyStamp")] ApplicationRole applicationRole)
        {
            if (id != applicationRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationRoleExists(applicationRole.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(applicationRole);
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

            var applicationRole = await _context.ApplicationRoles.SingleOrDefaultAsync(m => m.Id == id);

            if (applicationRole == null)
            {
                return NotFound();
            }

            return View(applicationRole);
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
            var applicationRole = await _context.ApplicationRoles.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationRoles.Remove(applicationRole);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// The ApplicationRoleExists
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool ApplicationRoleExists(string id)
        {
            return _context.ApplicationRoles.Any(e => e.Id == id);
        }
    }
}