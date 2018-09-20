using System;

namespace Trybot
{
    /// <summary>
    /// Represents a policy builder which can be used to configure a <see cref="IBotPolicy"/> instance with several <see cref="Bot"/> implementations.
    /// </summary>
    public interface IBotPolicyBuilder
    {
        /// <summary>
        /// Adds a <see cref="ConfigurableBot{TConfiguration}"/> implementation to the policy.
        /// </summary>
        /// <typeparam name="TBot">The type of the <see cref="Bot"/>.</typeparam>
        /// <typeparam name="TConfiguration">The type of the configuration.</typeparam>
        /// <param name="factory">The factory delegate used to create the <see cref="Bot"/> implementation.</param>
        /// <param name="configuratorAction">The action delegate used to configure the given <see cref="Bot"/>.</param>
        /// <returns>The policy builder itself.</returns>
        IBotPolicyBuilder AddBot<TBot, TConfiguration>(Func<Bot, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : ConfigurableBot<TConfiguration>
            where TConfiguration : new();

        /// <summary>
        /// Adds a <see cref="ConfigurableBot{TConfiguration}"/> implementation to the policy.
        /// </summary>
        /// <typeparam name="TBot">The type of the <see cref="Bot"/>.</typeparam>
        /// <typeparam name="TConfiguration">The type of the configuration.</typeparam>
        /// <param name="factory">The factory delegate used to create the <see cref="Bot"/> implementation.</param>
        /// <param name="configuration">The configuration used to configure the given <see cref="Bot"/>.</param>
        /// <returns>The policy builder itself.</returns>
        IBotPolicyBuilder AddBot<TBot, TConfiguration>(Func<Bot, TConfiguration, TBot> factory, TConfiguration configuration)
            where TBot : ConfigurableBot<TConfiguration>;

        /// <summary>
        /// Adds a <see cref="Bot"/> implementation to the policy.
        /// </summary>
        /// <typeparam name="TBot">The type of the <see cref="Bot"/>.</typeparam>
        /// <param name="factory">The factory delegate used to create the <see cref="Bot"/> implementation.</param>
        /// <returns>The policy builder itself.</returns>
        IBotPolicyBuilder AddBot<TBot>(Func<Bot, TBot> factory)
            where TBot : Bot;
    }

    /// <summary>
    /// Represents a policy builder which can be used to configure a <see cref="IBotPolicy{TResult}"/> instance with several <see cref="Bot{TResult}"/> implementations.
    /// </summary>
    public interface IBotPolicyBuilder<TResult>
    {
        /// <summary>
        /// Adds a <see cref="ConfigurableBot{TConfiguration,TResult}"/> implementation to the policy.
        /// </summary>
        /// <typeparam name="TBot">The type of the <see cref="Bot"/>.</typeparam>
        /// <typeparam name="TConfiguration">The type of the configuration.</typeparam>
        /// <param name="factory">The factory delegate used to create the <see cref="Bot"/> implementation.</param>
        /// <param name="configuratorAction">The action delegate used to configure the given <see cref="Bot"/>.</param>
        /// <returns>The policy builder itself.</returns>
        IBotPolicyBuilder<TResult> AddBot<TBot, TConfiguration>(Func<Bot<TResult>, TConfiguration, TBot> factory, Action<TConfiguration> configuratorAction)
            where TBot : ConfigurableBot<TConfiguration, TResult>
            where TConfiguration : new();

        /// <summary>
        /// Adds a <see cref="ConfigurableBot{TConfiguration,TResult}"/> implementation to the policy.
        /// </summary>
        /// <typeparam name="TBot">The type of the <see cref="Bot"/>.</typeparam>
        /// <typeparam name="TConfiguration">The type of the configuration.</typeparam>
        /// <param name="factory">The factory delegate used to create the <see cref="Bot"/> implementation.</param>
        /// <param name="configuration">The configuration used to configure the given <see cref="Bot"/>.</param>
        /// <returns>The policy builder itself.</returns>
        IBotPolicyBuilder<TResult> AddBot<TBot, TConfiguration>(Func<Bot<TResult>, TConfiguration, TBot> factory, TConfiguration configuration)
            where TBot : ConfigurableBot<TConfiguration, TResult>;

        /// <summary>
        /// Adds a <see cref="Bot{TResult}"/> implementation to the policy.
        /// </summary>
        /// <typeparam name="TBot">The type of the <see cref="Bot"/>.</typeparam>
        /// <param name="factory">The factory delegate used to create the <see cref="Bot"/> implementation.</param>
        /// <returns>The policy builder itself.</returns>
        IBotPolicyBuilder<TResult> AddBot<TBot>(Func<Bot<TResult>, TBot> factory)
            where TBot : Bot<TResult>;
    }
}
