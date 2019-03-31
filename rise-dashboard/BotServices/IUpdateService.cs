using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Rise.BotServices
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}
