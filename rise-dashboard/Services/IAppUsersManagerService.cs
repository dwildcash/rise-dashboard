using rise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Services
{
    public interface IAppUsersManagerService
    {
        ApplicationUser GetLastMsgUser(string excluded_username);

        List<ApplicationUser> GetBoomUsers(string excluded_username);

        List<ApplicationUser> GetRainUsers(string excluded_username, int num = 10);

        Task<ApplicationUser> GetUserAsync(Update tgupdate);

        Task<ApplicationUser> GetUserAsync(string userName, long telegramId = 0, bool flagMsgUpdate = false);

        void Update_Photourl(long telegramId, string photourl);
    }
}