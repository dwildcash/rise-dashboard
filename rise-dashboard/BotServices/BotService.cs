using rise.Models;
using Telegram.Bot;

namespace Rise.BotServices
{
    public class BotService : IBotService
    {
        public BotService()
        {
            // use proxy if configured in appsettings.*.json
            Client = new TelegramBotClient(AppSettingsProvider.BotApiKey);
        }

        public TelegramBotClient Client { get; }
    }
}