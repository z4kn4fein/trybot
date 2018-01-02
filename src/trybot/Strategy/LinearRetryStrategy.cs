using System;

namespace Trybot.Strategy
{
    /// <summary>
    /// Represents a retry strategy implementation with linear delay calculation.
    /// </summary>
    public class LinearRetryStrategy : RetryStartegy
    {
        /// <summary>
        /// Constructs a <see cref="LinearRetryStrategy"/>
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="delay">The initial delay.</param>
        public LinearRetryStrategy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
        }

        /// <summary>
        /// Calculates the next delay value.
        /// </summary>
        /// <param name="currentAttempt">The current attempt.</param>
        /// <returns>The inital delay multiplied by the current attempt.</returns>
        protected override TimeSpan GetNextDelay(int currentAttempt) => TimeSpan.FromMilliseconds(currentAttempt * base.Delay.TotalMilliseconds);
    }
}
