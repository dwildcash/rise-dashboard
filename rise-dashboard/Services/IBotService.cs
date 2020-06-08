using Telegram.Bot;

namespace rise.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}