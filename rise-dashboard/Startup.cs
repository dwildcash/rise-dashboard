using rise.Code.Tasks;

namespace rise
{
    using Code;
    using Code.Scheduling;
    using Data;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Models;
    using Rise.Services;
    using Services;
    using System;

    /// <summary>
    /// Defines the <see cref="Startup" />
    /// </summary>
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private static int debugMode = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The env<see cref="IHostingEnvironment"/></param>
        public Startup(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Build AppSettings
            BuildAppSettingsProvider();

            _logger = logger;
        }

        /// <summary>
        /// Build a static App Settings based on appsettings.json
        /// </summary>
        private void BuildAppSettingsProvider()
        {
            AppSettingsProvider.APIUrl = Configuration["AppSettings:APIUrl"];
            AppSettingsProvider.SiteUrl = Configuration["AppSettings:SiteUrl"];
            AppSettingsProvider.IPStackApiKey = Configuration["AppSettings:IPStackApiKey"];
            AppSettingsProvider.AltillyMarket = Configuration["AppSettings:AltillyMarket"];
            AppSettingsProvider.LiveCoinMarket = Configuration["AppSettings:LiveCoinMarket"];
            AppSettingsProvider.VinexMarket = Configuration["AppSettings:VinexMarket"];
            AppSettingsProvider.CoinMarketCapTickerCode = Configuration["AppSettings:CoinMarketCapTickerCode"];
            AppSettingsProvider.CoinName = Configuration["AppSettings:CoinName"];
            AppSettingsProvider.CoinRewardDay = int.Parse(Configuration["AppSettings:CoinRewardDay"]);
            AppSettingsProvider.CoinFullName = Configuration["AppSettings:CoinFullName"];
            AppSettingsProvider.CoinMainSite = Configuration["AppSettings:CoinMainSite"];
            AppSettingsProvider.CoinExplorer = Configuration["AppSettings:CoinExplorer"];
            AppSettingsProvider.DonationAddress = Configuration["AppSettings:DonationAddress"];
            AppSettingsProvider.LiveCoinWalletAddress = Configuration["AppSettings:LiveCoinWalletAddress"];
            AppSettingsProvider.AltillyWalletAddress = Configuration["AppSettings:AltillyWalletAddress"];
            AppSettingsProvider.VinexWalletAddress = Configuration["AppSettings:VinexWalletAddress"];
            AppSettingsProvider.SimulateRoundCount = int.Parse(Configuration["AppSettings:SimulateRoundCount"]);
            AppSettingsProvider.MaxTransactionsToFetch = int.Parse(Configuration["AppSettings:MaxTransactionsToFetch"]);
            AppSettingsProvider.BotApiKey = Configuration["AppSettings:BotApiKey"];
            AppSettingsProvider.BotName = Configuration["AppSettings:BotName"];
            AppSettingsProvider.Salt = Configuration["AppSettings:Salt"];
            AppSettingsProvider.EncryptionKey = Configuration["AppSettings:EncryptionKey"];
            AppSettingsProvider.WebHookSecret = Configuration["AppSettings:WebHookSecret"];
            AppSettingsProvider.TelegramChannelId = long.Parse(Configuration["AppSettings:TelegramChannelId"]);
            AppSettingsProvider.NodeLogFile = Configuration["AppSettings:NodeLogFile"];
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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            // DB for aspnet
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=apps.db"));

            // Users Management Service
            services.AddScoped<IAppUsersManagerService, AppUsersManagerService>();

            // Assign Identity User and Context for Auth by ASP
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // Will need to investigate token expiration
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromDays(7));

            // Force Anti Forgery token Validation
            services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // Support For Razor Pages
            services.AddRazorPages();

            // Configure BotService
            services.AddSingleton<IBotService, BotService>();

            // Add signalr service
            services.AddSignalR();

            // Start tail log service
            //services.AddHostedService<TailLogService>();

            // Configure 1Min Update Tasks
            services.AddSingleton<IScheduledTask, Update2MinTasks>();

            // Configure 30Min Update Task
            services.AddSingleton<IScheduledTask, Update5MinTasks>();

            // Configure Quote of the Day Tasks
            services.AddSingleton<IScheduledTask, UpdateQuoteOfTheDayTask>();

            // Update IP localisation once a day
            services.AddSingleton<IScheduledTask, UpdateIpLocalisationTask>();

            // Save quote price every minutes.
            services.AddSingleton<IScheduledTask, SaveQuoteTask>();

            if (debugMode != 1)
            {
                // Update Tip Account Task
                services.AddSingleton<IScheduledTask, UpdateTipAccountStatsTask>();
            }

            // Configure Telegram bot and bot response
            services.AddScoped<IUpdateService, UpdateService>();

 
            // Config Start Scheduler
            services.AddScheduler((sender, args) =>
            {
                _logger.LogError("Received Exception from Startup Scheduler{0}", args.Exception.Message);
                args.SetObserved();
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        /// <summary>
        /// The Configure
        /// </summary>
        /// <param name="app">The app<see cref="IApplicationBuilder"/></param>
        /// <param name="env">The env<see cref="IHostingEnvironment"/></param>
        /// <param name="userManager">The userManager<see cref="UserManager{ApplicationUser}"/></param>
        /// <param name="roleManager">The roleManager<see cref="RoleManager{ApplicationRole}"/></param>
        /// <param name="context">The context<see cref="ApplicationDbContext"/></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                debugMode = 1;
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
            catch (Exception ex)
            {
                _logger.LogError("Received Exception from Configure {0}", ex.Message);
            }

            // Seed Initial User
            DbSeedData.SeedData(userManager, roleManager, context).Wait();

            // Use Forwarded Header to keep track of client info.
            app.UseForwardedHeaders();

            app.UseStaticFiles();

            // Use Authentication
            app.UseAuthentication();

            //app.UseSignalR(routes =>
            // {
            //     routes.MapHub<NotificationHub>("/Hub/notificationHub");
            //});



            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Map attribute-routed API controllers
                endpoints.MapDefaultControllerRoute(); // Map conventional MVC controllers using the default route
                endpoints.MapRazorPages();
            });      
        }
    }
}