using Microsoft.Extensions.Logging;
using rise.Helpers;
using rise.Models;
using rise.Services;
using System;
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
        private ApplicationUser appuser;

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
            appuser = await _appUsersManagerService.GetUserAsync(message.From.Username, message.From.Id);

            string command = string.Empty;

            // Match Command
            if (Regex.Matches(message.Text, @"!(\S+)\s?").Count > 0)
            {
                command = Regex.Matches(message.Text.ToUpper(), @"!(\S+)\s?")[0].ToString().Trim();
            }

            // Info command
            if (command == "!INFO")
            {
                await cmd_Info(message);
            }

            // Info Price
            if (command == "!PRICE")
            {
                await cmd_Price(message.Chat.Id);
            }

            // Return a  geek joke
            if (command == "!JOKE")
            {
                await cmd_Joke(message.Chat.Id);
            }

            // Show Rise Exchanges
            if (command == "!EXCHANGES")
            {
                await cmd_Exchanges(message);
            }

            // show Deposit
            if (command == "!DEPOSIT")
            {
                await cmd_Deposit(message);
            }
        }

        /// <summary>
        /// Display Current Rise Exchanges
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Deposit(Message message)
        {
            string strResponse = string.Empty;

            if (message.Chat.Id == AppSettingsProvider.TelegramChannelId)
            {
                if (appuser == null)
                {
                    strResponse = "Please use !deposit command only in private message";
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
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
                _logger.LogError("Received Exception from cmd_deposit {0}", ex.Message);
            }
        }

        /// <summary>
        /// Display a chuck Norris Joke
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Joke(long chatId)
        {
            await _botService.Client.SendChatActionAsync(chatId, ChatAction.Typing);
            var strResponse = await QuoteOfTheDayManager.GetQuoteOfTheDay();

            if (strResponse != null)
            {
                await _botService.Client.SendTextMessageAsync(chatId, strResponse, ParseMode.Html);
            }
        }

        /// <summary>
        /// Display Current Rise Exchanges
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Exchanges(Message message)
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
                _logger.LogError("Received Exception from cmd_exchanges {0}", ex.Message);
            }
        }

        /// <summary>
        /// Display Current Price
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Price(long chatId)
        {
            await _botService.Client.SendChatActionAsync(chatId, ChatAction.Typing);
            var quote = await QuoteManager.GetRiseQuote();

            string strResponse = "Price (sat): <b>" + Math.Round(quote.Price * 100000000) + "</b>" + Environment.NewLine +
                "Usd Price: <b>$" + Math.Round(quote.USDPrice, 4) + "</b>" + Environment.NewLine +
                "Volume: <b>" + Math.Round(quote.Volume).ToString("N0") + "</b>";
            await _botService.Client.SendTextMessageAsync(chatId, strResponse, ParseMode.Html);
            var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Rise.coinquote.io", "https://rise.coinquote.io"));
            await _botService.Client.SendTextMessageAsync(chatId, "Open website", replyMarkup: keyboard);
        }


        /// <summary>
        /// Send Info
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task cmd_Info(Message message)
        {
            string strResponse = string.Empty;

            if (message.Chat.Id == AppSettingsProvider.TelegramChannelId)
            {
                if (appuser == null)
                {
                    strResponse = "Please use !Info command only in private message";
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, strResponse, ParseMode.Html);
                    return;
                }
            }

            try
            {
                strResponse = "<b>Rise Information/Tools</b>" + Environment.NewLine +
                "<b>Rise Website</b> - https://rise.vision" + Environment.NewLine +
                "<b>Rise Explorer</b> - https://explorer.rise.vision/" + Environment.NewLine +
                "<b>Rise GitHub</b> - https://github.com/RiseVision" + Environment.NewLine +
                "<b>Rise Web Wallet</b> - https://wallet.rise.vision" + Environment.NewLine +
                "<b>Rise Medium</b> - https://medium.com/rise-vision" + Environment.NewLine +
                "<b>Rise Force Game</b> - http://duhec.net/riseForce" + Environment.NewLine +
                "<b>Rise Dashboard</b> - https://rise.coinquote.io" + Environment.NewLine +
                "<b>Rise Twitter</b> - https://twitter.com/RiseVisionTeam" + Environment.NewLine +
                "<b>Rise Telegram</b> - https://t.me/risevisionofficial" + Environment.NewLine +
                "<b>Rise Slack</b> - https://risevision.slack.com/" + Environment.NewLine +
                "<b>Rise Discord</b> - https://discord.gg/6jyWQnJ" + Environment.NewLine +
                "<b>Rise BitcoinTalk</b> - https://bitcointalk.org/index.php?topic=3211240.200" + Environment.NewLine +
                "<b>Rise Intro Youtube</b> - https://www.youtube.com/watch?v=wZ2vIGl_gCM&feature=youtu.be" + Environment.NewLine +
                "<b>Rise Telegram Tipping service</b> -!help";
                await _botService.Client.SendTextMessageAsync(appuser.TelegramId, strResponse, ParseMode.Html);
            }
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from cmd_Info {0}", ex.Message);
            }
        }
    }
}