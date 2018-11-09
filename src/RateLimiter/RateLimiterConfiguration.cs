using System;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    /// <summary>
    /// Describes the configuration of the rate limiter bot.
    /// </summary>
    public class RateLimiterConfiguration
    {
        internal IRateLimiterStrategy Strategy { get; private set; }

        /// <summary>
        /// Sets a custom rate limiter strategy used to determine a given operation should be rejected or not.
        /// </summary>
        /// <param name="strategy">The rate limiter strategy.</param>
        /// <returns>The configuration.</returns>
        public RateLimiterConfiguration UseStrategy(IRateLimiterStrategy strategy)
        {
            Shield.EnsureNotNull(strategy, nameof(strategy));

            this.Strategy = strategy;
            return this;
        }

        /// <summary>
        /// Sets the underlying strategy to fixed time window rate limiter used to 
        /// determine an operation is allowed to execute or not.
        /// </summary>
        /// <param name="maxAmountOfAllowedOperations">The maximum allowed operation count within the given time interval.</param>
        /// <param name="withinTimeInterval">The time interval within only the given maximum amount of operations set by the <paramref name="maxAmountOfAllowedOperations"/> allowed.</param>
        /// <returns></returns>
        public RateLimiterConfiguration FixedWindow(int maxAmountOfAllowedOperations, TimeSpan withinTimeInterval)
        {
            Shield.EnsureTrue(maxAmountOfAllowedOperations > 0, $"{nameof(maxAmountOfAllowedOperations)} must be greater than zero!");
            Shield.EnsureTrue(withinTimeInterval > TimeSpan.Zero, $"{nameof(withinTimeInterval)} must be grater than zero!");

            this.Strategy = new FixedWindowStrategy(maxAmountOfAllowedOperations, withinTimeInterval);
            return this;
        }

        /// <summary>
        /// Sets the underlying strategy to sliding window rate limiter used to 
        /// determine an operation is allowed to execute or not.
        /// </summary>
        /// <param name="maxAmountOfAllowedOperations">The maximum allowed operation count within the given time interval.</param>
        /// <param name="withinTimeInterval">The time interval within only the given maximum amount of operations set by the <paramref name="maxAmountOfAllowedOperations"/> allowed.</param>
        /// <returns></returns>
        public RateLimiterConfiguration SlidingWindow(int maxAmountOfAllowedOperations, TimeSpan withinTimeInterval)
        {
            Shield.EnsureTrue(maxAmountOfAllowedOperations > 0, $"{nameof(maxAmountOfAllowedOperations)} must be greater than zero!");
            Shield.EnsureTrue(withinTimeInterval > TimeSpan.Zero, $"{nameof(withinTimeInterval)} must be grater than zero!");

            this.Strategy = new SlidingWindowStrategy(maxAmountOfAllowedOperations, withinTimeInterval);
            return this;
        }
    }
}
