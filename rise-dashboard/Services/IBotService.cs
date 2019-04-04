using Telegram.Bot;

namespace Rise.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}