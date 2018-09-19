using System;
using Trybot.CircuitBreaker;

namespace Trybot
{
    public static class CircuitBreakerBotPolicyBuilderExtensions
    {
        public static IBotPolicyBuilder CircuitBreaker(this IBotPolicyBuilder builder,
        Action<CircuitBreakerConfiguration> configuratorAction, Action<DefaultCircuitBreakerStrategyConfiguration> strategyConfiguration)
        {
            var strategyConfig = new DefaultCircuitBreakerStrategyConfiguration();
            strategyConfiguration?.Invoke(strategyConfig);
            return builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config,
                switcher => new DefaultCircuitBreakerStrategy(switcher, strategyConfig)), configuratorAction);
        }

        public static IBotPolicyBuilder CustomCircuitBreaker(this IBotPolicyBuilder builder,
        Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory, Action<CircuitBreakerConfiguration> configuratorAction) =>
            builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config, strategyFactory), configuratorAction);

        public static IBotPolicyBuilder CircuitBreaker(this IBotPolicyBuilder builder,
            CircuitBreakerConfiguration configuration, DefaultCircuitBreakerStrategyConfiguration strategyConfiguration)
        {
            return builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config,
                switcher => new DefaultCircuitBreakerStrategy(switcher, strategyConfiguration)), configuration);
        }

        public static IBotPolicyBuilder CustomCircuitBreaker(this IBotPolicyBuilder builder,
            Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory, CircuitBreakerConfiguration configuration) =>
            builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config, strategyFactory), configuration);


        public static IBotPolicyBuilder<TResult> CircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            Action<CircuitBreakerConfiguration<TResult>> configuratorAction, Action<DefaultCircuitBreakerStrategyConfiguration> strategyConfiguration)
        {
            var strategyConfig = new DefaultCircuitBreakerStrategyConfiguration();
            strategyConfiguration?.Invoke(strategyConfig);
            return builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config,
                switcher => new DefaultCircuitBreakerStrategy(switcher, strategyConfig)), configuratorAction);
        }

        public static IBotPolicyBuilder<TResult> CustomCircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory, Action<CircuitBreakerConfiguration<TResult>> configuratorAction) =>
            builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config, strategyFactory), configuratorAction);

        public static IBotPolicyBuilder<TResult> CircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            CircuitBreakerConfiguration<TResult> configuraton, DefaultCircuitBreakerStrategyConfiguration strategyConfiguration)
        {
            return builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config,
                switcher => new DefaultCircuitBreakerStrategy(switcher, strategyConfiguration)), configuraton);
        }

        public static IBotPolicyBuilder<TResult> CustomCircuitBreaker<TResult>(this IBotPolicyBuilder<TResult> builder,
            Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory, CircuitBreakerConfiguration<TResult> configuraton) =>
            builder.AddBot((innerBot, config) => new CircuitBreakerBot<TResult>(innerBot, config, strategyFactory), configuraton);
    }
}
