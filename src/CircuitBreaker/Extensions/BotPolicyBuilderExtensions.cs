using System;
using Trybot.CircuitBreaker;
using Trybot.Utils;

namespace Trybot
{
    /// <summary>
    /// Circuit breaker related <see cref="IBotPolicyBuilder"/> extensions.
    /// </summary>
    public static class CircuitBreakerBotPolicyBuilderExtensions
    {
        /// <summary>
        /// Adds a circuit breaker bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The bots configurator action.</param>
        /// <param name="strategyConfiguration">The circuit breakers configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(config => config
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)),
        ///             strategyConfig => strategyConfig
        ///                 .DurationOfOpen(TimeSpan.FromSeconds(15))
        ///                 .FailureThresholdBeforeOpen(5)
        ///                 .SuccessThresholdInHalfOpen(2));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder CircuitBreaker(this IBotPolicyBuilder builder,
        Action<CircuitBreakerConfiguration> configuratorAction, Action<DefaultCircuitBreakerStrategyConfiguration> strategyConfiguration)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));
            Shield.EnsureNotNull(strategyConfiguration, nameof(strategyConfiguration));

            var strategyConfig = new DefaultCircuitBreakerStrategyConfiguration();
            strategyConfiguration?.Invoke(strategyConfig);
            return builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config,
                new DefaultCircuitBreakerStrategy(config, strategyConfig)), configuratorAction);
        }

        /// <summary>
        /// Adds a circuit breaker bot with a custom strategy implementation to a <see cref="IBotPolicy"/>.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The bots configurator action.</param>
        /// <param name="strategyFactory">The custom circuit breaker strategy factory delegate.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(switcher => new CustomCircuitBreakerStrategy(switcher),
        ///     config => config
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder CustomCircuitBreaker(this IBotPolicyBuilder builder,
        Func<CircuitBreakerConfiguration, CircuitBreakerStrategy> strategyFactory, Action<CircuitBreakerConfiguration> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));
            Shield.EnsureNotNull(strategyFactory, nameof(strategyFactory));

            return builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config, strategyFactory(config)), configuratorAction);
        }

        /// <summary>
        /// Adds a circuit breaker bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The bots configuraton.</param>
        /// <param name="strategyConfiguration">The circuit breakers configuraton.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(new CircuitBreakerConfiguration()
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)),
        ///             new DefaultCircuitBreakerStrategyConfiguration()
        ///                 .DurationOfOpen(TimeSpan.FromSeconds(15))
        ///                 .FailureThresholdBeforeOpen(5)
        ///                 .SuccessThresholdInHalfOpen(2));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder CircuitBreaker(this IBotPolicyBuilder builder,
            CircuitBreakerConfiguration configuration, DefaultCircuitBreakerStrategyConfiguration strategyConfiguration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));
            Shield.EnsureNotNull(strategyConfiguration, nameof(strategyConfiguration));

            return builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config,
                new DefaultCircuitBreakerStrategy(config, strategyConfiguration)), configuration);
        }

        /// <summary>
        /// Adds a circuit breaker bot with a custom strategy implementation to a <see cref="IBotPolicy"/>.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuration">The bots configuraton.</param>
        /// <param name="strategyFactory">The custom circuit breaker strategy factory delegate.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(switcher => new CustomCircuitBreakerStrategy(switcher),
        ///     new CircuitBreakerConfiguration()
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder CustomCircuitBreaker(this IBotPolicyBuilder builder,
            Func<CircuitBreakerConfiguration, CircuitBreakerStrategy> strategyFactory, CircuitBreakerConfiguration configuration)
        {
            Shield.EnsureNotNull(configuration, nameof(configuration));
            Shield.EnsureNotNull(strategyFactory, nameof(strategyFactory));

            return builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config, strategyFactory(config)), configuration);
        }

        /// <summary>
        /// Adds a circuit breaker bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The bots configurator action.</param>
        /// <param name="strategyConfiguration">The circuit breakers configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(config => config
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .BrakeWhenResultIs(result => result != OperationResult.Ok)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)),
        ///             strategyConfig => strategyConfig
        ///                 .DurationOfOpen(TimeSpan.FromSeconds(15))
        ///                 .FailureThresholdBeforeOpen(5)
        ///                 .SuccessThresholdInHalfOpen(2));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> CircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            Action<CircuitBreakerConfiguration<TResult>> configuratorAction, Action<DefaultCircuitBreakerStrategyConfiguration> strategyConfiguration)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));
            Shield.EnsureNotNull(strategyConfiguration, nameof(strategyConfiguration));

            var strategyConfig = new DefaultCircuitBreakerStrategyConfiguration();
            strategyConfiguration?.Invoke(strategyConfig);
            return builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config,
                new DefaultCircuitBreakerStrategy(config, strategyConfig)), configuratorAction);
        }

        /// <summary>
        /// Adds a circuit breaker bot with a custom strategy implementation to a <see cref="IBotPolicy"/>.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuratorAction">The bots configurator action.</param>
        /// <param name="strategyFactory">The custom circuit breaker strategy factory delegate.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(switcher => new CustomCircuitBreakerStrategy(switcher),
        ///     config => config
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .BrakeWhenResultIs(result => result != OperationResult.Ok)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> CustomCircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            Func<CircuitBreakerConfiguration<TResult>, CircuitBreakerStrategy> strategyFactory, Action<CircuitBreakerConfiguration<TResult>> configuratorAction)
        {
            Shield.EnsureNotNull(configuratorAction, nameof(configuratorAction));
            Shield.EnsureNotNull(strategyFactory, nameof(strategyFactory));

            return builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config, strategyFactory(config)), configuratorAction);
        }

        /// <summary>
        /// Adds a circuit breaker bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuraton">The bots configuraton.</param>
        /// <param name="strategyConfiguration">The circuit breakers configurator action.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(new CircuitBreakerConfiguration()
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .BrakeWhenResultIs(result => result != OperationResult.Ok)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)),
        ///             new DefaultCircuitBreakerStrategyConfiguration()
        ///                 .DurationOfOpen(TimeSpan.FromSeconds(15))
        ///                 .FailureThresholdBeforeOpen(5)
        ///                 .SuccessThresholdInHalfOpen(2));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> CircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            CircuitBreakerConfiguration<TResult> configuraton, DefaultCircuitBreakerStrategyConfiguration strategyConfiguration)
        {
            Shield.EnsureNotNull(configuraton, nameof(configuraton));
            Shield.EnsureNotNull(strategyConfiguration, nameof(strategyConfiguration));

            return builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config,
                new DefaultCircuitBreakerStrategy(config, strategyConfiguration)), configuraton);
        }

        /// <summary>
        /// Adds a circuit breaker bot with a custom strategy implementation to a <see cref="IBotPolicy"/>.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <param name="configuraton">The bots configuraton.</param>
        /// <param name="strategyFactory">The custom circuit breaker strategy factory delegate.</param>
        /// <returns>The policy builder.</returns>
        /// <example>
        /// <code>
        /// builder.CircuitBreaker(switcher => new CustomCircuitBreakerStrategy(switcher),
        ///     new CircuitBreakerConfiguration()
        ///         .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)
        ///         .BrakeWhenResultIs(result => result != OperationResult.Ok)
        ///         .OnClosed(() => onClosedAction())
        ///         .OnHalfOpen(() => onHalfOpenAction())
        ///         .OnOpen(openDuration => onOpenAction(openDuration)));
        /// </code>
        /// </example>
        public static IBotPolicyBuilder<TResult> CustomCircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            Func<CircuitBreakerConfiguration<TResult>, CircuitBreakerStrategy> strategyFactory, CircuitBreakerConfiguration<TResult> configuraton)
        {
            Shield.EnsureNotNull(configuraton, nameof(configuraton));
            Shield.EnsureNotNull(strategyFactory, nameof(strategyFactory));

            return builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config, strategyFactory(config)), configuraton);
        }
    }
}
