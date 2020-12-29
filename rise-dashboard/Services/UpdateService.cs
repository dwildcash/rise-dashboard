using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rise.Code.DataFetcher;
using rise.Code.Rise;
using rise.Data;
using rise.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Emoji = rise.Code.Rise.Emoji;

namespace rise.Services
{
    public class UpdateService : QuoteStats, IUpdateService
    {
        private readonly IBotService _botService;
        private readonly IAppUsersManagerService _appUsersManagerService;
        private readonly ApplicationDbContext _appdb;

        public UpdateService(IBotService botService, IAppUsersManagerService appUsersManagerService, ApplicationDbContext context)
        {
            _botService = botService;
            _appUsersManagerService = appUsersManagerService;
            _appdb = context;
        }

        public async Task EchoAsync(Update update)
        {
            // Check if we have a message.
            if (update == null || update.Type != UpdateType.Message || update.Message.Text.Length == 0)
            {
                return;
            }

            var message = update.Message;
            var flagMsgUpdate = message.Chat.Id == AppSettingsProvider.TelegramChannelId;
            var maxusers = 5;
            var botcommands = new List<string>();
            var lstDestUsers = new List<string>();
            var lstDestAddress = new List<string>();
            var lstAmount = new List<double>();
            var lstAppUsers = new List<ApplicationUser>();

            // Get the user who sent message
            var appuser = await _appUsersManagerService.GetUserAsync(message.From.Username, message.From.Id, flagMsgUpdate);

            /* Configure Photo Url
            try
            {
                var file_id = _botService.Client.GetUserProfilePhotosAsync(message.From.Id).Result.Photos[0][0].FileId;
                var file = _botService.Client.GetFileAsync(file_id);
                var filepath = file.Result.FilePath;
                var photo_url = "https://api.telegram.org/file/bot" + AppSettingsProvider.BotApiKey + "/" + filepath;
                _appUsersManagerService.Update_Photourl(appuser.TelegramId, photo_url);
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
            }*/

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
                    lstDestUsers = Regex.Matches(message.Text, @"@(\S+)\s?").Select(m => m.Value.Replace("@", string.Empty).Trim()).ToList();
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
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();

                return;
            }

            foreach (var command in botcommands)
            {
                switch (command.Trim())
                {
                    case "!RISEFORCE":

                        RiseManager rm = new RiseManager();

                        var balance = await rm.AccountBalanceAsync("11843004005205985384R");

                        if (lstAmount.Count > 0)
                        {
                            double amount = Math.Round(lstAmount.FirstOrDefault() - 0.1, 2);
                            if (await cmd_preSend(amount, command, 1, message.Chat.Id, appuser))
                            {
                                await cmd_Withdraw(appuser, amount, "11843004005205985384R");
                            }
                            else
                            {
                                break;
                            }

                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thank you " + appuser.UserName + " for supporting RiseForce with " + Math.Round(amount, 2) + " <b>RISE</b> added to the season Jackpot!", ParseMode.Html);
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, Emoji.Rocket + " Current RiseForce Jackpot confirmed on blockchain: <b>" + balance + " </b> RISE" + " ( + " + Math.Round(amount, 2) + " unconfirmed.)", ParseMode.Html);
                        }

                        else
                        {
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, Emoji.Rocket + " Current RiseForce Jackpot confirmed Balance: <b>" + balance + " </b> RISE", ParseMode.Html);

                            var RiseForceResult = await RiseForceScoreFetcher.FetchRiseForceStats();

                            var stringTopScore = string.Empty;
                            int i = 0;

                            foreach (var user in RiseForceResult.result.top10.OrderByDescending(x => x.score).ToList())
                            {
                                if (i != 0)
                                {
                                    stringTopScore += user.name + " <b>" + user.score + "</b>" + Environment.NewLine;
                                }

                                else
                                {
                                    stringTopScore += Emoji.Star + " " + user.name + " <b>" + user.score + "</b>" + Emoji.Star + Environment.NewLine;
                                }

                                i++;
                            }

                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "<b>TOP 10 SCORE </b>" + Environment.NewLine + stringTopScore, ParseMode.Html);
                        }
                        break;

                    // Show Pool
                    case "!POOL":
                        await cmd_ShowPool(appuser, lstDestAddress);
                        break;

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

                    case "!CLEARLOGS":
                        await cmd_ClearLogs(appuser);
                        break;

                    // Withdraw RISE to address
                    case "!WITHDRAW":
                        {
                            double amount = Math.Round(lstAmount.FirstOrDefault() - 0.1, 2);

                            if (await cmd_preSend(amount, command, 1, message.Chat.Id, appuser))
                            {
                                await cmd_Withdraw(appuser, amount, lstDestAddress.FirstOrDefault());
                            }

                            break;
                        }

                    //// Boom!
                    case "!BOOM":
                        {
                            try
                            {
                                double amount = 0;

                                //fetch the second number for the new max number of people.
                                if (lstAmount.Count > 1 && Math.Abs(lstAmount[1]) > 0)
                                {
                                    maxusers = int.Parse(lstAmount[1].ToString(CultureInfo.InvariantCulture));
                                }
                                else if (lstAmount.Count == 0)
                                {
                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, Emoji.Joy + " You forgot to enter a RISE amount!", ParseMode.Html);
                                    return;
                                }

                                amount = Math.Round(lstAmount.FirstOrDefault() - (0.1 * maxusers), 2);

                                lstAppUsers = _appUsersManagerService.GetBoomUsers(appuser.UserName, maxusers);

                                if (await cmd_preSend(amount, command, lstAppUsers.Count, message.Chat.Id, appuser))
                                {
                                    var txtmsg = string.Empty;

                                    if (amount >= 90)
                                    {
                                        txtmsg = Emoji.Boom + Emoji.Boom + Emoji.Boom + " KABOOM!!! " + Emoji.Boom + Emoji.Boom + Emoji.Boom + " its a wonderful day for active users!";
                                    }
                                    else if (amount >= 10)
                                    {
                                        txtmsg = Emoji.Boom + Emoji.Boom + " BOOM!!! " + Emoji.Boom + Emoji.Boom + " its a nice day for active users!";
                                    }
                                    else
                                    {
                                        txtmsg = Emoji.Boom + " Hola!! " + Emoji.Boom + " its a good day for active users! ";
                                    }

                                    await cmd_Send(message, appuser, amount, lstAppUsers, txtmsg);
                                }
                            }
                            catch (Exception ex)
                            {
                                var log = new Log();
                                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                                _appdb.Logger.Add(log);
                                _appdb.SaveChangesAsync().Wait();
                            }
                        }
                        break;
                    // Let it Rain Rise
                    case "!RAIN":
                        {
                            double amount = 0;

                            //fetch the second number for the new max number of people.
                            if (lstAmount.Count > 1 && Math.Abs(lstAmount[1]) > 0)
                            {
                                maxusers = int.Parse(lstAmount[1].ToString(CultureInfo.InvariantCulture));
                            }
                            else if (lstAmount.Count == 0)
                            {
                                await _botService.Client.SendTextMessageAsync(message.Chat.Id, Emoji.Joy + " You forgot to enter a RISE amount!", ParseMode.Html);
                                return;
                            }

                            amount = Math.Round(lstAmount.FirstOrDefault() - (0.1 * maxusers), 2);

                            lstAppUsers = _appUsersManagerService.GetRainUsers(appuser.UserName, maxusers);

                            // Check before sending
                            if (await cmd_preSend(amount, command, lstAppUsers.Count, message.Chat.Id, appuser))
                            {
                                var txtmsg = string.Empty;

                                if (amount >= 90)
                                {
                                    txtmsg = Emoji.Star + Emoji.Star + Emoji.Star + " RAIN RAIN!!!! " + Emoji.Star + Emoji.Star + Emoji.Star + " its a wonderful rainy day for last day active users!";
                                }
                                else if (amount >= 10)
                                {
                                    txtmsg = Emoji.Star + Emoji.Star + " RAIN!!! " + Emoji.Star + Emoji.Star + " its a nice day for last day active users.!";
                                }
                                else
                                {
                                    txtmsg = Emoji.Star + " RAIN!!! " + Emoji.Star + " its a good day for last day active users!";
                                }

                                await cmd_Send(message, appuser, amount, lstAppUsers, txtmsg);
                            }
                        }
                        break;

                    // send coin to address
                    case "!SEND":
                        {
                            if (lstAmount.Count == 0)
                            {
                                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "You forgot to enter a rise amount!", ParseMode.Html);
                                return;
                            }

                            double amount = Math.Round(lstAmount.FirstOrDefault() - 0.1, 2);

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
                                if (await cmd_preSend(amount, command, lstAppUsers.Count, message.Chat.Id, appuser))
                                {
                                    var txtmsg = string.Empty;

                                    if (amount >= 90)
                                    {
                                        txtmsg = Emoji.Money_With_Wings + " wake up!!! its a wonderful day! " + Emoji.Money_With_Wings;
                                    }
                                    else if (amount >= 10)
                                    {
                                        txtmsg = Emoji.Moneybag + " wake up!!! its a nice day! " + Emoji.Moneybag;
                                    }
                                    else
                                    {
                                        txtmsg = Emoji.Star + " wake up!! its an good day!" + Emoji.Star;
                                    }

                                    await cmd_Send(message, appuser, amount, lstAppUsers, txtmsg);
                                }
                            }
                            else if (lstAppUsers.Count == 0)
                            {
                                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please tell me a user to send to!.", ParseMode.Html);
                            }
                            else
                            {
                                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please provide a valid user. Dont forget the @username", ParseMode.Html);
                            }
                        }
                        break;

                    // Tell when last message from user
                    case "!SEEN":
                        {
                            lstAppUsers = new List<ApplicationUser>();

                            foreach (var user in lstDestUsers)
                            {
                                var e = await _appUsersManagerService.GetUserAsync(user);

                                if (e != null)
                                {
                                    lstAppUsers.Add(e);
                                }
                            }

                            await cmd_Seen(appuser, message, lstAppUsers);
                        }
                        break;

                    // Info Price
                    case "!PRICE":
                        await cmd_Price(message, appuser);
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
        }


        /// <summary>
        /// Show Help
        /// </summary>
        /// <param name="appuser"></param>
        /// <returns></returns>
        private async Task cmd_ClearLogs(ApplicationUser appuser)
        {
            if (appuser.UserName == "Dwildcash")
            {
                _appdb.Logger.RemoveRange(_appdb.Logger);
                await _appdb.SaveChangesAsync();

                await _botService.Client.SendTextMessageAsync(appuser.TelegramId, "All logs deleted", ParseMode.Html);
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
                                     "<b>!balance</b> - Show your current balance and wallet address" + Environment.NewLine +
                                     "<b>!pool</b> - Show a a list of all active Pools" + Environment.NewLine +
                                     "<b>!rain</b> - !rain 10 (to any random users active in last day with min 5 msg)" + Environment.NewLine +
                                     "<b>!boom</b> - !boom 10 (to any random users active in the last 4 hours with min of 5 msg)" + Environment.NewLine +
                                     "<b>!send</b> - !send 5 RISE to @Dwildcash" + Environment.NewLine +
                                     "<b>!withdraw</b> - !withdraw 5 RISE to 5953135380169360325R" + Environment.NewLine +
                                     "<b>!RiseForce</b> - Send Rise to RiseForce JackPot" + Environment.NewLine +
                                     "<b>!seen</b> - Show last message from user !seen @Dwildcash" + Environment.NewLine +
                                     "<b>!hope</b> - Show hope quote" + Environment.NewLine +
                                     "<b>!exchanges</b> - Display current RISE Exchanges" + Environment.NewLine +
                                     "<b>!price</b> - Show current RISE Price" + Environment.NewLine;

                await _botService.Client.SendTextMessageAsync(appuser.TelegramId, strResponse, ParseMode.Html);
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
        /// Test Function
        /// </summary>
        /// <param name="message"></param>
        /// <param name="appuser"></param>
        /// <returns></returns>
        private async Task cmd_TestFunction(Message message, ApplicationUser appuser)
        {
            try
            {
                var myurl = appuser.Photo_Url.Replace("https", "http").TrimEnd();
                var request = WebRequest.Create(myurl);

                using (var response = request.GetResponse())
                {
                    using (var image = new Bitmap(Image.FromStream(response.GetResponseStream())))
                    {
                        var resized = new Bitmap(50, 50);
                        using (var graphics = Graphics.FromImage(resized))
                        {
                            graphics.CompositingQuality = CompositingQuality.HighSpeed;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.DrawImage(image, 0, 0, 50, 50);

                            resized.Save("/tmp/e.png", ImageFormat.Png);

                            using (var fileStream = new FileStream("/tmp/e.png", FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                await _botService.Client.SendPhotoAsync(
                                    chatId: message.Chat.Id,
                                    photo: new InputOnlineFile(fileStream),
                                    caption: "Screenshot !"
                                );
                            }
                        }
                    }
                }
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
            RiseManager rm = new RiseManager();

            try
            {
                if (amount > 0 && !string.IsNullOrEmpty(recipientId))
                {
                    await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                    var balance = await rm.AccountBalanceAsync(sender.Address);

                    if (balance >= (amount + 0.1))
                    {
                        var tx = await rm.CreatePaimentAsync(amount * 100000000, sender.GetSecret(), recipientId);

                        if (tx.success)
                        {
                            await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Successfully sent <b>" + amount + "</b> RISE to " + recipientId, ParseMode.Html);

                            var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + tx.transactionId));
                            await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Transaction Id:" + tx.transactionId + "", replyMarkup: keyboard);
                        }
                        else
                        {
                            await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Error withdrawing <b>" + amount + "</b> RISE to " + recipientId + " try to reduce the amount...", ParseMode.Html);
                        }
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
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
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
                if (numReceivers > 30)
                {
                    await _botService.Client.SendTextMessageAsync(chatId, Emoji.Point_Up + " Please use a maximum of 30 users!", ParseMode.Html);
                    return false;
                }

                if (numReceivers == 0)
                {
                    await _botService.Client.SendTextMessageAsync(chatId, Emoji.No_Good + " Sorry I did not find any user to send RISE :(", ParseMode.Html);
                    return false;
                }

                if (amount <= 0.1)
                {
                    await _botService.Client.SendTextMessageAsync(chatId, Emoji.Scream_Cat + "It make no sense to " + command + " amount lower than 0.1! (network fees are 0.1 RISE!!)", ParseMode.Html);
                    return false;
                }

                RiseManager rm = new RiseManager();

                var balance = await rm.AccountBalanceAsync(appuser.Address);

                if (balance < ((0.1 * numReceivers) + amount))
                {
                    await _botService.Client.SendTextMessageAsync(chatId, Emoji.Astonished + " Not enough RISE to " + command + " " + amount + " RISE to " + numReceivers + " users. RISE Balance:" + balance, ParseMode.Html);
                    return false;
                }
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
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

            await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

            var amountToSend = amount / destusers.Count;

            foreach (var destuser in destusers.Where(x => x.Address != null))
            {
                try
                {
                    var secret = sender.GetSecret();
                    RiseManager rm = new RiseManager();

                    var tx = await rm.CreatePaimentAsync(amountToSend * 100000000, secret, destuser.Address);

                    if (tx != null && tx.success)
                    {
                        await _botService.Client.SendTextMessageAsync(destuser.TelegramId, "You received " + amountToSend + " from @" + sender.UserName, ParseMode.Html);
                        var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + tx.transactionId));
                        await _botService.Client.SendTextMessageAsync(destuser.TelegramId, "Transaction Id:" + tx.transactionId, replyMarkup: keyboard);
                    }
                    else
                    {
                        var log = new Log();
                        log.LogMessage("Error processing transaction for " + destuser.UserName + " Id:" + destuser.TelegramId);
                        _appdb.Logger.Add(log);
                        _appdb.SaveChangesAsync().Wait();
                    }
                }
                catch (Exception ex)
                {
                    var log = new Log();
                    log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                    _appdb.Logger.Add(log);
                    _appdb.SaveChangesAsync().Wait();
                }
            }

            var destUsersUsername = string.Join(",", destusers.Select(x => "@" + x.UserName));
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, destUsersUsername + " " + bannerMsg + "  @" + sender.UserName + " sent you <b>" + Math.Round(amountToSend, 3) + " RISE</b> :)", ParseMode.Html);
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
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, Emoji.Eyes + " Please provide a user name starting with <b>@</b> ex !seen @Dwildcash", ParseMode.Html);
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
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
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
                    RiseManager rm = new RiseManager();
                    var balance = Math.Round(await rm.AccountBalanceAsync(sender.Address), 4);
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
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
            }
        }

        /// <summary>
        /// Show Pools
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task cmd_ShowPool(ApplicationUser sender, List<string> DestList)
        {
            try
            {
                WalletAccountResult walletAccountResult = null;
                string estimateAward = null;

                await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);

                if (DestList.Count > 0)
                {
                    using (var hc = new HttpClient())
                    {
                        var result = JObject.Parse(await hc.GetStringAsync(AppSettingsProvider.APIUrl + "/api/accounts?address=" + DestList.FirstOrDefault()));
                        walletAccountResult = JsonConvert.DeserializeObject<WalletAccountResult>(result.ToString());
                    }
                }

                // Show All Pool
                foreach (var pool in _appdb.DelegateForms)
                {
                    var mydelegate = DelegateResult.Current.Delegates.Where(o => o.Address == pool.Address).FirstOrDefault();

                    if (walletAccountResult != null)
                    {
                        double d = @Math.Round((double)mydelegate.ForgingChance / 100.0 * (double)AppSettingsProvider.CoinRewardDay * double.Parse(walletAccountResult.account.Balance) / 100000000 / ((mydelegate.VotesWeight / 100000000) + double.Parse(walletAccountResult.account.Balance) / 100000000) * (double)pool.Share / 100, 1);
                        estimateAward = "est. daily reward:" + d + " Rise";
                    }

                    await _botService.Client.SendTextMessageAsync(sender.TelegramId, "<b>" + mydelegate.Username + "</b> " + "Sharing " + "<b>" + pool.Share + "%</b> every " + pool.Payout_interval + " day " + estimateAward, ParseMode.Html);
                }

                var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Pools List", "https://dashboard.rise.vision/DelegateForms"));
                await _botService.Client.SendTextMessageAsync(sender.TelegramId, "click to see more details", replyMarkup: keyboard);
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
        /// Show Rise Exchanges
        /// </summary>
        /// <param name="message"></param>
        /// <param name="appuser"></param>
        /// <returns></returns>
        private async Task cmd_Exchanges(Message message, ApplicationUser appuser)
        {
            try
            {
                double volXtcom = 0;
                double priceXtcom = 0;

                coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now.AddDays(-1)).ToList();

                var quoteXtcom = LastXtcomQuote();

                if (quoteXtcom != null)
                {
                    volXtcom = Math.Round(quoteXtcom.Volume);
                    priceXtcom = Math.Round(quoteXtcom.Price * 100000000);
                }

                await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                var strResponse = "<b>-= Current Rise Exchanges =-</b>" + Environment.NewLine +
                                  "<b>Xt.com</b> - https://www.xt.com" + Environment.NewLine +
                                  "Price (sat): <b>" + priceXtcom + "</b>" + Environment.NewLine +
                                  "Vol (24H): <b>" +  volXtcom.ToString("N0") + "</b>" + Environment.NewLine +
                                  "Bitcoin Price: <b>" + Math.Round(double.Parse(CoinbaseBtcQuote.Current.amount), 2) + "$</b> (Coinbase)";

                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
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
        /// Show Rise Price
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Price(Message message, ApplicationUser appuser)
        {
            try
            {
                await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                this.coinQuoteCol = _appdb.CoinQuotes.Where(x => x.TimeStamp >= DateTime.Now.AddDays(-7)).ToList();
                var quote = LastAllQuote();

                var strResponse = "Price (sat): <b>" + Math.Round(quote.Price * 100000000) + " (24h:" + Math.Round(PercentChange(1), 2) + "%) (1w: " + Math.Round(PercentChange(7), 2) + "%)</b>" + Environment.NewLine +
                                  "USD Price: <b>$" + Math.Round(quote.USDPrice, 4) + " (" + USDPricePercentChange(1) + "%)</b>" + Environment.NewLine +
                                  "Volume: <b>" + Math.Round(quote.Volume).ToString("N0") + " (" + VolumePercentChange(1) + "%) </b>" + Environment.NewLine +
                                  "Day Range: <b>" + Math.Round(DaysLow(1) * 100000000) + " - " + Math.Round(DaysHigh(1) * 100000000) + " sat</b>" + Environment.NewLine +
                                  "Bitcoin Price: <b>" + Math.Round(double.Parse(CoinbaseBtcQuote.Current.amount), 2).ToString("N0") + "$</b> (Coinbase)";

                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("dashboard.rise.vision", "https://dashboard.rise.vision"));
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "click to open website", replyMarkup: keyboard);
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
                                  "<b>Rise Dashboard</b> - https://dashboard.rise.vision" + Environment.NewLine +
                                  "<b>Riseforce Game</b> - https://riseforce.rise.vision" + Environment.NewLine +
                                  "<b>Rise RoadMap</b> - https://rise.vision/roadmap/" + Environment.NewLine +
                                  "<b>Rise Explorer</b> - https://explorer.rise.vision/" + Environment.NewLine +
                                  "<b>Rise GitHub</b> - https://github.com/RiseVision" + Environment.NewLine +
                                  "<b>Rise Web Wallet</b> - https://wallet.rise.vision" + Environment.NewLine +
                                  "<b>Rise Medium</b> - https://medium.com/rise-vision" + Environment.NewLine +
                                  "<b>Rise Twitter</b> - https://twitter.com/RiseVisionTeam" + Environment.NewLine +
                                  "<b>Rise Telegram</b> - https://t.me/risevisionofficial" + Environment.NewLine +
                                  "<b>Rise TG Official Updates</b> - https://t.me/riseupdates" + Environment.NewLine +
                                  "<b>Rise Slack</b> - https://risevision.slack.com/" + Environment.NewLine +
                                  "<b>Rise BitcoinTalk</b> - https://bitcointalk.org/index.php?topic=3211240.200" + Environment.NewLine +
                                  "<b>Rise Intro Youtube</b> - https://www.youtube.com/watch?v=wZ2vIGl_gCM&feature=youtu.be" + Environment.NewLine +
                                  "<b>Rise Telegram Tipping Bot</b> - type !help";
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.LogMessage(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                _appdb.Logger.Add(log);
                _appdb.SaveChangesAsync().Wait();
            }
        }
    }
}