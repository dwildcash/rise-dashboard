using rise.Code.DataFetcher;
using rise.Code.Rise;
using rise.Code.Scheduling;
using rise.Models;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Code.Tasks
{
    /// <summary>
    /// Defines the <see cref="Update15MinTasks" />
    /// </summary>
    public class Update15MinTasks : IScheduledTask
    {
        /// <summary>
        /// Gets the Schedule
        /// </summary>
        public string Schedule => "*/15 * * * *";

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var delegateResult = await DelegateFetcher.FetchDelegates();

            foreach (var mdelegate in delegateResult.Delegates)
            {
                mdelegate.Voters = DelegateVotersFetcher.FetchVoters(mdelegate.PublicKey).Result.Accounts.Count;
            }

            if (delegateResult.Success)
            {
                DelegateResult.Current = ForgingChanceCalculator.SimulateForgingRounds(delegateResult);
            }      
        }
    }
}