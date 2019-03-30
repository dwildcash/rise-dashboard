namespace rise_tgbot
{
    using Microsoft.Extensions.Configuration;
    using rise_lib;
    using rise_tgbot.Helpers;
    using rise_tgbot.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Defines the <see cref="Program" />
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Defines the Bot
        /// </summary>
        private static TelegramBotClient Bot;

        private static int msgCount = 0;

        /// <summary>
        /// The Main
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/></param>
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            BuildAppSettingsProvider();

            Bot = new TelegramBotClient(AppSettingsProvider.BotAPIKey);
            var me = Bot.GetMeAsync().Result;

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            LogManager.WriteLog("Application Started BotName:" + me.Username);

            System.Threading.Thread.Sleep(-1);
            Bot.StopReceiving();
        }

        /// <summary>
        /// The BotOnReceiveError
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="receiveErrorEventArgs">The receiveErrorEventArgs<see cref="ReceiveErrorEventArgs"/></param>
        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}", receiveErrorEventArgs.ApiRequestException.ErrorCode, receiveErrorEventArgs.ApiRequestException.Message);
            LogManager.WriteLog(receiveErrorEventArgs.ApiRequestException.Message);
        }

        /// <summary>
        /// The BotOnMessageReceived
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="messageEventArgs">The messageEventArgs<see cref="MessageEventArgs"/></param>
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.Text)
            {
                return;
            }

            // Increase msg count
            msgCount++;

            var user = UserManager.GetUser(message.From.Username, message.From.Id);

            string command = string.Empty;
            string destUser = string.Empty;

            // Match Command
            if (Regex.Matches(message.Text, @"!(\S+)\s?").Count > 0)
            {
                command = Regex.Matches(message.Text.ToUpper(), @"!(\S+)\s?")[0].ToString().Trim();
            }

            // Match DestUser if any
            if (Regex.Matches(message.Text, @"@(\S+)\s?").Count > 0)
            {
                destUser = Regex.Matches(message.Text, @"@(\S+)\s?")[0].ToString().Replace("@", "").Trim();
            }

            if (command == "!SEND" || command == "!RAIN" || command == "!BOOM" || command == "!SPLASH")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                double amount = 0;
                List<User> DestUserLst = new List<User>();

                int maxUsers = 1;
                var recepientId = string.Empty;
                string strResponse = string.Empty;

                // Verify if we have a dest user
                if (command == "!SEND" && string.IsNullOrEmpty(destUser))
                {
                    strResponse = "Please provide a user name starting with <b>@</b>!send 10 RISE to @Dwildcash";
                    LogManager.WriteLog(strResponse);
                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }

                // Retreive the amount to send
                try
                {
                    amount = double.Parse(Regex.Matches(message.Text, @"[-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?").Cast<Match>().Select(m => m.Value).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    strResponse = "Illegal amount. ex: !send 10 Rise to @Dwildcash";
                    LogManager.WriteLog(strResponse + " exception" + ex.Message);
                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }

                if (command == "!SPLASH")
                {
                    var balance = await RiseManager.AccountBalanceAsync(user.Address);

                    int lockMsg = msgCount;
                    if (balance < amount + 0.1)
                    {
                        strResponse = "Not enough RISE to splash!! " + amount + " RISE";
                        LogManager.WriteLog(strResponse);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                        return;
                    }
                    else
                    {
                        strResponse = "@" + user.UserName + " activated a <b>Splash!</b> Be active! I will choose a winner in the next messages!";
                        LogManager.WriteLog(strResponse);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);

                        var waitMsg = lockMsg + (int)RandomGenerator.NextLong(1, 10);

                        while (msgCount < waitMsg)
                        {
                            Thread.Sleep(1000);
                        }

                        DestUserLst.Add(UserManager.GetLastMsgUser(user.UserName));
                    }
                }

                if (command == "!SEND")
                {
                    var myuser = UserManager.GetUserByUserName(destUser);

                    if (myuser == null)
                    {
                        strResponse = "I dont know @" + destUser + " ask him to create a deposit address. with !deposit";
                        LogManager.WriteLog(strResponse);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                        return;
                    }
                    else
                    {
                        DestUserLst.Add(myuser);
                    }
                }
                else
                {
                    if (command == "!RAIN")
                    {
                        maxUsers = 5;
                        DestUserLst = UserManager.GetRainUsers(user.UserName, maxUsers);
                    }
                    else if (command == "!BOOM")
                    {
                        DestUserLst = UserManager.GetBoomUsers(user.UserName);
                        maxUsers = DestUserLst.Count();
                    }

                    amount = Math.Round((amount - ((double)maxUsers * 0.1)) / (double)maxUsers, 2);

                    if (amount <= 0.1)
                    {
                        strResponse = "That make no sense to send " + amount + " to " + maxUsers + " users";
                        LogManager.WriteLog(strResponse);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                        return;
                    }
                }

                // Verify user balance
                try
                {
                    if (user?.Address != null)
                    {
                        var balance = await RiseManager.AccountBalanceAsync(user.Address);

                        if (balance <= 0.1 || amount + 0.1 > balance || ((command == "!RAIN" || command == "!BOOM") && balance < (((double)maxUsers * 0.1) + ((double)amount * (double)maxUsers))))
                        {
                            if (command == "!RAIN" || command == "!BOOM")
                            {
                                strResponse = "Not enough RISE to " + command + " <b>" + amount + " RISE</b> to " + maxUsers + " Users. Balance:<b> " + balance + "</b> RISE";
                            }
                            else
                            {
                                strResponse = "Not enough RISE to " + command + " <b>" + amount + " RISE</b> balance:<b>" + balance + "</b> RISE";
                            }

                            LogManager.WriteLog(strResponse);
                            await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                            return;
                        }
                    }
                    else
                    {
                        strResponse = "You dont have a wallet Address! Please create one with !deposit";
                        LogManager.WriteLog(strResponse);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    strResponse = "Error fetching user @" + user.UserName + " balance";
                    LogManager.WriteLog(strResponse + " Exception:" + ex.Message);
                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }

                foreach (var selUser in DestUserLst)
                {
                    // Destination user section
                    try
                    {
                        if (selUser?.Address != null)
                        {
                            recepientId = selUser.Address;
                        }
                        else
                        {
                            // destination user doesnt have a wallet let create one
                            UserManager.CreateWallet(selUser.TelegramId);
                            selUser.Address = UserManager.GetUser(selUser.UserName, selUser.TelegramId).Address;
                        }

                        try
                        {
                            var t = await RiseManager.CreatePaiment(amount * 100000000, user.GetSecret(), selUser.Address);

                            if (command == "!SEND" || command == "!SPLASH")
                            {
                                if (command == "!SEND")
                                {
                                    strResponse = "@" + selUser.UserName + " wake up, its a wonderful day!! thanks to @" + user.UserName + " he sent <b>" + amount + " RISE</b> to you :)";
                                }
                                else
                                {
                                    strResponse = "@" + selUser.UserName + " wake up, you have been <b>SPLASHED!!</b> thanks to @" + user.UserName + " he sent <b>" + amount + " RISE</b> to you :)";
                                }

                                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                                var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + t.transactionId));
                                await Bot.SendTextMessageAsync(message.Chat, "Transaction Id:" + t.transactionId + "", replyMarkup: keyboard);
                            }
                        }
                        catch (Exception ex)
                        {
                            strResponse = "Problem processing Transaction. Make sure you are in doing it right ex: !send 2 rise @Dwildcash";
                            LogManager.WriteLog(strResponse + " Exception:" + ex.Message);
                            await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        strResponse = "Problem processing Transaction. Here an exemple: !send 2 rise to @Dwildcash";
                        LogManager.WriteLog(strResponse + " Exception:" + ex.Message);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                        return;
                    }
                }

                if (command == "!RAIN" || command == "!BOOM")
                {
                    string winners = string.Empty;

                    foreach (var ee in DestUserLst)
                    {
                        winners += "@" + ee.UserName + " ";
                    }

                    if (winners.Count() == 0)
                    {
                        strResponse = "Mehhh... look like you are alone here :(";
                    }
                    else if (command == "!RAIN")
                    {
                        strResponse = "It's raining! hallelujah!! @" + user.UserName + " dropped <b>" + amount + " RISE</b> to each of these active users " + winners;
                    }
                    else if (command == "!BOOM")
                    {
                        strResponse = "Boom!!! @" + user.UserName + " dropped <b>" + amount + " RISE</b> to each of these active users in the last hour! " + winners;
                    }

                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                }
            }

            // Get Current price for rise.coinquote.io
            if (command == "!WITHDRAW")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                double amount = 0;
                string strResponse;

                try
                {
                    amount = double.Parse(Regex.Matches(message.Text, @"[-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?").Cast<Match>().Select(m => m.Value).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    strResponse = "Illegal amount entered. ex: !withdraw 10 RISE to 5953135380169360325R";
                    LogManager.WriteLog(strResponse + " exception" + ex.Message);
                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }

                try
                {
                    var recipientId = message.Text.ToUpper().Split(' ').Last();

                    if (recipientId[recipientId.Length - 1] != 'R')
                    {
                        strResponse = "<b>" + recipientId + "</b> doesnt look to be a valid RISE address (doesnt end with an 'R')";
                        LogManager.WriteLog(strResponse);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                        return;
                    }

                    if (user?.Address != null)
                    {
                        var balance = await RiseManager.AccountBalanceAsync(user.Address);


                        if (balance <= 0.1 || amount <= 0.1)
                        {
                            strResponse = "You dont have enough to cover network fees <b>0.1</b> <b>" + amount + " RISE</b> to " + recipientId + " balance:<b>" + balance + "</b> RISE";
                            LogManager.WriteLog(strResponse);
                            await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                            return;
                        }

                        if (balance < amount + 0.1)
                        {
                            strResponse = "You dont have enough RISE to withdraw " + amount + " balance: " + balance + "</b> RISE";
                            LogManager.WriteLog(strResponse);
                            await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                            return;
                        }

                    }
                    else
                    {
                        strResponse = "You dont have a <b>RISE</b> wallet Address! Please create one with !deposit";
                        LogManager.WriteLog(strResponse);
                        await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                        return;
                    }

                    var t = await RiseManager.CreatePaiment(amount * 100000000, user.GetSecret(), recipientId);

                    strResponse = "Sent <b>" + amount + "</b> to " + recipientId + " Status:" + t.success;
                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);

                    var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + t.transactionId));

                    await Bot.SendTextMessageAsync(message.Chat.Id, "Transaction Id:" + t.transactionId + "", replyMarkup: keyboard);
                }
                catch (Exception ex)
                {
                    strResponse = "Problem processing Transaction - make sure you are in doing it right ex: !withdraw 10 rise 5953135380169360325R";
                    LogManager.WriteLog(strResponse + " Exception:" + ex.Message);
                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }
            }

            // Get Current price for rise.coinquote.io
            if (command == "!PRICE")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                var quote = await QuoteManager.GetRiseQuote();

                string strResponse = "Price (sat): <b>" + Math.Round(quote.price * 100000000) + "</b>" + Environment.NewLine +
                    "Usd Price: <b>$" + Math.Round(quote.usdPrice, 4) + "</b>" + Environment.NewLine +
                    "Volume: <b>" + Math.Round(quote.volume).ToString("N0") + "</b>";
                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Open Website", "https://rise.coinquote.io"));
                await Bot.SendTextMessageAsync(message.Chat.Id, "Quote from rise.coinquote.io", replyMarkup: keyboard);
            }

            // Check when a user last logged
            if (command == "!SEEN")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                string strResponse;

                var myuser = UserManager.GetUserByUserName(destUser);

                if (string.IsNullOrEmpty(destUser))
                {
                    strResponse = "Please provide a user name starting with <b>@</b> ex !seen @Dwildcash";
                    LogManager.WriteLog(strResponse);
                    await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }

                if (myuser?.LastMessage != null)
                {
                    var lastseen = user.LastMessage;
                    double hours = Math.Round((DateTime.Now - myuser.LastMessage).TotalHours, 2);
                    double minutes = Math.Round((DateTime.Now - myuser.LastMessage).TotalMinutes, 2);
                    string showtime;

                    if (hours <= 1 && minutes > 0)
                    {
                        showtime = minutes + " minutes ago.";
                    }
                    else if (myuser.UserName == user.UserName)
                    {
                        showtime = "... @" + user.UserName + " problem finding yourself? ";
                    }
                    else if (minutes == 0 && myuser.UserName != user.UserName)
                    {
                        showtime = " ... helloooo!! is it me you looking for?";
                    }
                    else
                    {
                        showtime = Math.Round(hours, 2) + " hours ago.";
                    }

                    strResponse = "Last Message from @" + myuser.UserName + " was " + showtime;
                }
                else
                {
                    strResponse = "Sorry, I don't know @" + destUser;
                }

                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }

            // Get the balance
            if (command == "!BALANCE")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                string strResponse;

                if (!string.IsNullOrEmpty(user.Address))
                {
                    strResponse = "<b>Current Balance for </b>@" + user.UserName + Environment.NewLine +
                        "Address: <b>" + user.Address + "</b>" + Environment.NewLine +
                        "Balance <b>" + Math.Round(await RiseManager.AccountBalanceAsync(user.Address), 4) + " RISE </b>";

                    if (string.IsNullOrEmpty(user.UserName))
                    {
                        strResponse += Environment.NewLine + " Note: Please configure your Telegram UserName if you want to Receive <b>Rise</b>";
                    }
                }
                else
                {
                    strResponse = "No wallet address found...Let me create one for you!";
                    command = "!DEPOSIT";
                }

                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }

            // Create a Deposit address
            if (command == "!DEPOSIT")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                var uuser = UserManager.GetUser(user.UserName, user.TelegramId);

                UserManager.CreateWallet(user.TelegramId);

                string strResponse = "Here we go @" + uuser.UserName + " <b>" + uuser.Address + "</b>";

                if (string.IsNullOrEmpty(uuser.UserName))
                {
                    strResponse += Environment.NewLine + " Note: Please configure your Telegram UserName if you want to Receive <b>Rise</b>";
                }

                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }

            // Return a  geek joke
            if (command == "!JOKE")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                var qotd = await QuoteOfTheDayManager.GetQuoteOfTheDay();

                if (qotd != null)
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, qotd, ParseMode.Html);
                }
            }

            // Return Help context
            if (command == "!HOPE")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                var strResponse = "There is always hope!";
                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }

            if (command == "!INFO")
            {
                var strResponse = "<b>RiseVison Information/Tools</b>" + Environment.NewLine +
                "<b>Rise Website</b> - https://rise.vision" + Environment.NewLine +
                "<b>Rise Explorer</b> - https://explorer.rise.vision/" + Environment.NewLine +
                "<b>Rise GitHub</b> - https://github.com/RiseVision" + Environment.NewLine +    
                "<b>Rise Medium</b> - https://medium.com/rise-vision" + Environment.NewLine +
                "<b>Rise Web Wallet</b> - https://wallet.rise.vision" + Environment.NewLine +
                "<b>Rise Twitter</b> - https://twitter.com/RiseVisionTeam" + Environment.NewLine +
                "<b>Rise Force Game</b> - http://duhec.net/riseForce" + Environment.NewLine +
                "<b>Rise Dashboard</b> - https://rise.coinquote.io" + Environment.NewLine +
                "<b>Rise Telegram</b> - https://t.me/risevisionofficial" + Environment.NewLine +
                "<b>Rise Slack</b> - https://risevision.slack.com/" + Environment.NewLine +
                "<b>Rise Discord</b> - https://discord.gg/6jyWQnJ" + Environment.NewLine +
                "<b>Rise BitcoinTalk</b> - https://bitcointalk.org/index.php?topic=3211240.200" + Environment.NewLine +
                "<b>Rise Intro Youtube</b> - https://www.youtube.com/watch?v=wZ2vIGl_gCM&feature=youtu.be" + Environment.NewLine +
                "<b>Rise Telegram Tipping service</b> -!help";
                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }


            // Return Help context
            if (command == "!EXCHANGES")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                var strResponse = "<b>-= Current Rise Exchanges =-</b>" + Environment.NewLine +
                        "<b>Livecoin</b> - http://livecoin.net" + Environment.NewLine +
                        "<b>RightBtc</b> - http://rightbtc.com" + Environment.NewLine +
                        "<b>Vinex</b> - https://vinex.network";
                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }

            // Return Help context
            if (command == "!HELP")
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                var strResponse = "<b>-= Help =-</b>" + Environment.NewLine +
                        "<b>!rain</b> - !rain 10 (to random users active in last 2 days)" + Environment.NewLine +
                        "<b>!boom</b> - !boom 10 (to all users active the in last hour)" + Environment.NewLine +
                        "<b>!splash</b> - !splash 10 (winner will be in random in next max 10 msg)" + Environment.NewLine +
                        "<b>!send</b> - !send 5 RISE to @Dwildcash" + Environment.NewLine +
                        "<b>!withdraw</b> - !withdraw 5 RISE to 5953135380169360325R" + Environment.NewLine +
                        "<b>!seen</b> - Show last message from user !seen @Dwildcash" + Environment.NewLine +
                        "<b>!deposit</b> - Create a Deposit RISE Address" + Environment.NewLine +
                        "<b>!balance</b> - Show current RISE Balance" + Environment.NewLine +
                        "<b>!joke</b> - Display a geek joke" + Environment.NewLine +
                        "<b>!exchanges</b> - Display current RISE Exchanges" + Environment.NewLine +
                        "<b>!price</b> - Show current RISE Price" + Environment.NewLine;
                await Bot.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }
        }

        /// <summary>
        /// Build Application AppSettings
        /// </summary>
        private static void BuildAppSettingsProvider()
        {
            AppSettingsProvider.BotAPIKey = Configuration["AppSettings:botAPIKey"];
            AppSettingsProvider.EncryptionKey = Configuration["AppSettings:EncryptionKey"];
            AppSettingsProvider.Salt = Configuration["AppSettings:Salt"];
        }

        /// <summary>
        /// Gets or sets the Configuration
        /// </summary>
        public static IConfiguration Configuration { get; set; }
    }
}