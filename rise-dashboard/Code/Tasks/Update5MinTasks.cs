using rise.Code.DataFetcher;
using rise.Code.Rise;
using rise.Code.Scheduling;
using rise.Models;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Code.Tasks
{
    /// <summary>
    /// Defines the <see cref="Update5MinTasks" />
    /// </summary>
    public class Update5MinTasks : IScheduledTask
    {
        /// <summary>
        /// Gets the Schedule
        /// </summary>
        public string Schedule => "*/5 * * * *";

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var delegateResult = await DelegateFetcher.FetchDelegates();

            if (delegateResult.Success)
            {
                DelegateResult.Current = ForgingChanceCalculator.SimulateForgingRounds(delegateResult);

                foreach (var mdelegate in delegateResult.Delegates)
                {
                    mdelegate.Voters = DelegateVotersFetcher.FetchVoters(mdelegate.PublicKey).Result.Accounts.Count;
                }
            }
        }
    }
}