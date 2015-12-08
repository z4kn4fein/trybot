using System;

namespace Trybot.Strategy
{
    /// <summary>
    /// Represents a retry strategy implementation with squares delay calculation.
    /// </summary>
    public class SquareRetryStartegy : RetryStartegy
    {
        /// <summary>
        /// Constructs a <see cref="SquareRetryStartegy"/>
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="delay">The initial delay.</param>
        public SquareRetryStartegy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
        }

        /// <summary>
        /// Calculates the next delay value.
        /// </summary>
        /// <param name="currentAttempt">The current attempt.</param>
        /// <returns>Squares of the multiplication of the initial delay by the current attempt.</returns>
        protected override TimeSpan GetNextDelay(int currentAttempt)
        {
            var tmpDelay = currentAttempt * base.Delay.TotalMilliseconds;
            return TimeSpan.FromMilliseconds(tmpDelay * tmpDelay);
        }
    }
}
