namespace rise.Code.Scheduling
{
    using rise.Code.Cron;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="SchedulerHostedService" />
    /// </summary>
    public class SchedulerHostedService : HostedService
    {
        /// <summary>
        /// Defines the UnobservedTaskException
        /// </summary>
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        /// <summary>
        /// Defines the _scheduledTasks
        /// </summary>
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerHostedService"/> class.
        /// </summary>
        /// <param name="scheduledTasks">The scheduledTasks<see cref="IEnumerable{IScheduledTask}"/></param>
        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            var referenceTime = DateTime.UtcNow;

            foreach (var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = referenceTime
                });
            }
        }

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        /// <summary>
        /// The ExecuteOnceAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        /// <summary>
        /// Defines the <see cref="SchedulerTaskWrapper" />
        /// </summary>
        private class SchedulerTaskWrapper
        {
            /// <summary>
            /// Gets or sets the Schedule
            /// </summary>
            public CrontabSchedule Schedule { get; set; }

            /// <summary>
            /// Gets or sets the Task
            /// </summary>
            public IScheduledTask Task { get; set; }

            /// <summary>
            /// Gets or sets the LastRunTime
            /// </summary>
            public DateTime LastRunTime { get; set; }

            /// <summary>
            /// Gets or sets the NextRunTime
            /// </summary>
            public DateTime NextRunTime { get; set; }

            /// <summary>
            /// The Increment
            /// </summary>
            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
            }

            /// <summary>
            /// The ShouldRun
            /// </summary>
            /// <param name="currentTime">The currentTime<see cref="DateTime"/></param>
            /// <returns>The <see cref="bool"/></returns>
            public bool ShouldRun(DateTime currentTime)
            {
                return NextRunTime < currentTime && LastRunTime != NextRunTime;
            }
        }
    }
}