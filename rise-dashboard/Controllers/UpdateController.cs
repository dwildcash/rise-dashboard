using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.BotServices;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Controllers
{
    [AllowAnonymous]
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
            return Ok();
        }
    }
}