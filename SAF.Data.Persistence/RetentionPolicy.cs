namespace SAF.Data.Persistence
{
    using System;

    using SAF.Data;

    /// <summary>
    /// Represents a retention policy of a specific length and granularity in units of time.
    /// </summary>
    public class RetentionPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetentionPolicy"/> class with the specified length and 
        /// granularity in time units.
        /// </summary>
        /// <param name="length">The length of time to retain items.</param>
        /// <param name="granularity">The granularity of the policy in time units.</param>
        public RetentionPolicy(TimeSpan length, TimeUnit granularity)
        {
            this.Length = length;
            this.Granularity = granularity;
        }

        /// <summary>
        /// Gets or sets the retention period to apply.
        /// </summary>
        public TimeSpan Length { get; set; }

        /// <summary>
        /// Gets or sets the granularity of the retention policy. Items will be retained
        /// at least one unit of the specified granularity.
        /// </summary>
        public TimeUnit Granularity { get; set; }

        /// <summary>
        /// Retrieves the time at which the retention period becomes effective.
        /// </summary>
        /// <param name="policyTime">The time that the policy is to be enacted.</param>
        /// <param name="period">The period of time that the retention policy is valid for.</param>
        /// <param name="granularity">The granularity to apply to the resulting point in time.</param>
        /// <returns>The time at which the retention period becomes effective in relation to the specified policy
        /// time, with the applied granularity.</returns>
        public static DateTime GetRetentionCutoff(DateTime policyTime, TimeSpan period, TimeUnit granularity)
        {
            DateTime retentionCutoff = policyTime - period;

            switch (granularity)
            {
                case TimeUnit.Years:
                    return new DateTime(retentionCutoff.Year, 1, 1);

                case TimeUnit.Months:
                    return new DateTime(retentionCutoff.Year, retentionCutoff.Month, 1);

                case TimeUnit.Weeks:
                    return new DateTime(
                        retentionCutoff.Year,
                        retentionCutoff.Month,
                        retentionCutoff.Day)
                            .Subtract(TimeSpan.FromDays((int)DateTime.Now.DayOfWeek));

                case TimeUnit.Days:
                    return retentionCutoff.Date;

                case TimeUnit.Hours:
                    return retentionCutoff.Date.Add(
                        new TimeSpan(retentionCutoff.Hour, 0, 0));

                case TimeUnit.Minutes:
                    return retentionCutoff.Date.Add(
                        new TimeSpan(retentionCutoff.Hour, retentionCutoff.Minute, 1));

                case TimeUnit.Seconds:
                    return retentionCutoff.Date.Add(
                        new TimeSpan(retentionCutoff.Hour, retentionCutoff.Minute, retentionCutoff.Second));

                case TimeUnit.Milliseconds:
                    return retentionCutoff.Date.Add(
                        new TimeSpan(
                            retentionCutoff.Hour,
                            retentionCutoff.Minute,
                            retentionCutoff.Second,
                            retentionCutoff.Millisecond));

                case TimeUnit.Ticks:
                    return retentionCutoff;
            }

            return retentionCutoff;
        }
    }
}
