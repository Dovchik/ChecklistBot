using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ChecklistBot.Services;

namespace ChecklistBot.Webhook.Controllers
{
    public class UpdateController : Controller
    {
        private readonly ChecklistBotService _checkListBot;
        private readonly BotConfig _botConfig;
        public UpdateController(ChecklistBotService checklistBot, BotConfig botConfig)
        {
            _botConfig = botConfig;
            _checkListBot = checklistBot;
        }
        // POST api/values
        [HttpPost]
        public void Update([FromBody] Update update)
        {
            _checkListBot.HandleUpdate(update);
        }
        [HttpGet]
        [Route("api/update/status")]
        public String Status()
        {
            return _checkListBot.GetStatus();
        }
    }
}
