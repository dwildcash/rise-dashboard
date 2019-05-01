# rise-Dashboard

This is the opensource Dashboard dedicated to the Rise coin.

- In order to build you will need ASP.net core 2.2

The source code is available here
- git clone https://github.com/dwildcash/rise-dashboard.git

A working exemple is available here
- https://rise.coinquote.io

# This command will build the dashboard
cd  ./rise-dashboard/rise-dashboard/
dotnet build

When running on Linux you will need to configure Nginx/Kestrel the following URL contain a lot of information on how to setup

https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.0&tabs=aspnetcore2x


#############################################################################
# Important! Appsettings.json
#############################################################################

You will need to create appsettings.json.  Iam using twitter oauth for authentification. Trust me
I changed secret found below please user your own.


--> Appsettings.json for Rise<--

{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },

  "Authentication:Twitter:ConsumerKey": "pKp6NsfdafasJa2IIjzt",
  "Authentication:Twitter:ConsumerSecret": "Q9ccY543523421431241234123lP5wbdoD1iwYE4UY",

  "AppSettings": {
    "CoinName": "RISE",
    "CoinFullName": "RISE",
    "CoinMainSite": "https://rise.vision",
    "CoinExplorer": "https://explorer.rise.vision",
    "APIUrl": "http://localhost:5555",
    "CoinRewardDay": 345,
    "CoinMarketCapTickerCode": "1294",
    "LiveCoinMarket": "RISE/BTC",
    "IPStackApiKey": "02f394132412341234123246f9211277123",
    "DonationAddress": "5556972430134253533R",
    "SimulateRoundCount": "10",
    "MaxTransactionsToFetch": "200",
    "SiteAdminKey": "fjdhajhfdasj542442314123412341234123d"
  }
}


#############################################################################
# Database creation
#############################################################################


How to create the database manually. (The db should create itself when the app start)

Tools –> NuGet Package Manager –> Package Manager Console

Run Add-Migration InitialCreate	to scaffold a migration to create the initial set of tables for your model. If you receive an error stating The term 'add-migration' is not recognized as the name of a cmdlet, close and reopen Visual Studio.

Run Update-Database to apply the new migration to the database. This command creates the database before applying migrations.

Add-Migration AddProductReviews



#############################################################################
# Note
#############################################################################

The success of this project depends largely on the contributions of the community. If you meet me on Telegram or Slack I will be happy to take notes of your suggestions and improvements.

Its always fun to receive contribution! look for dwildcash in explorer or (5953135380169360325R).

Have fun!

@Dwildcash