using System;

namespace Trybot.RateLimiter
{
    /// <summary>
    /// Represent a rate limiter strategy abstract class used by the rate limiter implementations.
    /// </summary>
    public abstract class RateLimiterStrategy
    {
        /// <summary>
        /// Using this as the parameter of the <see cref="RateLimiterConfiguration.UseStrategy"/> 
        /// method indicates that a fixed time window rate limiter strategy should be used to 
        /// determine an operation is allowed to execute or not.
        /// </summary>
        public static readonly Func<int, TimeSpan, RateLimiterStrategy> FixedWindow = (count, interval) => new FixedWindowStrategy(count, interval);

        /// <summary>
        /// Using this as the parameter of the <see cref="RateLimiterConfiguration.UseStrategy"/> 
        /// method indicates that a sliding time window rate limiter strategy should be used to 
        /// determine an operation is allowed to execute or not.
        /// </summary>
        public static readonly Func<int, TimeSpan, RateLimiterStrategy> SlidingWindow = (count, interval) => new SlidingWindowStrategy(count, interval);

        /// <summary>
        /// The maximum count of the allowed operations within the given interval.
        /// </summary>
        protected int MaxOperationCount { get; }

        /// <summary>
        /// The time interval within the maximum amount of operations allowed.
        /// </summary>
        protected TimeSpan Interval { get; }

        /// <summary>
        /// Constructor used to construct rate limiter implementations.
        /// </summary>
        /// <param name="maxOperationCount">The maximum count of the allowed operations within the given interval.</param>
        /// <param name="interval">The time interval within the maximum amount of operations allowed.</param>
        protected RateLimiterStrategy(int maxOperationCount, TimeSpan interval)
        {
            this.MaxOperationCount = maxOperationCount;
            this.Interval = interval;
        }

        /// <summary>
        /// Determines whether the current operation should be rejected by the rate limirer strategy or not.
        /// </summary>
        /// <returns>True if the current operation should be rejected, otherwise false.</returns>
        public abstract bool ShouldLimit();
    }
}
