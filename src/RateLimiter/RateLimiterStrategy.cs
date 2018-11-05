using System;

namespace Trybot.RateLimiter
{
    /// <summary>
    /// Represent a rate limiter strategy abstract class used by the rate limiter implementations.
    /// </summary>
    public abstract class RateLimiterStrategy
    {
        /// <summary>
        /// The maximum count of the allowed operations within the given interval.
        /// </summary>
        protected int MaxOperationCount { get; }

        /// <summary>
        /// The time interval.
        /// </summary>
        protected TimeSpan Interval { get; }

        /// <summary>
        /// Constructor used to construct rate limiter implementations.
        /// </summary>
        /// <param name="maxOperationCount">The maximum count of the allowed operations within the given interval.</param>
        /// <param name="interval">The time interval.</param>
        protected RateLimiterStrategy(int maxOperationCount, TimeSpan interval)
        {
            this.MaxOperationCount = maxOperationCount;
            this.Interval = interval;
        }
    }
}
