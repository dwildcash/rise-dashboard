using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using rise.Code.Rise;
using rise.Data;
using rise.Helpers;
using rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Services
{
    public class AppUsersManagerService : IAppUsersManagerService
    {

        private readonly ApplicationDbContext _appdb;

        public AppUsersManagerService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _appdb = context;
        }

        /// <summary>
        /// Get the last Message
        /// </summary>
        /// <param name="excludedUsername"></param>
        /// <returns></returns>
        public List<ApplicationUser> GetLastMsgUsers(string excludedUsername, int maxusers = 1)
        {
            if (excludedUsername == null) throw new ArgumentNullException(nameof(excludedUsername));

            try
            {
                return _appdb.ApplicationUsers.Where(x => x.UserName != excludedUsername && x.Address != null).OrderByDescending(x => x.LastMessage).Take(maxusers).ToList();
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
            }

            return null;
        }

        /// <summary>
        /// Return List of Boom Users
        /// </summary>
        /// <param name="excludedUsername"></param>
        /// <returns></returns>
        public List<ApplicationUser> GetBoomUsers(string excludedUsername, int maxusers = 7)
        {
            if (excludedUsername == null) throw new ArgumentNullException(nameof(excludedUsername));

            try
            {
                return _appdb.Users.Where(x => x.LastMessage > DateTime.Now.AddMinutes(-240) && x.UserName != excludedUsername && x.UserName != null && x.MessageCount >= 5 && x.Address != null && x.Address != string.Empty).AsEnumerable().OrderBy(x => Guid.NewGuid()).Take(maxusers).ToList();
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
            }

            return null;
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

            try
            {
                return _appdb.Users.Where(x => x.LastMessage > DateTime.Now.AddDays(-1) && x.UserName != excludedUsername && x.UserName != null && x.MessageCount >= 5 && x.Address != null && x.Address != string.Empty).AsEnumerable().OrderBy(x => Guid.NewGuid()).Take(maxusers).ToList();
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
            }

            return null;
        }

        /// <summary>
        /// Update Photo Url
        /// </summary>
        /// <param name="telegramId"></param>
        /// <param name="photourl"></param>
        public void Update_Photourl(long telegramId, string photourl)
        {
            try
            {
                _appdb.Users.FirstOrDefault(x => x.TelegramId == telegramId).Photo_Url = photourl;
                _appdb.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
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
                if (telegramId != 0)
                {
                    appuser = _appdb.Users.OfType<ApplicationUser>().FirstOrDefault(x => x.TelegramId == telegramId);

                    try
                    {
                        // New user detected
                        if (appuser == null)
                        {
                            // Create new user
                            appuser = new ApplicationUser { UserName = userName, TelegramId = telegramId, Role = "Member" };
                            _appdb.Users.Add(appuser);
                        }
                    }
                    catch (Exception ex)
                    {
                        var log = new Log();
                        log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                        _appdb.Logger.Add(log);
                        _appdb.SaveChangesAsync().Wait();
                        return null;
                    }

                    // Update Username
                    if (userName != appuser?.UserName)
                    {
                        appuser.UserName = userName;
                    }

                    // Create a wallet for everyone
                    if (appuser.Address == null)
                    {
                        RiseManager rm = new RiseManager();

                        // Create a Wallet for user
                        var accountresult = await rm.CreateAccount();

                        if (accountresult.success)
                        {
                            appuser.Address = accountresult.account.Address;
                            appuser.Secret = CryptoManager.EncryptStringAES(accountresult.account.secret, AppSettingsProvider.EncryptionKey);
                            appuser.PublicKey = accountresult.account.PublicKey;
                        }
                    }

                    // Flag update message
                    if (flagMsgUpdate)
                    {
                        appuser.MessageCount++;
                        appuser.LastMessage = DateTime.Now;
                    }

                    _appdb.SaveChanges();
                }
                else
                {
                    appuser = _appdb.Users.OfType<ApplicationUser>().FirstOrDefault(x => x.UserName == userName);
                }
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
            }

            return appuser;
        }
    }
}