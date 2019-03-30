using rise_lib;
using rise_tgbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rise_tgbot.Helpers
{
    public static class UserManager
    {
        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="telegramId"></param>
        public static User GetUserByUserName(string Username)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    return context.Users.Where(x => string.Equals(x.UserName, Username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    string mesg = "Could not find " + Username + " Exception: " + ex.Message;
                    LogManager.WriteLog(mesg);
                }
            }

            return null;
        }


        /// <summary>
        /// Get Last user by Msd
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static User GetLastMsgUser(string username)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    return context.Users.Where(x=>x.UserName != username).OrderByDescending(x=>x.LastMessage).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog("Error Getting Last " + ex.Message);
                    return null;
                }
            }
        }


        /// <summary>
        /// Return a list of Random users
        /// </summary>
        /// <returns></returns>
        public static List<User> GetBoomUsers(string username)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    return context.Users.Where(x => x.LastMessage > DateTime.Now.AddHours(-1) && x.UserName != username && x.UserName != null && x.MessageCount > 2).ToList();
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog("Error Getting Boom Users " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Return a list of Random users
        /// </summary>,
        /// <returns></returns>
        public static List<User> GetRainUsers(string username, int num = 10)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    return context.Users.Where(x => x.LastMessage > DateTime.Now.AddDays(-2) && x.UserName != username && x.UserName != null && x.MessageCount > 3).OrderBy(x => Guid.NewGuid()).Take(num).ToList();
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog("Error Getting Rain Users " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="telegramId"></param>
        public static User GetUser(string UserName, int TelegramId)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    if (context.Users.Where(x => x.TelegramId == TelegramId).Count() == 0)
                    {
                        var user = new User
                        {
                            UserName = UserName,
                            TelegramId = TelegramId,
                            MessageCount = 1,
                            Role = "Member",
                            LastMessage = DateTime.Now
                        };

                        context.Add(user);
                        context.SaveChanges();

                        return user;
                    }
                    else
                    {
                        var user = context.Users.Where(x => x.TelegramId == TelegramId).FirstOrDefault();

                        if (user.UserName == null && UserName != null)
                        {
                            user.UserName = UserName;
                        }

                        user.MessageCount++;
                        user.LastMessage = DateTime.Now;
                        context.SaveChanges();
                        return user;
                    }
                }
                catch (Exception ex)
                {
                    string mesg = "Problem creating user " + UserName + " TelegramId:" + TelegramId + " Exception: " + ex.Message;
                    LogManager.WriteLog(mesg);
                }
            }

            return null;
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="telegramId"></param>
        public static void CreateWallet(int telegramId)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    var user = context.Users.Where(x => x.TelegramId == telegramId).FirstOrDefault();

                    if (string.IsNullOrEmpty(user.Address))
                    {
                        var account = RiseManager.CreateAccount();
                        user.Address = account.Result.account.address;
                        user.Secret = CryptoManager.EncryptStringAES(account.Result.account.secret, AppSettingsProvider.EncryptionKey);
                        user.PublicKey = account.Result.account.publicKey;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    string mesg = "Problem creating wallet for " + telegramId + " Exception: " + ex.Message;
                    LogManager.WriteLog(mesg);
                }
            }
        }
    }
}