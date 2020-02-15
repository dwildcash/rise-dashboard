# rise-Dashboard

This is the opensource Dashboard dedicated to the Rise coin.

In order to build you will need ASP.net core 3.1
You can learn how to install dotnet core on linux here:
- https://docs.microsoft.com/en-us/dotnet/core/install/

The source code is available here
- git clone https://github.com/dwildcash/rise-dashboard.git

A working exemple is available here
- https://rise.coinquote.io


When running on Linux you will need to configure a Nginx reverse proxy for Kestrel.

The following URL contain a lot of information on how you can setup

- https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.0&tabs=aspnetcore2x


#############################################################################
# Important! appsettings.json
#############################################################################

You will need to create the appsettings.json file (example below) in ./rise-dashboard/rise-dashboard project folder.  
Iam using a telegram bot for authentification part. 

Trust me all secrets below wont work... found below please use your own.


--> Appsettings.json for Rise<--

{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },

  "AppSettings": {
    "SiteUrl": "https://dashboard.rise.vision",
    "BotName": "botname",
    "CoinName": "RISE",
    "CoinFullName": "RISE",
    "CoinMainSite": "https://rise.vision",
    "CoinExplorer": "https://explorer.rise.vision",
    "APIUrl": "http://localhost:5555",
    "CoinRewardDay": 345,
    "CoinMarketCapTickerCode": "1294",
    "LiveCoinMarket": "RISE/BTC",
    "VinexMarket": "BTC_RISE",
    "LiveCoinWalletAddress": "5920507067941756798R",
    "VinexWalletAddress": "7705924848357154463R",
    "AltillyWalletAddress": "10360579734214425763R",
    "BitkerWalletAddress": "576909973426919893R",
    "AltillyMarket": "RISEBTC",
    "IPStackApiKey": "putapikey",
    "DonationAddress": "5556972430134253533R",
    "SimulateRoundCount": "100",
    "MaxTransactionsToFetch": "200",
    "BotApiKey": "put_your_botapikey",
    "EncryptionKey": "put_your_encryptkey",
    "TelegramChannelId": "-1001377697093",
    "Salt": "salt",
    "NodeLogFile": "c:\\source\\test.txt",
    "WebHookSecret": "put_your_webhooksecret",
  }
}


#############################################################################
# Database creation
#############################################################################

To create the intial database go to the rise-dashboard project folder and type

dotnet ef database update


#############################################################################
# Note
#############################################################################

The success of this project depends largely on the contributions of the community. 

If you meet me on Telegram or Slack I will be happy to take notes of your suggestions and improvements.

Its always fun to receive contribution! look for dwildcash in explorer or (5953135380169360325R).

Have fun!

@Dwildcash