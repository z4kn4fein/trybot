using System;
using Trybot.CircuitBreaker;
using Trybot.Fallback;

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
        Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory,  Action<CircuitBreakerConfiguration> configuratorAction) =>
            builder.AddBot((innerBot, config) => new CircuitBreakerBot(innerBot, config, strategyFactory), configuratorAction);
    }
}
