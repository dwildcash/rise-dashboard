using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Services
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}