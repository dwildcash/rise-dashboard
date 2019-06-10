# rise-Dashboard

This is the opensource Dashboard dedicated to the Rise coin.

- In order to build you will need ASP.net core 2.2

The source code is available here
- git clone https://github.com/dwildcash/rise-dashboard.git

A working exemple is available here
- https://rise.coinquote.io


When running on Linux you will need to configure Nginx/Kestrel the following URL contain a lot of information on how to setup

https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.0&tabs=aspnetcore2x


#############################################################################
# Important! Appsettings.json
#############################################################################

You will need to create the appsettings.json file in the project folder.  
Iam using a telegram bot for authentification part. 

Trust me all secret below wont work... found below please use your own.


--> Appsettings.json for Rise<--

{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },

  "AppSettings": {
    "SiteUrl": "https://rise.coinquote.io",
    "BotName": "RiseStar_bot",
    "CoinName": "RISE",
    "CoinFullName": "RISE",
    "CoinMainSite": "https://rise.vision",
    "CoinExplorer": "https://explorer.rise.vision",
    "APIUrl": "http://localhost:5555",
    "CoinRewardDay": 345,
    "CoinMarketCapTickerCode": "1294",
    "LiveCoinMarket": "RISE/BTC",
    "LiveCoinWalletAddress": "5920507067941756798R",
    "VinexWalletAddress": "7705924848357154463R",
    "AltillyMarket": "RISEBTC",
    "VinexMarket": "BTC_RISE",
    "IPStackApiKey": "63456345634563456345",
    "DonationAddress": "5953135380169360325R",
    "SimulateRoundCount": "100",
    "MaxTransactionsToFetch": "1000",
    "BotApiKey": "64534564536345634564356345",
    "EncryptionKey": "gfdsgfsdgsdfgsdfgsd",
    "TelegramChannelId": "-1001377697093",
    "Salt": "gsdfgsdfgfsd",
    "WebHookSecret":  "7346sdafsad432311"
  }
}


#############################################################################
# Database creation
#############################################################################

To create the database go to the rise-dashboard project folder and type

dotnet ef database update


#############################################################################
# Note
#############################################################################

The success of this project depends largely on the contributions of the community. 

If you meet me on Telegram or Slack I will be happy to take notes of your suggestions and improvements.

Its always fun to receive contribution! look for dwildcash in explorer or (5953135380169360325R).

Have fun!

@Dwildcash