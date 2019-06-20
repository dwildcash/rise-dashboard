using Microsoft.Extensions.Logging;
using rise.Code.DataFetcher;
using rise.Code.Rise;
using rise.Data;
using rise.Models;
using rise.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Rise.Services
{
    public class UpdateService : QuoteStats, IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;
        private readonly IAppUsersManagerService _appUsersManagerService;
        private readonly ApplicationDbContext _appdb;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, IAppUsersManagerService appUsersManagerService, ApplicationDbContext context)
        {
            _botService = botService;
            _appUsersManagerService = appUsersManagerService;
            _logger = logger;
            _appdb = context;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            var flagMsgUpdate = message.Chat.Id == AppSettingsProvider.TelegramChannelId;

            // Get the user who sent message
            var appuser = await _appUsersManagerService.GetUserAsync(message.From.Username, message.From.Id, flagMsgUpdate);

            try
            {
                if (message.PinnedMessage != null)
                {
                    TgPinnedMsg tgPinnedMsg = new TgPinnedMsg
                    {
                        Date = DateTime.Now,
                        AppUser = appuser,
                        Message = message.Text
                    };
                    await _appdb.AddAsync(tgPinnedMsg);
                    await _appdb.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving pinned msg {0}" + ex.Message);
                return;
            }

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
                                if (await cmd_preSend(lstAmount.FirstOrDefault() - (0.1 * lstDestAddress.Count), command,
                                    lstDestAddress.Count, message.Chat.Id, appuser))
                                {
                                    await cmd_Withdraw(appuser, lstAmount.FirstOrDefault() - (0.1 * lstDestAddress.Count),
                                        lstDestAddress.FirstOrDefault());
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
                        // send coin to address
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
                            await cmd_Price(appuser);
                            break;
                        // Return a  geek joke
                        case "!CHUCKNORRIS":
                            // await cmd_ChuckNorrisJoke(message);
                            break;
                        // Return a  geek joke
                        case "!JOKE":
                            // await cmd_Joke(message);
                            break;
                        // Return a  geek joke
                        case "!V2":
                            await cmd_V2(message);
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
                                     "<b>!send</b> - !send 5 RISE to @Dwildcash" + Environment.NewLine +
                                     "<b>!withdraw</b> - !withdraw 5 RISE to 5953135380169360325R" + Environment.NewLine +
                                     "<b>!seen</b> - Show last message from user !seen @Dwildcash" + Environment.NewLine +
                                     "<b>!balance</b> - Show current RISE Balance" + Environment.NewLine +
                                     //"<b>!joke</b> - Display a geek joke" + Environment.NewLine +
                                     //"<b>!chucknorris</b> - Display a chucknorris joke" + Environment.NewLine +
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
                "They say a person needs just three things to be truly happy in this world: someone to love, something to do, and something to hope for.",
                "And now these three remain: faith, hope and love. But the greatest of these is love.",
                "It's amazing how a little tomorrow can make up for a whole lot of yesterday.",
                "You may choose to look the other way but you can never say again that you did not know.",
                "We need hope, or else we cannot endure.",
                "Do not lose hope — what you seek will be found. Trust ghosts. Trust those that you have helped to help you in their turn. Trust dreams. Trust your heart, and trust your story.",
                "There is nothing like hope to create the future.",
                "There is always hope!",
                "You might think I lost all hope at that point. I did. And as a result I perked up and felt much better.",
                "If at first the idea is not absurd, then there is no hope for it.",
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
                "Education is the realization of hope for the future.",
                "Optimism is the faith that leads to achievement. Nothing can be done without hope and confidence.",
                "Never lose hope. Storms make people stronger and never last forever.",
                "We must accept finite disappointment, but never lose infinite hope",
                "Hope itself is like a star – not to be seen in the sunshine of prosperity, and only to be discovered in the night of adversity.",
                "Walk on with hope in your heart, and you’ll never walk alone.",
                "Everything that is done in this world is done by hope.",
                "A person can do incredible things if he or she has enough hope.",
                "Faith goes up the stairs that love has built and looks out the windows which hope has opened.",
                "The difference between hope and despair is a different way of telling stories from the same facts.",
                "Hope is a waking dream.",
                "We need hope, or else we cannot endure.",
                "When you have lost hope, you have lost everything. And when you think all is lost, when all is dire and bleak, there is always hope.",
                "When you’re at the end of your rope, tie a knot and hold on.",
                "Hope means hoping when everything seems hopeless",
                "If it were not for hopes, the heart would break.",
                "That was all a man needed: hope. It was lack of hope that discouraged a man.",
                "Never lose faith in yourself, and never lose hope; remember, even when this world throws its worst and then turns its back, there is still always hope.",
                "Hope?’ he says. ‘There is always hope, John. New developments have yet to present themselves. Not all the information is in. No. Don’t give up hope just yet. It’s the last thing to go. When you have lost hope, you have lost everything. And when you think all is lost, when all is dire and bleak, there is always hope.",
                "It’s probably my job to tell you life isn’t fair, but I figure you already know that. So instead, I’ll tell you that hope is precious, and you’re right not to give up.",
                "Where there is no hope, it is incumbent on us to invent it.",
                "Hope is a force of nature. Don’t let anyone tell you different.",
                "Hope is the power of being cheerful in circumstances that we know to be desperate.",
                "You see, if we have no hope, we have nothing"
            };

            var random = new Random();
            var index = random.Next(lstHope.Count);
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, lstHope[index], ParseMode.Html);
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
                        var tx = await RiseManager.CreatePaiment(amount * 100000000, sender.GetSecret(), recipientId);

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
                            if (destusers.Count <= 15 && tx.success)
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
                                        "Transaction Id:" + tx.transactionId, replyMarkup: keyboard);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Received Exception from cmd_Send private Message {0}",
                                        ex.Message);
                                }
                            }
                        }

                        var destUsersUsername = string.Join(",", destusers.Select(x => "@" + x.UserName));
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, destUsersUsername + " " + bannerMsg + "  @" + sender.UserName + " sent you <b>" + Math.Round(amountToSend, 3) + " RISE</b> :)", ParseMode.Html);
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
                coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now.AddDays(-1)).ToList();
                var quote = LastAllQuote();

                if (!string.IsNullOrEmpty(sender.Address))
                {
                    var balance = Math.Round(await RiseManager.AccountBalanceAsync(sender.Address), 4);
                    var strResponse = "<b>Current Balance for </b>@" + sender.UserName + Environment.NewLine +
                                      "Address: <b>" + sender.Address + "</b>" + Environment.NewLine +
                                      "Balance: <b>" + balance + " RISE </b>" + Environment.NewLine +
                                      "USD: <b>" + Math.Round(balance * quote.USDPrice, 4) + "$</b>";

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
        /// Show a random geek joke
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_ChuckNorrisJoke(Message message)
        {
            await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            var strResponse = await ChuckNorrisJokeFetcher.FetchChuckNorrisJoke();

            if (strResponse != null)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse.value, ParseMode.Html);
            }
        }

        private async Task cmd_V2(Message message)
        {
            try
            {
                await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                var strResponse = @"Thanks to yarn and Lerna we were able to rewrite all main RISE functionalities separating the concerns of each module.  The underlying technology of the RISE core needs to be constantly updated to leverage both performance and security improvements." + Environment.NewLine +
                                "With this in mind we decided to update:" + Environment.NewLine +
                                "Node.JS from v8 to v10" + Environment.NewLine +
                                "TypeScript from 2.8 to 3.4.5" + Environment.NewLine +
                                "PostgresSQL from 10.4 to 11.3" + Environment.NewLine +
                                "We then decided to remove Redis entirely as it was no longer used in the codebase and there was no real reason to keep it as third - party dep.";

                using (var stream = System.IO.File.Open("./wwwroot/img/v2.png", FileMode.Open))
                {
                    var rep = await _botService.Client.SendPhotoAsync(message.Chat.Id, stream, "Rise vision Core Update V2");
                }

                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Medium Article", "https://medium.com/rise-vision/introducing-rise-v2-521a58e1e9de"));
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "More details here", replyMarkup: keyboard);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_V2 {0}", ex.Message);
            }
        }

        /// <summary>
        /// Show Chuck Norris Joke
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Joke(Message message)
        {
            try
            {
                await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                var strResponse = await JokeFetcher.FetchJoke();

                if (strResponse != null)
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse.attachments.FirstOrDefault().text, ParseMode.Html);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Joke {0}", ex.Message);
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
            try
            {
                double volBitker = 0;
                double priceBitker = 0;

                double volooobtc = 0;
                double priceooobtc = 0;

                double volAltilly = 0;
                double priceAltilly = 0;

                double volLivecoin = 0;
                double priceLivecoin = 0;

                double volVinex = 0;
                double priceVinex = 0;

                coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now.AddDays(-1)).ToList();

                var quoteAltilly = LastAltillyQuote();
                var quoteVinex = LastVinexQuote();
                var quoteooobtc = LastooobtcQuote();
                var quoteLivecoin = LastLiveCoinQuote();
                var quoteBitker = LastBitkerQuote();

                if (quoteAltilly != null)
                {
                    volAltilly = Math.Round(quoteAltilly.Volume);
                    priceAltilly = Math.Round(quoteAltilly.Price * 100000000);
                }

                if (quoteBitker != null)
                {
                    volBitker = Math.Round(quoteBitker.Volume);
                    priceBitker = Math.Round(quoteBitker.Price * 100000000);
                }

                if (quoteVinex != null)
                {
                    volVinex = Math.Round(quoteVinex.Volume);
                    priceVinex = Math.Round(quoteVinex.Price * 100000000);
                }

                if (quoteLivecoin != null)
                {
                    volLivecoin = Math.Round(quoteLivecoin.Volume);
                    priceLivecoin = Math.Round(quoteLivecoin.Price * 100000000);
                }

                if (quoteooobtc != null)
                {
                    volooobtc = Math.Round(quoteooobtc.Volume);
                    priceooobtc = Math.Round(quoteooobtc.Price * 100000000);
                }

                await _botService.Client.SendChatActionAsync(appuser.TelegramId, ChatAction.Typing);
                var strResponse = "<b>-= Current Rise Exchanges =-</b>" + Environment.NewLine +
                                  "<b>ooobtc</b> - https://www.ooobtc.com/ (24h V:" + volooobtc.ToString("N0") + " - Price:" + priceooobtc + ")" + Environment.NewLine +
                                  "<b>Bitker</b> - https://www.bitker.com/ (24h V:" + volBitker.ToString("N0") + " - Price:" + priceBitker + ")" + Environment.NewLine +
                                  "<b>Altilly</b> - https://altilly.com  (24h V:" + volAltilly.ToString("N0") + " - Price:" + priceAltilly + ")" + Environment.NewLine +
                                  "<b>Livecoin</b> - https://livecoin.net (24h V:" + volLivecoin.ToString("N0") + " - Price:" + priceLivecoin + ")" + Environment.NewLine +
                                  "<b>Vinex</b> - https://vinex.network (24h V:" + volVinex.ToString("N0") + " - Price:" + priceVinex + ")";
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
        private async Task cmd_Price(ApplicationUser sender)
        {
            try
            {
                await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);
                this.coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now.AddDays(-7)).ToList();
                var quote = LastAllQuote();

                var strResponse = "Price (sat): <b>" + Math.Round(quote.Price * 100000000) + " (24h:" + Math.Round(PercentChange(1), 2) + "%) (1w: " + Math.Round(PercentChange(7), 2) + "%)</b>" + Environment.NewLine +
                                  "USD Price: <b>$" + Math.Round(quote.USDPrice, 4) + " (" + USDPricePercentChange(1) + "%)</b>" + Environment.NewLine +
                                  "Volume: <b>" + Math.Round(quote.Volume).ToString("N0") + " (" + VolumePercentChange(1) + "%) </b>" + Environment.NewLine +
                                  "Day Range: <b>" + Math.Round(DaysLow(1) * 100000000) + " - " + Math.Round(DaysHigh(1) * 100000000) + " sat</b>";

                await _botService.Client.SendTextMessageAsync(sender.TelegramId, strResponse, ParseMode.Html);
                var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Rise.coinquote.io", "https://rise.coinquote.io"));
                await _botService.Client.SendTextMessageAsync(sender.TelegramId, "click to open website", replyMarkup: keyboard);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Price {0}", ex.Message);
            }
        }

        /// <summary>
        /// Send Rise Info
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Info(Message message)
        {
            try
            {
                var strResponse = "<b>Rise Information/Tools</b>" + Environment.NewLine +
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