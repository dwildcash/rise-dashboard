# rise-Dashboard

This is the opensource version for my rise Dashboard

- Install Visual Studio 2017 (Iam using community edition),  it will compile on linux easily
- git clone https://github.com/dwildcash/rise-dashboard.git

# This command will build the dashboard
cd  ./rise-dashboard/rise-dashboard/
dotnet build

For running on Linux configure Nginx/Kestrel the following URL contain a lot of information on how to setup

https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.0&tabs=aspnetcore2x


######################################
# Important!
######################################

You will need to create appsettings.json with your discord app info. This is only an example, this config will not work.

ex from my appsettings.json

{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },

  "Authentication:Discord:ClientId": "343264873341441281",
  "Authentication:Discord:ClientSecret": "WDTMKsHs3ND1YfgiYb1j7Ia79KWJBhMI",

  "AppSettings": {
    "Example": "rise"
  }
}


Have fun!

@dwildcash