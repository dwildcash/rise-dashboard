﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using rise.Data;
using rise.Models;
using rise_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace rise.Helpers
{
    public class AppUsersManager
    {
        /// <summary>
        /// Defines the scopeFactory
        /// </summary>
        private readonly IServiceScopeFactory scopeFactory;

        private readonly UserManager<ApplicationUser> _userManager;

        public AppUsersManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Get Last user by Msd
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ApplicationUser GetLastMsgUser(string excluded_username)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    return dbContext.ApplicationUsers.Where(x => x.UserName != excluded_username).OrderByDescending(x => x.LastMessage).FirstOrDefault();
                }
                catch (Exception ex)
                {
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
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddHours(-1) && x.UserName != excluded_username && x.UserName != null && x.MessageCount > 2).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Return a list of Random users
        /// </summary>,
        /// <returns></returns>
        public List<ApplicationUser> GetRainUsers(string excluded_username, int num = 10)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddDays(-2) && x.UserName != excluded_username && x.UserName != null && x.MessageCount > 3).OrderBy(x => Guid.NewGuid()).Take(num).ToList();
                }
                catch (Exception ex)
                {
                    return null;
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

            if (aspnetuser != null)
            {
                // Update Chat Id
                aspnetuser.ChatId = tgupdate.Message.Chat.Id;
            }

            return null;
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="TelegramId"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserAsync(string userName, long telegramId)
        {
            ApplicationUser appuser = null;

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                appuser = dbContext.Users.OfType<ApplicationUser>().Where(x => x.TelegramId == telegramId).FirstOrDefault();

                try
                {
                    // New user detected
                    if (appuser == null)
                    {
                        // Create new user
                        appuser = new ApplicationUser { UserName = userName, TelegramId = telegramId };

                        // Create a Wallet for user
                        await CreateWalletAsync(appuser);
                    }
                }
                catch
                {
                    return null;
                }
            }

            return appuser;
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="telegramId"></param>
        private async Task CreateWalletAsync(ApplicationUser appuser)
        {
            try
            {
                if (string.IsNullOrEmpty(appuser.Address))
                {
                    var account = RiseManager.CreateAccount();
                    appuser.Address = account.Result.account.address;
                    appuser.Secret = CryptoManager.EncryptStringAES(account.Result.account.secret, AppSettingsProvider.EncryptionKey);
                    appuser.PublicKey = account.Result.account.publicKey;

                    await _userManager.UpdateAsync(appuser);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}