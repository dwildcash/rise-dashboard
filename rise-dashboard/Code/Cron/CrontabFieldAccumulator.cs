namespace rise.Code.Cron
{
    /// <summary>
    /// The CrontabFieldAccumulator
    /// </summary>
    /// <param name="start">The start<see cref="int"/></param>
    /// <param name="end">The end<see cref="int"/></param>
    /// <param name="interval">The interval<see cref="int"/></param>
    public delegate void CrontabFieldAccumulator(int start, int end, int interval);
}