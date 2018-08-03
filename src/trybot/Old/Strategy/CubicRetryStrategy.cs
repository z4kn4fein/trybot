using System;

namespace Trybot.Strategy
{
    /// <summary>
    /// Represents a retry strategy with basic cubic delay calculation.
    /// </summary>
    public class CubicRetryStrategy : RetryStartegy
    {

        /// <summary>
        /// Constructs a <see cref="CubicRetryStrategy"/>
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="delay">The initial delay.</param>
        public CubicRetryStrategy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
        }

        /// <summary>
        /// Calculates the next delay value.
        /// </summary>
        /// <param name="currentAttempt">The current attempt.</param>
        /// <returns>The basic cubic calculation of the initial delay and the current attempt.</returns>
        protected override TimeSpan GetNextDelay(int currentAttempt)
        {
            var tmpDelay = currentAttempt * base.Delay.TotalMilliseconds;
            return TimeSpan.FromMilliseconds(tmpDelay * tmpDelay * tmpDelay);
        }
    }
}
