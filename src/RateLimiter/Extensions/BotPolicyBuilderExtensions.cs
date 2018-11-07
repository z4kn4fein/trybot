using System;
using Trybot.RateLimiter;
using Trybot.Utils;

namespace Trybot
{
    /// <summary>
    /// Rate limiter related <see cref="IBotPolicyBuilder"/> extensions.
    /// </summary>
    public static class RateLimiterBotPolicyBuilderExtensions
    {
        /// <summary>
        /// Adds a rate limiter bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.RateLimit(new RateLimiterConfiguration()
        ///     .MaxAmountOfAllowedOperations(10)
        ///     .WithinTimeInterval(TimeSpan.FromSeconds(5))
        ///     .UseStrategy(RateLimiterStrategy.SlidingWindow))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder RateLimit(this IBotPolicyBuilder builder, RateLimiterConfiguration configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, rateLimitConfig) => new RateLimiterBot(innerBot, rateLimitConfig), configuration);
        }

        /// <summary>
        /// Adds a rate limiter bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.RateLimit(config => config
        ///     .MaxAmountOfAllowedOperations(10)
        ///     .WithinTimeInterval(TimeSpan.FromSeconds(5))
        ///     .UseStrategy(RateLimiterStrategy.SlidingWindow))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder RateLimit(this IBotPolicyBuilder builder, Action<RateLimiterConfiguration> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, rateLimitConfig) => new RateLimiterBot(innerBot, rateLimitConfig), configuratorAction);
        }

        /// <summary>
        /// Adds a rate limiter bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <typeparam name="TResult">The result type of the passed operation.</typeparam>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.RateLimit(new RetryConfiguration()
        ///     .MaxAmountOfAllowedOperations(10)
        ///     .WithinTimeInterval(TimeSpan.FromSeconds(5))
        ///     .UseStrategy(RateLimiterStrategy.SlidingWindow))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> RateLimit<TResult>(this IBotPolicyBuilder<TResult> builder, RateLimiterConfiguration configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, rateLimitConfig) => new RateLimiterBot<TResult>(innerBot, rateLimitConfig), configuration);
        }

        /// <summary>
        /// Adds a rate limiter bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <typeparam name="TResult">The result type of the passed operation.</typeparam>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.RateLimit(config => config
        ///     .MaxAmountOfAllowedOperations(10)
        ///     .WithinTimeInterval(TimeSpan.FromSeconds(5))
        ///     .UseStrategy(RateLimiterStrategy.SlidingWindow))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> RateLimit<TResult>(this IBotPolicyBuilder<TResult> builder, Action<RateLimiterConfiguration> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, rateLimitConfig) => new RateLimiterBot<TResult>(innerBot, rateLimitConfig), configuratorAction);
        }
    }
}
