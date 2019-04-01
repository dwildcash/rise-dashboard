using rise_lib;
using rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using rise.Data;

namespace rise.Helpers
{
    public class RiseUsersManager
    {

        /// <summary>
        /// Defines the scopeFactory
        /// </summary>
        private readonly IServiceScopeFactory scopeFactory;

        private readonly UserManager<ApplicationUser> _userManager;


        public RiseUsersManager(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="telegramId"></param>
        public ApplicationUser GetUserByUserName(string Username)
        {

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var time = DateTime.Now.ToUniversalTime();
            }


                return null;
        }


        /// <summary>
        /// Get Last user by Msd
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ApplicationUser GetLastMsgUser(string username)
        {

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    return dbContext.ApplicationUsers.Where(x=>x.UserName != username).OrderByDescending(x=>x.LastMessage).FirstOrDefault();
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
        public List<ApplicationUser> GetBoomUsers(string username)
        {

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddHours(-1) && x.UserName != username && x.UserName != null && x.MessageCount > 2).ToList();
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
        public List<ApplicationUser> GetRainUsers(string username, int num = 10)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    return dbContext.Users.Where(x => x.LastMessage > DateTime.Now.AddDays(-2) && x.UserName != username && x.UserName != null && x.MessageCount > 3).OrderBy(x => Guid.NewGuid()).Take(num).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="telegramId"></param>
        public ApplicationUser GetUser(string UserName, int TelegramId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    if (dbContext.Users.Where(x => x.TelegramId == TelegramId).Count() == 0)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = UserName,
                            TelegramId = TelegramId,
                            MessageCount = 1,
                            Role = "Member",
                            LastMessage = DateTime.Now
                        };

                        dbContext.Add(user);
                        dbContext.SaveChanges();

                        return user;
                    }
                    else
                    {
                        var user = dbContext.Users.Where(x => x.TelegramId == TelegramId).FirstOrDefault();

                        if (user.UserName == null && UserName != null)
                        {
                            user.UserName = UserName;
                        }

                        user.MessageCount++;
                        user.LastMessage = DateTime.Now;
                        dbContext.SaveChanges();
                        return user;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="telegramId"></param>
        public void CreateWallet(int telegramId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var user = dbContext.Users.Where(x => x.TelegramId == telegramId).FirstOrDefault();

                    if (string.IsNullOrEmpty(user.Address))
                    {
                        var account = RiseManager.CreateAccount();
                        user.Address = account.Result.account.address;
                        user.EncryptedBip39 = CryptoManager.EncryptStringAES(account.Result.account.secret, AppSettingsProvider.EncryptionKey);
                        user.PublicKey = account.Result.account.publicKey;
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}

 