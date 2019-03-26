namespace rise.Code.Cron
{
    using System;

    /// <summary>
    /// Defines the CrontabFieldKind
    /// </summary>
    [Serializable]
    public enum CrontabFieldKind
    {
        /// <summary>
        /// Defines the Minute
        /// </summary>
        Minute,

        /// <summary>
        /// Defines the Hour
        /// </summary>
        Hour,

        /// <summary>
        /// Defines the Day
        /// </summary>
        Day,

        /// <summary>
        /// Defines the Month
        /// </summary>
        Month,

        /// <summary>
        /// Defines the DayOfWeek
        /// </summary>
        DayOfWeek
    }
}