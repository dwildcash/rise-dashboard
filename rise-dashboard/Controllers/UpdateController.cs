﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.BotServices;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Controllers
{
 
    public class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        // POST api/update
        [HttpPost]
        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Post([FromBody]Update update)
        {
            var e = "emman";

            await _updateService.EchoAsync(update);
            return Ok();
        }
    }
}