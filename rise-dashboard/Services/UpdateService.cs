using Microsoft.Extensions.Logging;
using rise.Helpers;
using rise.Models;
using rise.Services;
using rise_lib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Rise.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;
        private readonly IAppUsersManagerService _appUsersManagerService;
        private static long _messagesCount;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, IAppUsersManagerService appUsersManagerService)
        {
            _botService = botService;
            _appUsersManagerService = appUsersManagerService;
            _logger = logger;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            var flagMsgUpdate = false;

            // Count only message coming from channel
            if (message.Chat.Id == AppSettingsProvider.TelegramChannelId)
            {
                flagMsgUpdate = true;
                _messagesCount++;
            }

            // Get the user who sent message
            var appuser = await _appUsersManagerService.GetUserAsync(message.From.Username, message.From.Id, flagMsgUpdate);

            var maxusers = 5;
            var botcommands = new List<string>();
            var lstDestUsers = new List<string>();
            var lstDestAddress = new List<string>();
            var lstAmount = new List<double>();

            try
            {
                // Match !Command if present
                if (Regex.Matches(message.Text, @"!(\S+)\s?").Count > 0)
                {
                    botcommands = Regex.Matches(message.Text.ToUpper(), @"!(\S+)\s?").Select(m => m.Value).ToList();
                }

                // Match @username if present
                if (Regex.Matches(message.Text, @"@(\S+)\s?").Count > 0)
                {
                    lstDestUsers = Regex.Matches(message.Text, @"@(\S+)\s?").Select(m => m.Value.Replace("@", "").Trim()).ToList();
                }

                // Match any double Amount if present
                if (Regex.Matches(message.Text, @"\b[0-9]+(\.[0-9]*)?([ ]|($|\s))").Count > 0)
                {
                    lstAmount = Regex.Matches(message.Text, @"\b[0-9]+(\.[0-9]*)?([ ]|($|\s))", RegexOptions.Multiline).Select(m => double.Parse(m.Value.Trim())).ToList();
                }

                // Match any Rise Address if present
                if (Regex.Matches(message.Text, @"\d+[R,r]").Count > 0)
                {
                    lstDestAddress = Regex.Matches(message.Text, @"\d+[R,r]").Select(m => m.Value.Trim()).ToList();
                }

                // add any extra users
                if (lstAmount.Count > 1 && lstAmount[1] < 5000 && Math.Abs(lstAmount[1]) > 0)
                {
                    maxusers = int.Parse(lstAmount[1].ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parsing parameters {0}" + ex.Message);
                return;
            }

            foreach (var command in botcommands)
            {
                try
                {
                    switch (command.Trim())
                    {
                        // Info command
                        case "!INFO":
                            await cmd_Info(message);
                            break;
                        // Show Balance
                        case "!BALANCE":
                            await cmd_ShowUserBalance(appuser);
                            break;
                        // Show Help
                        case "!HELP":
                            await cmd_Help(appuser);
                            break;

                        // Withdraw RISE to address
                        case "!WITHDRAW":
                            {
                                if (await cmd_preSend(lstAmount.FirstOrDefault() - (0.1 * lstDestAddress.Count), command, lstDestAddress.Count, message.Chat.Id, appuser))
                                {
                                    await cmd_Withdraw(appuser, lstAmount.FirstOrDefault() - (0.1 * lstDestAddress.Count), lstDestAddress.FirstOrDefault());
                                }

                                break;
                            }
                        // Splash!
                        case "!SPLASH":
                            {
                                // add any extra users
                                if (lstAmount.Count == 1)
                                {
                                    maxusers = 1;
                                }

                                if (await cmd_preSend(lstAmount.FirstOrDefault(), command, maxusers, message.Chat.Id, appuser))
                                {
                                    var waitMsg = _messagesCount + (int)RandomGenerator.NextLong(1, 4);

                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, " Be active! @" + appuser.UserName + " activated a Splash! a winner will be selected in the next messages!", ParseMode.Html);
                                    var i = 0;

                                    while (_messagesCount < waitMsg)
                                    {
                                        Thread.Sleep(1000);

                                        if (i == 30)
                                        {
                                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Timeout! Splash Aborted... sorry guys no winner :(", ParseMode.Html);
                                            return;
                                        }

                                        i++;
                                    }

                                    var lstAppUsers = _appUsersManagerService.GetLastMsgUsers(appuser.UserName, maxusers);

                                    await cmd_Send(message, appuser, lstAmount.FirstOrDefault(), lstAppUsers, "SPLASH!!!");
                                }

                                break;
                            }
                        // Boom!
                        case "!BOOM":
                            {
                                try
                                {
                                    if (lstAmount.Count > 1 && Math.Abs(lstAmount[1]) > 0)
                                    {
                                        maxusers = int.Parse(lstAmount[1].ToString(CultureInfo.InvariantCulture));
                                    }

                                    var lstAppUsers = _appUsersManagerService.GetBoomUsers(appuser.UserName, maxusers);

                                    if (await cmd_preSend(lstAmount.FirstOrDefault() - (lstAppUsers.Count * 0.1), command,
                                        lstAppUsers.Count, message.Chat.Id, appuser))
                                    {
                                        await cmd_Send(message, appuser,
                                            lstAmount.FirstOrDefault() - (lstAppUsers.Count * 0.1), lstAppUsers, "BOOM!!!");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Received Exception from Boom {0}" + ex.Message);
                                }

                                break;
                            }
                        // Let it Rain Rise
                        case "!RAIN":
                            {
                                if (lstAmount.Count > 1 && Math.Abs(lstAmount[1]) > 0)
                                {
                                    maxusers = int.Parse(lstAmount[1].ToString(CultureInfo.InvariantCulture));
                                }

                                var lstAppUsers = _appUsersManagerService.GetRainUsers(appuser.UserName, maxusers);

                                // Check before sending
                                if (await cmd_preSend(lstAmount.FirstOrDefault() - (lstAppUsers.Count * 0.1), command,
                                    lstAppUsers.Count, message.Chat.Id, appuser))
                                {
                                    await cmd_Send(message, appuser, lstAmount.FirstOrDefault() - (lstAppUsers.Count * 0.1),
                                        lstAppUsers, "its Raining!!!");
                                }

                                break;
                            }
                        // Withdraw coin to address
                        case "!SEND":
                            {
                                var lstAppUsers = new List<ApplicationUser>();

                                foreach (var user in lstDestUsers)
                                {
                                    var e = await _appUsersManagerService.GetUserAsync(user);

                                    if (e != null)
                                    {
                                        lstAppUsers.Add(e);
                                    }
                                }

                                if (lstAppUsers.Count >= 1)
                                {
                                    // Check before sending
                                    if (await cmd_preSend(lstAmount.FirstOrDefault() - (lstAppUsers.Count * 0.1), command, lstAppUsers.Count, message.Chat.Id, appuser))
                                    {
                                        await cmd_Send(message, appuser, lstAmount.FirstOrDefault() - (lstAppUsers.Count * 0.1), lstAppUsers, "wake up!!! its a wonderful day!");
                                    }
                                }
                                else
                                {
                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Users:" + string.Join(" ", lstDestUsers) + " were not found", ParseMode.Html);
                                }

                                break;
                            }
                        // Tell when last message from user
                        case "!SEEN":
                            {
                                var lstAppUsers = new List<ApplicationUser>();

                                foreach (var user in lstDestUsers)
                                {
                                    var e = await _appUsersManagerService.GetUserAsync(user);

                                    if (e != null)
                                    {
                                        lstAppUsers.Add(e);
                                    }
                                }

                                await cmd_Seen(appuser, message, lstAppUsers);
                                break;
                            }
                        // Info Price
                        case "!PRICE":
                            await cmd_Price(message);
                            break;
                        // Return a  geek joke
                        case "!JOKE":
                            await cmd_Joke(message);
                            break;
                        // Return a  geek joke
                        case "!HOPE":
                            await cmd_Hope(message);
                            break;
                        // Show Rise Exchanges
                        case "!EXCHANGES":
                            await cmd_Exchanges(message, appuser);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error parsing !command {0}" + ex.Message);
                }

                return;
            }
        }

        /// <summary>
        /// Show Help
        /// </summary>
        /// <param name="appuser"></param>
        /// <returns></returns>
        private async Task cmd_Help(ApplicationUser appuser)
        {
            try
            {
                await _botService.Client.SendChatActionAsync(appuser.TelegramId, ChatAction.Typing);

                var strResponse = "<b>-= Help =-</b>" + Environment.NewLine +
                                     "<b>!rain</b> - !rain 10 (to random users active min 3 msg)" + Environment.NewLine +
                                     "<b>!boom</b> - !boom 10 (to all users active the in last day min 2 msg)" + Environment.NewLine +
                                     "<b>!splash</b> - !splash 10 (winner will be in random in next max 10 msg)" + Environment.NewLine +
                                     "<b>!send</b> - !send 5 RISE to @Dwildcash" + Environment.NewLine +
                                     "<b>!withdraw</b> - !withdraw 5 RISE to 5953135380169360325R" + Environment.NewLine +
                                     "<b>!seen</b> - Show last message from user !seen @Dwildcash" + Environment.NewLine +
                                     "<b>!balance</b> - Show current RISE Balance" + Environment.NewLine +
                                     "<b>!joke</b> - Display a geek joke" + Environment.NewLine +
                                     "<b>!hope</b> - Show hope quote" + Environment.NewLine +
                                     "<b>!exchanges</b> - Display current RISE Exchanges" + Environment.NewLine +
                                     "<b>!price</b> - Show current RISE Price" + Environment.NewLine;

                await _botService.Client.SendTextMessageAsync(appuser.TelegramId, strResponse, ParseMode.Html);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Help {0}" + ex.Message);
            }
        }

        /// <summary>
        /// Send Hope Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Hope(Message message)
        {
            await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var lstHope = new List<string>
            {
                "You may say I'm a dreamer, but I'm not the only one. I hope someday you'll join us. And the world will live as one.",
                "I like the night. Without the dark, we'd never see the stars.",
                "They say a person needs just three things to be truly happy in this world: someone to love, something to do, and something to hope for.",
                "If you're reading this...Congratulations, you're alive. If that's not something to smile about, then I don't know what is",
                "And now these three remain: faith, hope and love. But the greatest of these is love.",
                "It's amazing how a little tomorrow can make up for a whole lot of yesterday.",
                "In a time of destruction, create something.",
                "You may choose to look the other way but you can never say again that you did not know.",
                "We need hope, or else we cannot endure.",
                "Do not lose hope — what you seek will be found. Trust ghosts. Trust those that you have helped to help you in their turn. Trust dreams. Trust your heart, and trust your story.",
                "There is nothing like hope to create the future.",
                "There is always hope!",
                "You might think I lost all hope at that point. I did. And as a result I perked up and felt much better.",
                "If at first the idea is not absurd, then there is no hope for it.",
                "Hope in reality is the worst of all evils because it prolongs the torments of man",
                "Yes We Can!",
                "She wondered that hope was so much harder then despair.",
                "A man devoid of hope and conscious of being so has ceased to belong to the future.",
                "Hope makes a merry heart.",
                "Hope is a strange commodity. It is an opiate. We swear we have relinquished it and, lo, here comes a day when, all unannounced, our enslavement to it returns.",
                "Stay hopeful.",
                "Do not dwell on your loss. Look forward with bright new hopes.",
                "Hope itself is like a star- not to be seen in the sunshine of prosperity, and only to be discovered in the night of adversity.",
                "There is a secret medicine given only to those who hurt so hard they can't hope",
                "We promise according to our hopes and perform according to our fears.",
                "To wish was to hope, and to hope was to expect",
                "May your choices reflect your hopes, not your fears.",
                "I inhale hope with every breath I take",
                "Hope does not leave without being given permission.",
                "Stay forever enthusiastic about your desirable dreams.",
                "Do not dwell on your loss. Look forward with bright new hopes.",
                "Stay hopeful.",
                "something was dead in each of us, and what was dead was hope.",
                "Education is the realization of hope for the future.",
                "You see, if we have no hope, we have nothing"
            };

            var random = new Random();
            var index = random.Next(lstHope.Count);
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, lstHope[index], ParseMode.Html);
        }

        /// <summary>
        /// Show Wallet Address and Key
        /// </summary>
        /// <param name="appuser"></param>
        private async Task cmd_Key(ApplicationUser appuser)
        {
            await _botService.Client.SendTextMessageAsync(appuser.TelegramId, "This is the private key for your TIP wallet account, please note it and delete this message!", ParseMode.Html);
            await _botService.Client.SendTextMessageAsync(appuser.TelegramId, "Address: <b>" + appuser.Address + "</b>", ParseMode.Html);
            await _botService.Client.SendTextMessageAsync(appuser.TelegramId, "Passphrase: " + appuser.GetSecret(), ParseMode.Html);
        }

        /// <summary>
        /// Withdraw coin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="amount"></param>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        private async Task cmd_Withdraw(ApplicationUser sender, double amount, string recipientId)
        {
            try
            {
                if (amount > 0 && !string.IsNullOrEmpty(recipientId))
                {
                    await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                    var balance = await RiseManager.AccountBalanceAsync(sender.Address);

                    if (balance >= amount)
                    {
                        var tx = await RiseManager.CreatePaiment((amount - 0.1) * 100000000, sender.GetSecret(), recipientId);

                        await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Successfully sent <b>" + amount + "</b> RISE to " + recipientId, ParseMode.Html);

                        var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + tx.transactionId));
                        await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Transaction Id:" + tx.transactionId + "", replyMarkup: keyboard);
                    }
                    else
                    {
                        await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Not enough RISE to Withdraw <b>" + amount + "</b> RISE balance" + balance + " RISE", ParseMode.Html);
                    }
                }
                else
                {
                    await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Please specify amount and address ex: !withdraw 10 5953135380169360325R", ParseMode.Html);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Withdraw {0}", ex.Message);
            }
        }

        /// <summary>
        /// Check Before Sending
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="command"></param>
        /// <param name="numReceivers"></param>
        /// <param name="chatId"></param>
        /// <param name="appuser"></param>
        /// <returns></returns>
        private async Task<bool> cmd_preSend(double amount, string command, int numReceivers, long chatId, ApplicationUser appuser)
        {
            try
            {
                if (numReceivers == 0)
                {
                    await _botService.Client.SendTextMessageAsync(chatId, "Sorry I do not find anyone to send RISE", ParseMode.Html);
                    return false;
                }

                if (amount <= 0.1)
                {
                    await _botService.Client.SendTextMessageAsync(chatId, "Yish! It make no sense to " + command + " amount lower than 0.1!", ParseMode.Html);
                    return false;
                }

                var balance = await RiseManager.AccountBalanceAsync(appuser.Address);

                if (balance < ((0.1 * numReceivers) + amount))
                {
                    await _botService.Client.SendTextMessageAsync(chatId, "Not enough RISE to " + command + " " + amount + " RISE to " + numReceivers + " users. RISE Balance:" + balance, ParseMode.Html);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_preSend private Message {0}", ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send Coin
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="amount"></param>
        /// <param name="destusers"></param>
        /// <param name="bannerMsg"></param>
        /// <returns></returns>
        private async Task cmd_Send(Message message, ApplicationUser sender, double amount, IReadOnlyCollection<ApplicationUser> destusers, string bannerMsg = "")
        {
            try
            {
                if (amount > 0 && destusers.Count > 0)
                {
                    await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                    var amountToSend = amount / destusers.Count;

                    var balance = await RiseManager.AccountBalanceAsync(sender.Address);

                    if (balance >= ((0.1 * destusers.Count) + amount))
                    {
                        foreach (var destuser in destusers.Where(x => x.Address != null))
                        {
                            var secret = sender.GetSecret();
                            var tx = await RiseManager.CreatePaiment(amountToSend * 100000000, secret, destuser.Address);
                            if (destusers.Count <= 15)
                            {
                                if (tx.success)
                                {
                                    try
                                    {
                                        await _botService.Client.SendTextMessageAsync(destuser.TelegramId,
                                        "You received " + amountToSend + " from @" + sender.UserName,
                                        ParseMode.Html);
                                        var keyboard = new InlineKeyboardMarkup(
                                        InlineKeyboardButton.WithUrl("See Transaction",
                                            "https://explorer.rise.vision/tx/" + tx.transactionId));
                                        await _botService.Client.SendTextMessageAsync(destuser.TelegramId,
                                        "Transaction Id:" + tx.transactionId + "", replyMarkup: keyboard);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError("Received Exception from cmd_Send private Message {0}",
                                        ex.Message);
                                    }
                                }
                            }
                        }

                        var destUsersUsername = string.Join(",", destusers.Select(x => "@" + x.UserName));
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, destUsersUsername + " " + bannerMsg + " thanks to @" + sender.UserName + " he sent <b>" + Math.Round(amountToSend, 3) + " RISE</b> to you :)", ParseMode.Html);
                    }
                    else
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Not enough RISE to send <b>" + amount + " RISE</b> to " + destusers.Count + " users. Balance: " + balance + " RISE", ParseMode.Html);
                    }
                }
                else
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please specify amount and user ex: !send 10 @Dwildcash", ParseMode.Html);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Send transaction {0}", ex.Message);
            }
        }

        /// <summary>
        /// Seen last User
        /// </summary>
        /// <param name="appuser"></param>
        /// <param name="message"></param>
        /// <param name="lookUsers"></param>
        /// <returns></returns>
        private async Task cmd_Seen(ApplicationUser appuser, Message message, List<ApplicationUser> lookUsers)
        {
            if (lookUsers == null) throw new ArgumentNullException(nameof(lookUsers));
            try
            {
                if (!lookUsers.Any())
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please provide a user name starting with <b>@</b> ex !seen @Dwildcash", ParseMode.Html);
                    return;
                }

                await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                foreach (var user in lookUsers)
                {
                    string strResponse;

                    if (user?.LastMessage != null)
                    {
                        var hours = Math.Round((DateTime.Now - user.LastMessage).TotalHours, 2);
                        var minutes = Math.Round((DateTime.Now - user.LastMessage).TotalMinutes, 2);
                        string showtime;

                        if (hours <= 1 && minutes > 0)
                        {
                            showtime = minutes + " minutes ago.";
                        }
                        else if (user.UserName == appuser.UserName)
                        {
                            showtime = "... @" + appuser.UserName + " problem finding yourself? ";
                        }
                        else
                        {
                            showtime = Math.Round(hours, 2) + " hours ago.";
                        }

                        strResponse = "Last Message from @" + user.UserName + " was " + showtime;
                    }
                    else
                    {
                        strResponse = "Sorry, I don't know @" + user.UserName;
                    }

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Seen {0}", ex.Message);
            }
        }

        /// <summary>
        /// Show User Balance
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task cmd_ShowUserBalance(ApplicationUser sender)
        {
            try
            {
                await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                if (!string.IsNullOrEmpty(sender.Address))
                {
                    var strResponse = "<b>Current Balance for </b>@" + sender.UserName + Environment.NewLine +
                                         "Address: <b>" + sender.Address + "</b>" + Environment.NewLine +
                                         "Balance <b>" + Math.Round(await RiseManager.AccountBalanceAsync(sender.Address), 4) + " RISE </b>";

                    if (string.IsNullOrEmpty(sender.UserName))
                    {
                        strResponse += Environment.NewLine + " Note: Please configure your Telegram UserName if you want to Receive <b>RISE</b>";
                    }

                    await _botService.Client.SendTextMessageAsync(sender.TelegramId, strResponse, ParseMode.Html);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_ShowUserBalance {0}", ex.Message);
            }
        }

        /// <summary>
        /// Show Chuck Norris Joke
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Joke(Message message)
        {
            await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            var strResponse = await QuoteOfTheDayManager.GetQuoteOfTheDay();

            if (strResponse != null)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }
        }

        /// <summary>
        /// Show Rise Exchanges
        /// </summary>
        /// <param name="message"></param>
        /// <param name="appuser"></param>
        /// <returns></returns>
        private async Task cmd_Exchanges(Message message, ApplicationUser appuser)
        {
            string strResponse;
            try
            {
                await _botService.Client.SendChatActionAsync(appuser.TelegramId, ChatAction.Typing);
                strResponse = "<b>-= Current Rise Exchanges =-</b>" + Environment.NewLine +
                "<b>Altilly</b> - https://altilly.com" + Environment.NewLine +
                "<b>Livecoin</b> - http://livecoin.net" + Environment.NewLine +
                "<b>RightBtc</b> - http://rightbtc.com" + Environment.NewLine +
                "<b>Vinex</b> - https://vinex.network";
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Exchanges {0}", ex.Message);
            }
        }

        /// <summary>
        /// Show Rise Price
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Price(Message message)
        {
            await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            var quote = await QuoteManager.GetRiseQuote();

            string strResponse = "Price (sat): <b>" + Math.Round(quote.Price * 100000000) + "</b>" + Environment.NewLine +
            "Usd Price: <b>$" + Math.Round(quote.USDPrice, 4) + "</b>" + Environment.NewLine +
            "Volume: <b>" + Math.Round(quote.Volume).ToString("N0") + "</b>";

            await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Rise.coinquote.io", "https://rise.coinquote.io"));
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "click to open website", replyMarkup: keyboard);
        }

        /// <summary>
        /// Send Rise Info
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Info(Message message)
        {
            string strResponse;

            try
            {
                strResponse = "<b>Rise Information/Tools</b>" + Environment.NewLine +
                "<b>Rise Website</b> - https://rise.vision" + Environment.NewLine +
                "<b>Rise RoadMap</b> - https://rise.vision/roadmap/" + Environment.NewLine +
                "<b>Rise Explorer</b> - https://explorer.rise.vision/" + Environment.NewLine +
                "<b>Rise GitHub</b> - https://github.com/RiseVision" + Environment.NewLine +
                "<b>Rise Web Wallet</b> - https://wallet.rise.vision" + Environment.NewLine +
                "<b>Rise Medium</b> - https://medium.com/rise-vision" + Environment.NewLine +
                "<b>Rise Dashboard</b> - https://rise.coinquote.io" + Environment.NewLine +
                "<b>Rise Force Game</b> - http://riseforce.io/" + Environment.NewLine +
                "<b>Rise Twitter</b> - https://twitter.com/RiseVisionTeam" + Environment.NewLine +
                "<b>Rise Telegram</b> - https://t.me/risevisionofficial" + Environment.NewLine +
                "<b>Rise TG Official Updates</b> - https://t.me/riseupdates" + Environment.NewLine +
                "<b>Rise Slack</b> - https://risevision.slack.com/" + Environment.NewLine +
                "<b>Rise Discord</b> - https://discord.gg/6jyWQnJ" + Environment.NewLine +
                "<b>Rise BitcoinTalk</b> - https://bitcointalk.org/index.php?topic=3211240.200" + Environment.NewLine +
                "<b>Rise Intro Youtube</b> - https://www.youtube.com/watch?v=wZ2vIGl_gCM&feature=youtu.be" + Environment.NewLine +
                "<b>Rise Telegram Tipping service</b> -!help";
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Info {0}", ex.Message);
            }
        }
    }
}