using Microsoft.Extensions.Logging;
using rise.Helpers;
using rise.Models;
using rise.Services;
using rise_lib;
using System;
using System.Collections.Generic;
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
        private static long messagesCount;

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
                messagesCount++;
            }

            // Get the user who sent message
            var appuser = await _appUsersManagerService.GetUserAsync(message.From.Username, message.From.Id, flagMsgUpdate);

            string command = string.Empty;
            List<string> lstDestUsers = new List<string>();
            List<string> lstDestAddress = new List<string>();
            List<double> lstAmount = new List<double>();

            try
            {
                // Match !Command if present
                if (Regex.Matches(message.Text, @"!(\S+)\s?").Count > 0)
                {
                    command = Regex.Matches(message.Text.ToUpper(), @"!(\S+)\s?")[0].ToString().Trim();
                }

                // Match @username if present
                if (Regex.Matches(message.Text, @"@(\S+)\s?").Count > 0)
                {
                    lstDestUsers = Regex.Matches(message.Text, @"@(\S+)\s?").Cast<Match>().Select(m => m.Value.Replace("@", "").Trim()).ToList();
                }

                // Match any double Amount if present
                if (Regex.Matches(message.Text, @"([ ]|(^|\s))?[0-9]+(\.[0-9]*)?([ ]|($|\s))").Count > 0)
                {
                    lstAmount = Regex.Matches(message.Text, @"([ ]|(^|\s))?[0-9]+(\.[0-9]*)?([ ]|($|\s))").Cast<Match>().Select(m => double.Parse(m.Value.Trim())).ToList();
                }

                // Match any Rise Address if present
                if (Regex.Matches(message.Text, @"\d+[R,r]").Count > 0)
                {
                    lstDestAddress = Regex.Matches(message.Text, @"\d+[R,r]").Cast<Match>().Select(m => m.Value.Trim()).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parsing parameters {0}" + ex.Message);
                return;
            }

            try
            {
                // Info command
                if (command == "!INFO")
                {
                    await cmd_Info(message);
                }

                // Show Balance
                if (command == "!BALANCE")
                {
                    await cmd_ShowUserBalance(appuser);
                }

                // Show Help
                if (command == "!HELP")
                {
                    await cmd_Help(message, appuser);
                }

                // Show Private Bip39
                if (command == "!KEY")
                {
                    await cmd_Key(appuser);
                }

                // Withdraw RISE to address
                if (command == "!WITHDRAW")
                {
                    await cmd_Withdraw(appuser, lstAmount.FirstOrDefault() - (0.1*lstDestAddress.Count()), lstDestAddress.FirstOrDefault());
                }

                // Splash!
                if (command == "!SPLASH")
                {
                    if (lstAmount.FirstOrDefault() <= 0.1)
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "It make no sense to Splash amount lower than 0.1! \xF0\x9F\x98\x94", ParseMode.Html);
                        return;
                    }

                    var balance = await RiseManager.AccountBalanceAsync(appuser.Address);

                    if (balance < (0.1 + lstAmount.FirstOrDefault()))
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Not enough RISE to !SPLASH " + lstAmount.FirstOrDefault() + " RISE Balance:" + balance, ParseMode.Html);
                        return;
                    }
                    else
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "@" + appuser.UserName + " activated a <b> " + lstAmount.FirstOrDefault() + " RISE Splash!</b> Be active! I will choose a winner in the next messages!", ParseMode.Html);
                    }

                    var waitMsg = messagesCount + (int)RandomGenerator.NextLong(1, 4);

                    int i = 0;

                    while (messagesCount < waitMsg)
                    {

                        Thread.Sleep(1000);

                        if (i == 30)
                        {
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Timeout! Splash Aborted... sorry no winner :(", ParseMode.Html);
                            return;
                        }

                        i++;
                    }

                    List<ApplicationUser> lstAppUsers = new List<ApplicationUser>();

                    lstAppUsers.Add(_appUsersManagerService.GetLastMsgUser(appuser.UserName));

                    await cmd_Send(message, appuser, lstAmount.FirstOrDefault(), lstAppUsers, "SPLASH!!!");
                }

                // Show Private Bip39
                if (command == "!VOTE")
                {
                    await cmd_Vote(message);
                }

                // Boom!
                if (command == "!BOOM")
                {
                    if (lstAmount.FirstOrDefault() <= 0.1)
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "It make no sense to boom amount lower than 0.1!", ParseMode.Html);
                        return;
                    }

                    List<ApplicationUser> lstAppUsers = _appUsersManagerService.GetBoomUsers(appuser.UserName);

                    await cmd_Send(message, appuser, lstAmount.FirstOrDefault()-(lstAppUsers.Count*0.1), lstAppUsers, "BOOM!!!");
                }

                // Let it Rain Rise
                if (command == "!RAIN")
                {
                    if (lstAmount.FirstOrDefault() <= 0.1)
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "It make no sense to rain amount lower than 0.1!", ParseMode.Html);
                        return;
                    }

                    List<ApplicationUser> lstAppUsers = _appUsersManagerService.GetRainUsers(appuser.UserName);

                    var balance = await RiseManager.AccountBalanceAsync(appuser.Address);

                    if (balance < ((0.1*lstAppUsers.Count) + lstAmount.FirstOrDefault()))
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Not enough RISE to !RAIN " + lstAmount.FirstOrDefault() + " RISE Balance:" + balance, ParseMode.Html);
                        return;
                    }
                   
                    await cmd_Send(message, appuser, lstAmount.FirstOrDefault()-(lstAppUsers.Count * 0.1), lstAppUsers, "its Raining!!!");
                }

                // Withdraw coin to address
                if (command == "!SEND")
                {
                    if (lstAmount.FirstOrDefault() <= 0.1)
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "It make no sense to send amount lower than 0.1", ParseMode.Html);
                        return;
                    }

                    List<ApplicationUser> lstAppUsers = new List<ApplicationUser>();

                    foreach (var user in lstDestUsers)
                    {
                        var e = _appUsersManagerService.GetUserByUsername(user);

                        if (e != null)
                        {
                            lstAppUsers.Add(e);
                        }
                    }

                    await cmd_Send(message, appuser, lstAmount.FirstOrDefault() - (lstAppUsers.Count * 0.1), lstAppUsers, "wake up!!!");
                }

                // Tell when last message from user
                if (command == "!SEEN")
                {
                    List<ApplicationUser> lstAppUsers = new List<ApplicationUser>();

                    foreach (var user in lstDestUsers)
                    {
                        var e = _appUsersManagerService.GetUserByUsername(user);

                        if (e != null)
                        {
                            lstAppUsers.Add(e);
                        }
                    }

                    await cmd_Seen(appuser, message, lstAppUsers);
                }

                // Info Price
                if (command == "!PRICE")
                {
                    await cmd_Price(message);
                }

                // Return a  geek joke
                if (command == "!JOKE")
                {
                    await cmd_Joke(message);
                }

                // Return a  geek joke
                if (command == "!HOPE")
                {
                    await cmd_Hope(message);
                }

                // Show Rise Exchanges
                if (command == "!EXCHANGES")
                {
                    await cmd_Exchanges(message, appuser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parsing !command {0}" + ex.Message);
                return;
            }
        }


        /// <summary>
        /// Show vote website
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Vote(Message message)
        {
            var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("RISE Voting", "https://vote.rise.vision/"));
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "click below to open RISE voting website", replyMarkup: keyboard);
        }


        /// <summary>
        /// Show Help
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Help(Message message, ApplicationUser appuser)
        {
            string strResponse = string.Empty;

            try
            {
                await _botService.Client.SendChatActionAsync(appuser.TelegramId, ChatAction.Typing);

                strResponse = "<b>-= Help =-</b>" + Environment.NewLine +
                "<b>!rain</b> - !rain 10 (to random users active in last 2 days)" + Environment.NewLine +
                "<b>!boom</b> - !boom 10 (to all users active the in last hour)" + Environment.NewLine +
                "<b>!splash</b> - !splash 10 (winner will be in random in next max 10 msg)" + Environment.NewLine +
                "<b>!send</b> - !send 5 RISE to @Dwildcash" + Environment.NewLine +
                "<b>!withdraw</b> - !withdraw 5 RISE to 5953135380169360325R" + Environment.NewLine +
                "<b>!seen</b> - Show last message from user !seen @Dwildcash" + Environment.NewLine +
                "<b>!balance</b> - Show current RISE Balance" + Environment.NewLine +
                "<b>!joke</b> - Display a geek joke" + Environment.NewLine +
                "<b>!hope</b> - Show hope quote" + Environment.NewLine +
                "<b>!exchanges</b> - Display current RISE Exchanges" + Environment.NewLine +
                "<b>!price</b> - Show current RISE Price" + Environment.NewLine +
                "<b>!key</b> - Send you Tip wallet passphrase in private msg" + Environment.NewLine;

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
            List<string> LstHope = new List<string>();

            LstHope.Add("You may say I'm a dreamer, but I'm not the only one. I hope someday you'll join us. And the world will live as one.");
            LstHope.Add("I like the night. Without the dark, we'd never see the stars.");
            LstHope.Add("They say a person needs just three things to be truly happy in this world: someone to love, something to do, and something to hope for.");
            LstHope.Add("If you're reading this...Congratulations, you're alive. If that's not something to smile about, then I don't know what is");
            LstHope.Add("And now these three remain: faith, hope and love. But the greatest of these is love.");
            LstHope.Add("It's amazing how a little tomorrow can make up for a whole lot of yesterday.");
            LstHope.Add("In a time of destruction, create something.");
            LstHope.Add("You may choose to look the other way but you can never say again that you did not know.");
            LstHope.Add("We need hope, or else we cannot endure.");
            LstHope.Add("Do not lose hope — what you seek will be found. Trust ghosts. Trust those that you have helped to help you in their turn. Trust dreams. Trust your heart, and trust your story.");
            LstHope.Add("There is nothing like hope to create the future.");
            LstHope.Add("There is always hope!");
            LstHope.Add("You might think I lost all hope at that point. I did. And as a result I perked up and felt much better.");
            LstHope.Add("If at first the idea is not absurd, then there is no hope for it.");
            LstHope.Add("Hope in reality is the worst of all evils because it prolongs the torments of man");
            LstHope.Add("Yes We Can!");
            LstHope.Add("She wondered that hope was so much harder then despair.");
            LstHope.Add("A man devoid of hope and conscious of being so has ceased to belong to the future.");
            LstHope.Add("Hope makes a merry heart.");
            LstHope.Add("Hope is a strange commodity. It is an opiate. We swear we have relinquished it and, lo, here comes a day when, all unannounced, our enslavement to it returns.");
            LstHope.Add("Stay hopeful.");
            LstHope.Add("Do not dwell on your loss. Look forward with bright new hopes.");
            LstHope.Add("Hope itself is like a star- not to be seen in the sunshine of prosperity, and only to be discovered in the night of adversity.");
            LstHope.Add("There is a secret medicine given only to those who hurt so hard they can't hope");
            LstHope.Add("We promise according to our hopes and perform according to our fears.");
            LstHope.Add("To wish was to hope, and to hope was to expect");
            LstHope.Add("May your choices reflect your hopes, not your fears.");
            LstHope.Add("I inhale hope with every breath I take");
            LstHope.Add("Hope does not leave without being given permission.");
            LstHope.Add("Stay forever enthusiastic about your desirable dreams.");
            LstHope.Add("Do not dwell on your loss. Look forward with bright new hopes.");
            LstHope.Add("Stay hopeful.");
            LstHope.Add("something was dead in each of us, and what was dead was hope.");
            LstHope.Add("Education is the realization of hope for the future.");
            LstHope.Add("You see, if we have no hope, we have nothing");

            var random = new Random();
            int index = random.Next(LstHope.Count);
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, LstHope[index], ParseMode.Html);
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
                double balance = 0;

                if (amount > 0 && !string.IsNullOrEmpty(recipientId))
                {
                    await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                    balance = await RiseManager.AccountBalanceAsync(sender.Address);

                    if (balance > (0.1 + amount))
                    {
                        var tx = await RiseManager.CreatePaiment(amount * 100000000, sender.GetSecret(), recipientId);

                        await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Successfully sent <b>" + amount + "</b> RISE to " + recipientId, ParseMode.Html);

                        var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + tx.transactionId));
                        await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Transaction Id:" + tx.transactionId + "", replyMarkup: keyboard);
                    }
                    else
                    {
                        await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Not enough RISE to Withdraw <b>" + amount + "</b> balance" + balance + " RISE", ParseMode.Html);
                        return;
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
        /// Withdraw coin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="amount"></param>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        private async Task cmd_Send(Message message, ApplicationUser sender, double amount, List<ApplicationUser> destusers, string bannerMsg = "")
        {
            double balance = 0;

            try
            {
                if (amount > 0 && destusers.Count > 0)
                {
                    await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                    double amountToSend = amount / destusers.Count();

                    balance = await RiseManager.AccountBalanceAsync(sender.Address);

                    if (balance >= ((0.1 * destusers.Count) + amount))
                    {
                        foreach (var destuser in destusers.Where(x => x.Address != null))
                        {
                            var tx = await RiseManager.CreatePaiment(amountToSend * 100000000, sender.GetSecret(), destuser.Address);

                            if (tx.success)
                            {
                                try
                                {
                                    await _botService.Client.SendTextMessageAsync(destuser.TelegramId, "You received " + amountToSend + " from @" + sender.UserName, ParseMode.Html);
                                    var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + tx.transactionId));
                                    await _botService.Client.SendTextMessageAsync(destuser.TelegramId, "Transaction Id:" + tx.transactionId + "", replyMarkup: keyboard);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Received Exception from cmd_Send private Message {0}", ex.Message);
                                }
                            }
                        }

                        var destUsersUsername = string.Join(",", destusers.Select(x => "@" + x.UserName));
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, destUsersUsername + " " + bannerMsg + " its a wonderful day!! thanks to @" + sender.UserName + " he sent <b>" + Math.Round(amountToSend, 3) + " RISE</b> to you :)", ParseMode.Html);
                    }
                    else
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Not enough RISE to send <b>" + amount + "</b> to " + destusers.Count() + " users. Balance: " + balance + " RISE", ParseMode.Html);
                        return;
                    }
                }
                else
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please specify amount and user ex: !send 10 @Dwildcash", ParseMode.Html);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Send transaction {0}", ex.Message);
            }
        }

        /// <summary>
        /// Tell when last user sent message on channel
        /// </summary>
        /// <param name="appuser"></param>
        /// <param name="message"></param>
        /// <param name="destUsers"></param>
        /// <returns></returns>
        private async Task cmd_Seen(ApplicationUser appuser, Message message, List<ApplicationUser> lookUsers)
        {
            try
            {
                var strResponse = string.Empty;

                if (lookUsers.Count() == 0)
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please provide a user name starting with <b>@</b> ex !seen @Dwildcash", ParseMode.Html);
                    return;
                }

                await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                foreach (var user in lookUsers)
                {
                    if (user?.LastMessage != null)
                    {
                        var lastseen = user.LastMessage;
                        double hours = Math.Round((DateTime.Now - user.LastMessage).TotalHours, 2);
                        double minutes = Math.Round((DateTime.Now - user.LastMessage).TotalMinutes, 2);
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
            string strResponse = string.Empty;

            try
            {
                await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                if (!string.IsNullOrEmpty(sender.Address))
                {
                    strResponse = "<b>Current Balance for </b>@" + sender.UserName + Environment.NewLine +
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
        /// Display a chuck Norris Joke
        /// </summary>
        /// <param name="chatId"></param>
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
        /// Display Current List Rise Exchanges
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Exchanges(Message message, ApplicationUser appuser)
        {
            var strResponse = string.Empty;
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
        /// Display RISE Current Price
        /// </summary>
        /// <param name="chatId"></param>
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
        /// Send RISE Info
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Info(Message message)
        {
            string strResponse = string.Empty;

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
                "<b>Rise Force Game</b> - http://duhec.net/riseForce" + Environment.NewLine +
                "<b>Rise Twitter</b> - https://twitter.com/RiseVisionTeam" + Environment.NewLine +
                "<b>Rise Telegram</b> - https://t.me/risevisionofficial" + Environment.NewLine +
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