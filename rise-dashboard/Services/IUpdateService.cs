using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Rise.Services
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}