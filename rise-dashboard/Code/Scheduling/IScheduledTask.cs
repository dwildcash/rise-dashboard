namespace rise.Code.Scheduling
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IScheduledTask" />
    /// </summary>
    public interface IScheduledTask
    {
        /// <summary>
        /// Gets the Schedule
        /// </summary>
        string Schedule { get; }

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}