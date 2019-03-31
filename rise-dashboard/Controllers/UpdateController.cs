using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Examples.DotNetCoreWebHook.Services;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Controllers
{
    [Route("api2/[controller]")]
    public class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        // POST api/update
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _updateService.EchoAsync(update);
            return StatusCode(200);
        }
    }
}
