using rise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Services
{
    public interface IAppUsersManagerService
    {
        ApplicationUser GetLastMsgUser(string excludedUsername);

        List<ApplicationUser> GetBoomUsers(string excludedUsername, int maxusers = 10);

        List<ApplicationUser> GetRainUsers(string excludedUsername, int maxusers = 10);

        Task<ApplicationUser> GetUserAsync(Update tgupdate);

        Task<ApplicationUser> GetUserAsync(string userName, long telegramId = 0, bool flagMsgUpdate = false);

        void Update_Photourl(long telegramId, string photourl);
    }
}