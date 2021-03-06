using Telegram.Bot;

namespace rise.Services
{
    public class BotService : IBotService
    {
        public BotService()
        {
            // use proxy if configured in appsettings.*.json
            Client = new TelegramBotClient(rise.Models.AppSettingsProvider.BotApiKey);
        }

        public TelegramBotClient Client { get; }
    }
}