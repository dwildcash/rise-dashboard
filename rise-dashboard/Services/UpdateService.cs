using Microsoft.Extensions.Logging;
using rise.Helpers;
using rise.Models;
using rise.Services;
using rise_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            // Get the user who sent message
            var appuser = await _appUsersManagerService.GetUserAsync(message.From.Username, message.From.Id);

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

            // Withdraw Rice
            if (command == "!SEND")
            {
                var strResponse = string.Empty;
                double amount = 0;
                List<ApplicationUser> ListDestuser = new List<ApplicationUser>();

                try
                {
                    amount = double.Parse(Regex.Matches(message.Text, @"[-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?").Cast<Match>().Select(m => m.Value).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    strResponse = "Illegal amount entered. ex: !withdraw 10 RISE to 5953135380169360325R";

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }

                var recipientUser = _appUsersManagerService.GetUserByUsername(destUser);
                ListDestuser.Add(recipientUser);

                List<rise_lib.Models.Transaction> tx = await cmd_SendTx(appuser, ListDestuser, amount);

                if (tx[0].success)
                {
                    strResponse = "@" + recipientUser.UserName + " wake up, it's a wonderful day!! thanks to @" + appuser.UserName + " he sent <b>" + amount + " RISE</b> to you :)";

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("See Transaction", "https://explorer.rise.vision/tx/" + tx[0].transactionId));
                    await _botService.Client.SendTextMessageAsync(message.Chat, "Transaction Id:" + tx[0].transactionId + "", replyMarkup: keyboard);
                }
                else
                {
                    strResponse = "Ohhh sorry " + appuser.UserName + " I got a problem while processing transaction.";
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                }       
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

            // Show Rise Exchanges
            if (command == "!EXCHANGES")
            {
                await cmd_Exchanges(message, appuser);
            }

            // show Deposit
            if (command == "!DEPOSIT")
            {
                await cmd_Deposit(message, appuser);
            }
        }

        /// <summary>
        /// Display Current Rise Exchanges
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Deposit(Message message, ApplicationUser appuser)
        {
            string strResponse = string.Empty;

            if (message.Chat.Id == AppSettingsProvider.TelegramChannelId)
            {
                if (appuser == null)
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please use !deposit command only in private message", ParseMode.Html);
                    return;
                }
            }

            try
            {
                await _botService.Client.SendChatActionAsync(appuser.TelegramId, ChatAction.Typing);

                strResponse = "Here we go @" + appuser.UserName + " this is your address <b>" + appuser.Address + "</b>";

                if (string.IsNullOrEmpty(appuser.UserName))
                {
                    strResponse += Environment.NewLine + " Note: Please configure your Telegram UserName if you want to Receive <b>Rise</b>";
                }

                await _botService.Client.SendTextMessageAsync(appuser.TelegramId, strResponse, ParseMode.Html);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Deposit {0}", ex.Message);
            }
        }

        /// <summary>
        /// Withdraw Rise to account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private async Task cmd_WithdrawTx(ApplicationUser sender, string address, double amount)
        {
            string strResponse = string.Empty;

            if (await cmd_BalanceCheck(sender, 1, amount))
            {
                try
                {
                    await _botService.Client.SendChatActionAsync(sender.TelegramId, ChatAction.Typing);
                   
                    var e = sender.GetSecret();
                    var tx = await RiseManager.CreatePaiment(amount * 100000000, e, address);

                    strResponse = "Ok @" + sender.UserName + " I sent " + amount + "RISE to this address <b>" + address + "</b>";

                    await _botService.Client.SendTextMessageAsync(sender.TelegramId, strResponse, ParseMode.Html);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Received Exception from cmd_WithdrawTx {0}" + ex.Message);
                }
            }
            else
            {
                await _botService.Client.SendTextMessageAsync(sender.TelegramId, "Sorry @" + sender.UserName + " you dont have enough RISE to send <b>" + amount + "</b> to address " + address, ParseMode.Html);
            }
        }

        /// <summary>
        /// Show Help
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task cmd_Help(Message message, ApplicationUser appuser)
        {
            string strResponse = string.Empty;

            if (message.Chat.Id == AppSettingsProvider.TelegramChannelId)
            {
                if (appuser == null)
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Please use !help command only in private message", ParseMode.Html);
                    return;
                }
            }

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
                        "<b>!deposit</b> - Create a Deposit RISE Address" + Environment.NewLine +
                        "<b>!balance</b> - Show current RISE Balance" + Environment.NewLine +
                        "<b>!joke</b> - Display a geek joke" + Environment.NewLine +
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
        /// Create Transaction
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Receiver"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private async Task<List<rise_lib.Models.Transaction>> cmd_SendTx(ApplicationUser Sender, List<ApplicationUser> ListReceiver, double amount)
        {
            List<rise_lib.Models.Transaction> txList = new List<rise_lib.Models.Transaction>();

            if (await cmd_BalanceCheck(Sender, ListReceiver.Count, amount))
            {
                try
                {
                    foreach (var destUser in ListReceiver)
                    {
                        if (destUser.Address == null || destUser.Address[destUser.Address.Length - 1] != 'R')
                        {
                            continue;
                        }

                        var tx = await RiseManager.CreatePaiment(amount * 100000000, Sender.GetSecret(), destUser.Address);
                        txList.Add(tx);
                    }
                }
                catch (Exception ex)
                {
                    var errTx = new rise_lib.Models.Transaction
                    {
                        success = false,
                        transactionId = null
                    };

                    txList.Add(errTx);

                    _logger.LogError("Received Exception from cmd_SendTx {0}" + ex.Message);
                    return txList;
                }
            }
            else
            {
                var errTx = new rise_lib.Models.Transaction
                {
                    success = false,
                    transactionId = null
                };

                txList.Add(errTx);

                await _botService.Client.SendTextMessageAsync(Sender.TelegramId, "Sorry you dont have enough RISE to send <b>" + amount + "</b> to " + ListReceiver.Count + " users", ParseMode.Html);
            }

            return txList;
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
                _logger.LogError("Received Exception from cmd_Deposit {0}", ex.Message);
            }
        }

        /// <summary>
        /// Verify if Balance is OK
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="NumTx"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private async Task<bool> cmd_BalanceCheck(ApplicationUser Sender, int NumTx, double amount)
        {
            try
            {
                var balance = await RiseManager.AccountBalanceAsync(Sender.Address);

                if (balance > (0.1 * NumTx) + amount)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from BalanceCheck {0} " + ex.Message);
                return false;
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
            string strResponse = string.Empty; ;

            if (message.Chat.Id == AppSettingsProvider.TelegramChannelId)
            {
                if (appuser == null)
                {
                    strResponse = "Please use !exchanges command only in private message";
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }
            }

            try
            {
                await _botService.Client.SendChatActionAsync(appuser.TelegramId, ChatAction.Typing);
                strResponse = "<b>-= Current Rise Exchanges =-</b>" + Environment.NewLine +
                "<b>Livecoin</b> - http://livecoin.net" + Environment.NewLine +
                "<b>RightBtc</b> - http://rightbtc.com" + Environment.NewLine +
                "<b>Vinex</b> - https://vinex.network";
                await _botService.Client.SendTextMessageAsync(appuser.TelegramId, strResponse, ParseMode.Html);
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