using System;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    /// <summary>
    /// Identifies the built-in rate limiter strategies.
    /// </summary>
    public enum RateLimiterMode
    {
        /// <summary>
        /// Using this as the parameter of the <see cref="RateLimiterConfiguration.Strategy"/> 
        /// method indicates that a fixed time window rate limiter strategy should be used to 
        /// determine an operation is allowed to execute or not.
        /// </summary>
        FixedWindow,

        /// <summary>
        /// Using this as the parameter of the <see cref="RateLimiterConfiguration.Strategy"/> 
        /// method indicates that a sliding time window rate limiter strategy should be used to 
        /// determine an operation is allowed to execute or not.
        /// </summary>
        SlidingWindow
    }

    /// <summary>
    /// Describes the configuration of the rate limiter bot.
    /// </summary>
    public class RateLimiterConfiguration
    {
        internal int MaxOperationCount { get; private set; }

        internal TimeSpan Interval { get; private set; }

        internal Func<int, TimeSpan, RateLimiterStrategy> StrategyFactory { get; private set; } = (count, interval) => new SlidingWindowStrategy(count, interval);

        /// <summary>
        /// Sets the maximum allowed operation count within the given time interval.
        /// </summary>
        /// <param name="maxOperationCount">The maximum allowed operation count.</param>
        /// <returns>The configuration.</returns>
        public RateLimiterConfiguration MaxAmountOfAllowedOperations(int maxOperationCount)
        {
            Shield.EnsureTrue(maxOperationCount > 0, $"{nameof(maxOperationCount)} must be greater than zero!");

            this.MaxOperationCount = maxOperationCount;
            return this;
        }

        /// <summary>
        /// Sets the time interval within only the given maximum amount of operations set by the <see cref="MaxAmountOfAllowedOperations"/> allowed.
        /// </summary>
        /// <param name="interval">The time interval.</param>
        /// <returns>The configuration.</returns>
        public RateLimiterConfiguration WithinTimeInterval(TimeSpan interval)
        {
            Shield.EnsureTrue(interval > TimeSpan.Zero, $"{nameof(interval)} must be grater than zero!");

            this.Interval = interval;
            return this;
        }

        /// <summary>
        /// Sets the factory delegate which produces a rate limiter strategy used to determine a given operation should be rejected or not.
        /// </summary>
        /// <param name="strategyFactory">The factory delegate.</param>
        /// <returns>The configuration.</returns>
        public RateLimiterConfiguration Strategy(Func<int, TimeSpan, RateLimiterStrategy> strategyFactory)
        {
            Shield.EnsureNotNull(strategyFactory, nameof(strategyFactory));

            this.StrategyFactory = strategyFactory;
            return this;
        }
    }
}
