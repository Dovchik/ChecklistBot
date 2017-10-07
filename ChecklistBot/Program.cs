
using ChecklistBot.DataAccess;
using ChecklistBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ChecklistBot.Polling

{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().
                AddJsonFile("appsettings.json");
            var config = builder.Build();

            var contextBuilder = new DbContextOptionsBuilder<ChecklistBotContext>().
                UseSqlite(config.GetConnectionString("ChecklistBotContext"));
            var context = new DataAccess.ChecklistBotContext(contextBuilder.Options);

            var bot = new ChecklistBot.Services.ChecklistBotService(config.GetSection("BotConfiguration").Get<BotConfig>(), context);
            bot.StartBot();

        }
    }
}
