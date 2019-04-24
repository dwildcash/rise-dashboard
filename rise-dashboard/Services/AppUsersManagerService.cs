using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using rise.Data;
using rise.Helpers;
using rise.Models;
using rise_lib;
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
        /// Get the last Message
        /// </summary>
        /// <param name="excludedUsername"></param>
        /// <returns></returns>
        public List<ApplicationUser> GetLastMsgUsers(string excludedUsername, int maxusers = 1)
        {
            if (excludedUsername == null) throw new ArgumentNullException(nameof(excludedUsername));

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    return dbContext.ApplicationUsers.Where(x => x.UserName != excludedUsername && x.Address != null).OrderByDescending(x => x.LastMessage).Take(maxusers).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetLastMsgUser {0}", ex.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Return List of Boom Users
        /// </summary>
        /// <param name="excludedUsername"></param>
        /// <returns></returns>
        public List<ApplicationUser> GetBoomUsers(string excludedUsername, int maxusers = 7)
        {
            if (excludedUsername == null) throw new ArgumentNullException(nameof(excludedUsername));

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddDays(-1) && x.UserName != excludedUsername && x.UserName != null && x.MessageCount > 2 && x.Address != null).Take(maxusers).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetBoomUsers {0}", ex.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// Get List of Rain Users
        /// </summary>
        /// <param name="excludedUsername"></param>
        /// <param name="maxusers"></param>
        /// <returns></returns>
        public List<ApplicationUser> GetRainUsers(string excludedUsername, int maxusers = 7)
        {
            if (excludedUsername == null) throw new ArgumentNullException(nameof(excludedUsername));

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddDays(-7) && x.UserName != excludedUsername && x.UserName != null && x.MessageCount > 3 && x.Address != null).OrderBy(x => Guid.NewGuid()).Take(maxusers).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from GetRainUsers {0}", ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Update Photo Url
        /// </summary>
        /// <param name="telegramId"></param>
        /// <param name="photourl"></param>
        public void Update_Photourl(long telegramId, string photourl)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    dbContext.Users.FirstOrDefault(x => x.TelegramId == telegramId).Photo_Url = photourl;
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
        public async Task<ApplicationUser> GetUserAsync(Update tgupdate) => await GetUserAsync(tgupdate.Message.Chat.Username, tgupdate.Message.Chat.Id);

        /// <summary>
        /// Get user Async
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="telegramId"></param>
        /// <param name="flagMsgUpdate"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserAsync(string userName, long telegramId = 0, bool flagMsgUpdate = false)
        {
            ApplicationUser appuser = null;

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    if (telegramId != 0)
                    {
                        appuser = dbContext.Users.OfType<ApplicationUser>().FirstOrDefault(x => x.TelegramId == telegramId);

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

                        // Update Username
                        if (userName != appuser?.UserName)
                        {
                            appuser.UserName = userName;
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
                            var accountresult = await RiseManager.CreateAccount();

                            if (accountresult.success)
                            {
                                appuser.Address = accountresult.account.address;
                                appuser.Secret = CryptoManager.EncryptStringAES(accountresult.account.secret, AppSettingsProvider.EncryptionKey);
                                appuser.PublicKey = accountresult.account.publicKey;
                            }
                        }

                        dbContext.SaveChanges();
                    }
                    else
                    {
                        appuser = dbContext.Users.OfType<ApplicationUser>().FirstOrDefault(x => x.UserName == userName);
                    }
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