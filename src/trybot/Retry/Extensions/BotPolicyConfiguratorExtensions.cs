using System;
using Trybot.Retry;

namespace Trybot
{
    public static class BotPolicyConfiguratorExtensions
    {
        public static IBotPolicyConfigurator Retry(this IBotPolicyConfigurator configurator, Action<RetryConfiguration> configuratorAction) =>
            configurator.Configure(config => config.AddBot((innerBot, retryConfig) => new RetryBot(innerBot, retryConfig), configuratorAction));

        public static IBotPolicyConfigurator<TResult> Retry<TResult>(this IBotPolicyConfigurator<TResult> configurator, Action<RetryConfiguration<TResult>> configuratorAction) =>
            configurator.Configure(config => config.AddBot((innerBot, retryConfig) => new RetryBot<TResult>(innerBot, retryConfig), configuratorAction));
    }
}
