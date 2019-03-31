using Telegram.Bot;

namespace Rise.BotServices
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}