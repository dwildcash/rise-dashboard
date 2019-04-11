using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using rise.Data;
using rise.Helpers;
using rise.Models;
using rise_lib;
using rise_lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Services
{
    public class AppUsersManagerService : IAppUsersManagerService
    {
        /// <summary>
        /// Defines the scopeFactory
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<AppUsersManagerService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppUsersManagerService(IServiceScopeFactory scopeFactory, UserManager<ApplicationUser> userManager, ILogger<AppUsersManagerService> logger)
        {
            _scopeFactory = scopeFactory;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get User by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserByUsername(string username)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    var user = dbContext.ApplicationUsers.Where(x => string.Equals(x.UserName, username, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    // Create a wallet for everyone
                    if (user.Address == null)
                    {
                        // Create a Wallet for user
                        AccountResult accountresult = await RiseManager.CreateAccount();

                        if (accountresult.success)
                        {
                            user.Address = accountresult.account.address;
                            user.Secret = CryptoManager.EncryptStringAES(accountresult.account.secret, AppSettingsProvider.EncryptionKey);
                            user.PublicKey = accountresult.account.publicKey;
                        }
                    }

                    dbContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetUserByUsername {0}", ex.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// Get Last user by Msd
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ApplicationUser GetLastMsgUser(string excluded_username)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    return dbContext.ApplicationUsers.Where(x => x.UserName != excluded_username && x.Address !=null).OrderByDescending(x => x.LastMessage).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetLastMsgUser {0}", ex.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Return a list of Random users
        /// </summary>
        /// <returns></returns>
        public List<ApplicationUser> GetBoomUsers(string excluded_username)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddHours(-1) && x.UserName != excluded_username && x.UserName != null && x.MessageCount > 2 && x.Address != null).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetBoomUsers {0}", ex.Message);
                }
                return null;
            }
        }


        /// <summary>
        /// Return a list of Random users
        /// </summary>,
        /// <returns></returns>
        public List<ApplicationUser> GetRainUsers(string excluded_username, int num = 10)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddDays(-2) && x.UserName != excluded_username && x.UserName != null && x.MessageCount > 3 && x.Address != null).OrderBy(x => Guid.NewGuid()).Take(num).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetRainUsers {0}", ex.Message);
                    return null;
                }
            }
        }


        /// <summary>
        /// Return a list of Random users
        /// </summary>,
        /// <returns></returns>
        public void uppdate_photourl(long telegramID, string photourl)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    dbContext.Users.Where(x => x.TelegramId == telegramID).FirstOrDefault().Photo_Url = photourl;
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetRainUsers {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="tgupdate"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserAsync(Update tgupdate)
        {
            ApplicationUser aspnetuser = await GetUserAsync(tgupdate.Message.Chat.Username, tgupdate.Message.Chat.Id);
            return null;
        }


        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="TelegramId"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserAsync(string userName, long telegramId, bool flagMsgUpdate=false)
        {
            ApplicationUser appuser = null;

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    appuser = dbContext.Users.OfType<ApplicationUser>().Where(x => x.TelegramId == telegramId).FirstOrDefault();

                    try
                    {
                        // New user detected
                        if (appuser == null)
                        {
                            // Create new user
                            appuser = new ApplicationUser { UserName = userName, TelegramId = telegramId, Role = "Member" };

                            dbContext.Users.Add(appuser);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Received Exception from CreateUser {0}", ex.Message);
                        return null;
                    }

                    // Flag update message
                    if (flagMsgUpdate)
                    {
                        appuser.MessageCount++;
                        appuser.LastMessage = DateTime.Now;
                    }

                    // Create a wallet for everyone
                    if (appuser.Address == null)
                    {
                        // Create a Wallet for user
                        AccountResult accountresult = await RiseManager.CreateAccount();

                        if (accountresult.success)
                        {
                            appuser.Address = accountresult.account.address;
                            appuser.Secret = CryptoManager.EncryptStringAES(accountresult.account.secret, AppSettingsProvider.EncryptionKey);
                            appuser.PublicKey = accountresult.account.publicKey;
                        }
                    }

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from GetUserAsync {0}", ex.Message);
            }

            return appuser;
        }
    }
}