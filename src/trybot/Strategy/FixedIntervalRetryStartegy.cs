using System;

namespace Trybot.Strategy
{
    /// <summary>
    /// Represents a retry strategy implementation with fixed intervals.
    /// </summary>
    public class FixedIntervalRetryStartegy : RetryStartegy
    {
        /// <summary>
        /// Constructs a <see cref="FixedIntervalRetryStartegy"/>
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="delay">The initial delay.</param>
        public FixedIntervalRetryStartegy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
        }

        /// <summary>
        /// Calculates the next delay value.
        /// </summary>
        /// <param name="currentAttempt">The current attempt.</param>
        /// <returns>Always the initial delay value.</returns>
        protected override TimeSpan GetNextDelay(int currentAttempt) => base.Delay;
    }
}
