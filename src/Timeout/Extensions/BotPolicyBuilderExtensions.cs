using System;
using Trybot.Timeout;
using Trybot.Utils;

namespace Trybot
{
    /// <summary>
    /// Timeout related <see cref="IBotPolicyBuilder"/> extensions.
    /// </summary>
    public static class TimeoutBotPolicyBuilderExtensions
    {
        /// <summary>
        /// Adds a timeout bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Timeout(config =>
        ///     config.After(TimeSpan.FromSeconds(15))
        ///           .OnTimeout(context => timeoutAction()));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder Timeout(this IBotPolicyBuilder builder, Action<TimeoutConfiguration> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, config) => new TimeoutBot(innerBot, config), configuratorAction);
        }

        /// <summary>
        /// Adds a timeout bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Timeout(
        ///     new TimeoutConfiguration()
        ///         .After(TimeSpan.FromSeconds(15))
        ///         .OnTimeout(context => timeoutAction()));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder Timeout(this IBotPolicyBuilder builder, TimeoutConfiguration configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, config) => new TimeoutBot(innerBot, config), configuration);
        }

        /// <summary>
        /// Adds a timeout bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <typeparam name="TResult">The result type of the passed operation.</typeparam>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Timeout(config =>
        ///     config.After(TimeSpan.FromSeconds(15))
        ///           .OnTimeout(context => timeoutAction()));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> Timeout<TResult>(this IBotPolicyBuilder<TResult> builder, Action<TimeoutConfiguration> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));

            return builder.AddBot((innerBot, config) => new TimeoutBot<TResult>(innerBot, config), configuratorAction);
        }

        /// <summary>
        /// Adds a timeout bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <typeparam name="TResult">The result type of the passed operation.</typeparam>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.Timeout(
        ///     new TimeoutConfiguration()
        ///         .After(TimeSpan.FromSeconds(15))
        ///         .OnTimeout(context => timeoutAction()));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> Timeout<TResult>(this IBotPolicyBuilder<TResult> builder, TimeoutConfiguration configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));

            return builder.AddBot((innerBot, config) => new TimeoutBot<TResult>(innerBot, config), configuration);
        }
    }
}
