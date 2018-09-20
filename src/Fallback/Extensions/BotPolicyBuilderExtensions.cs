using System;
using Trybot.Fallback;
using Trybot.Utils;

namespace Trybot
{
    /// <summary>
    /// Fallback related <see cref="IBotPolicyBuilder"/> extensions.
    /// </summary>
    public static class FallbackBotPolicyBuilderExtensions
    {
        /// <summary>
        /// Adds a fallback bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Fallback(config => config
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .OnFallback((exception, context) => onFallbackAction()))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder Fallback(this IBotPolicyBuilder builder, Action<FallbackConfiguration> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, config) => new FallbackBot(innerBot, config), configuratorAction);
        }

        /// <summary>
        /// Adds a fallback bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Fallback(new FallbackConfiguration()
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .OnFallback((exception, context) => onFallbackAction()))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder Fallback(this IBotPolicyBuilder builder, FallbackConfiguration configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, config) => new FallbackBot(innerBot, config), configuration);
        }

        /// <summary>
        /// Adds a fallback bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Fallback(config => config
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .WhenResultIs(result => result != OperationResult.Ok)
        ///     .OnFallback((exception, context) => onFallbackAction()))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> Fallback<TResult>(this IBotPolicyBuilder<TResult> builder, Action<FallbackConfiguration<TResult>> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, config) => new FallbackBot<TResult>(innerBot, config), configuratorAction);
        }

        /// <summary>
        /// Adds a fallback bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Fallback(new FallbackConfiguration()
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .WhenResultIs(result => result != OperationResult.Ok)
        ///     .OnFallback((exception, context) => onFallbackAction()))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> Fallback<TResult>(this IBotPolicyBuilder<TResult> builder, FallbackConfiguration<TResult> configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, config) => new FallbackBot<TResult>(innerBot, config), configuration);
        }
    }
}
