namespace rise
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using rise.Code;
    using rise.Code.Scheduling;
    using rise.Data;
    using rise.Models;
    using rise.Services;
    using Rise.Services;
    using System;

    /// <summary>
    /// Defines the <see cref="Startup" />
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The env<see cref="IHostingEnvironment"/></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Build AppSettings
            BuildAppSettingsProvider();
        }

        /// <summary>
        /// Build a static App Settings based on appsettings.json
        /// </summary>
        private void BuildAppSettingsProvider()
        {
            AppSettingsProvider.APIUrl = Configuration["AppSettings:APIUrl"];
            AppSettingsProvider.SiteUrl = Configuration["AppSettings:SiteUrl"];
            AppSettingsProvider.IPStackApiKey = Configuration["AppSettings:IPStackApiKey"];
            AppSettingsProvider.LiveCoinMarket = Configuration["AppSettings:LiveCoinMarket"];
            AppSettingsProvider.CoinMarketCapTickerCode = Configuration["AppSettings:CoinMarketCapTickerCode"];
            AppSettingsProvider.CoinName = Configuration["AppSettings:CoinName"];
            AppSettingsProvider.CoinRewardDay = int.Parse(Configuration["AppSettings:CoinRewardDay"]);
            AppSettingsProvider.CoinFullName = Configuration["Appsettings:CoinFullName"];
            AppSettingsProvider.CoinMainSite = Configuration["AppSettings:CoinMainSite"];
            AppSettingsProvider.CoinExplorer = Configuration["AppSettings:CoinExplorer"];
            AppSettingsProvider.DonationAddress = Configuration["AppSettings:DonationAddress"];
            AppSettingsProvider.RightBtcMarket = Configuration["AppSettings:RightBtcMarket"];
            AppSettingsProvider.LiveCoinWalletAddress = Configuration["Appsettings:LiveCoinWalletAddress"];
            AppSettingsProvider.SimulateRoundCount = int.Parse(Configuration["AppSettings:SimulateRoundCount"]);
            AppSettingsProvider.MaxTransactionsToFetch = int.Parse(Configuration["AppSettings:MaxTransactionsToFetch"]);
            AppSettingsProvider.BotApiKey = Configuration["AppSettings:BotApiKey"];
            AppSettingsProvider.BotName = Configuration["AppSettings:BotName"];
            AppSettingsProvider.Salt = Configuration["AppSettings:Salt"];
            AppSettingsProvider.EncryptionKey = Configuration["AppSettings:EncryptionKey"];
            AppSettingsProvider.WebHookSecret = Configuration["AppSettings:WebHookSecret"];
            AppSettingsProvider.TelegramChannelId = long.Parse(Configuration["AppSettings:TelegramChannelId"]);
        }

        /// <summary>
        /// Gets or sets the Configuration
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// The ConfigureServices
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Service for slack auth
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            // DB for aspnet
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=apps.db"));

            // Assign Identity User and Context for Auth by ASP
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // Will need to investigate token expiration
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromDays(7));

            // Force Anti Forgery token Validation
            services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            // Configure 1Min Update Tasks
            services.AddSingleton<IScheduledTask, Update2MinTasks>();

            // Configure 30Min Update Task
            services.AddSingleton<IScheduledTask, Update15MinTasks>();

            // Configure Quote of the Day Tasks
            services.AddSingleton<IScheduledTask, UpdateQuoteOfTheDayTask>();

            // Update IP localisation once a day
            services.AddSingleton<IScheduledTask, UpdateIpLocalisationTask>();

            // Save Livecoin quote price every minutes.
            services.AddSingleton<IScheduledTask, SaveQuoteTask>();

            // Configure Telegram bot
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddSingleton<IBotService, BotService>();

            // Users Management Service
            services.AddSingleton<IAppUsersManagerService, AppUsersManagerService>();

            // Config Start Scheduler
            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });
        }

        /// <summary>
        /// The Configure
        /// </summary>
        /// <param name="app">The app<see cref="IApplicationBuilder"/></param>
        /// <param name="env">The env<see cref="IHostingEnvironment"/></param>
        /// <param name="userManager">The userManager<see cref="UserManager{ApplicationUser}"/></param>
        /// <param name="roleManager">The roleManager<see cref="RoleManager{ApplicationRole}"/></param>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.EnsureCreated();
                }
            }
            catch (Exception e)
            {
                Console.Write(e.InnerException);
            }

            // Seed Initial User
            DbSeedData.SeedData(userManager, roleManager, context);

            app.UseStaticFiles();

            // Use Authentication
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Use Forwatded Header to keep track of client info.
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }
    }
}