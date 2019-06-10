namespace rise.Code
{
    using Data;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using rise.Code.DataFetcher;
    using rise.Code.Rise;
    using Scheduling;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="SaveQuoteTask" />
    /// </summary>
    public class UpdateTipAccountStatsTask : IScheduledTask
    {
        /// <summary>
        /// Gets the Schedule
        /// </summary>
        public string Schedule => "*/2 * * * *";

        /// <summary>
        /// Defines the scopeFactory
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var UsersLst = dbContext.ApplicationUsers.ToList<ApplicationUser>();
                    TipAccountStats.UsersCount = UsersLst.Count();

                    // List all account address
                    TipAccountStats.AddressLst = dbContext.ApplicationUsers.Select(x => x.Address).ToList();

                    // Reset the balance
                    double TotalBalance = 0;
                    int TotalTransactions = 0;
                    long TotalAmountTransactions = 0;

                    foreach (var account in UsersLst)
                    {
                        if (account.Address != null)
                        {
                            TotalBalance += await RiseManager.AccountBalanceAsync(account.Address);
                            var tx = TransactionsFetcher.FetchAllUserTransactions(account.Address).Result.transactions.ToList();
                            TotalTransactions += tx.Count();
                            TotalAmountTransactions += tx.Sum(x=>x.amount / 100000000);
                        }
                    }

                    TipAccountStats.TotalBalance = TotalBalance;
                    TipAccountStats.TotalTransactions = TotalTransactions;
                    TipAccountStats.TotalAmountTransactions = TotalAmountTransactions;
                    TipAccountStats.LastGenerated = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.InnerException);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scopeFactory"></param>
        public UpdateTipAccountStatsTask(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
    }
}