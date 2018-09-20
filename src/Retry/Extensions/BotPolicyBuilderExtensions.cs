using System;
using Trybot.Retry;
using Trybot.Utils;

namespace Trybot
{
    /// <summary>
    /// Retry related <see cref="IBotPolicyBuilder"/> extensions.
    /// </summary>
    public static class RetryBotPolicyBuilderExtensions
    {
        /// <summary>
        /// Adds a retry bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Retry(new RetryConfiguration()
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .WithMaxAttemptCount(5)
        ///     .WaitBetweenAttempts((attempt, exception) => 
        ///         TimeSpan.FromSeconds(Math.Pow(2, attempt)))
        ///     .OnRetry((exception, context) => Log($"{context.CurrentAttempt}. attempt, wait {context.CurrentDelay}")))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder Retry(this IBotPolicyBuilder builder, RetryConfiguration configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, retryConfig) => new RetryBot(innerBot, retryConfig), configuration);
        }

        /// <summary>
        /// Adds a retry bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Retry(config => config
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .WithMaxAttemptCount(5)
        ///     .WaitBetweenAttempts((attempt, exception) => 
        ///         TimeSpan.FromSeconds(Math.Pow(2, attempt)))
        ///     .OnRetry((exception, context) => Log($"{context.CurrentAttempt}. attempt, wait {context.CurrentDelay}")))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder Retry(this IBotPolicyBuilder builder, Action<RetryConfiguration> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, retryConfig) => new RetryBot(innerBot, retryConfig), configuratorAction);
        }

        /// <summary>
        /// Adds a retry bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <typeparam name="TResult">The result type of the passed operation.</typeparam>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Retry(new RetryConfiguration()
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .WhenResultIs(result => result != OperationResult.Ok)
        ///     .WithMaxAttemptCount(5)
        ///     .WaitBetweenAttempts((attempt, result, exception) => 
        ///         TimeSpan.FromSeconds(Math.Pow(2, attempt)))
        ///     .OnRetry((exception, result, context) => Log($"{context.CurrentAttempt}. attempt, wait {context.CurrentDelay}")))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> Retry<TResult>(this IBotPolicyBuilder<TResult> builder, RetryConfiguration<TResult> configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, retryConfig) => new RetryBot<TResult>(innerBot, retryConfig), configuration);
        }

        /// <summary>
        /// Adds a retry bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <typeparam name="TResult">The result type of the passed operation.</typeparam>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Retry(config => config
        ///     .WhenExceptionOccurs(exception => exception is HttpRequestException)
        ///     .WhenResultIs(result => result != OperationResult.Ok)
        ///     .WithMaxAttemptCount(5)
        ///     .WaitBetweenAttempts((attempt, result, exception) => 
        ///         TimeSpan.FromSeconds(Math.Pow(2, attempt)))
        ///     .OnRetry((exception, result, context) => Log($"{context.CurrentAttempt}. attempt, wait {context.CurrentDelay}")))
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> Retry<TResult>(this IBotPolicyBuilder<TResult> builder, Action<RetryConfiguration<TResult>> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, retryConfig) => new RetryBot<TResult>(innerBot, retryConfig), configuratorAction);
        }
    }
}
