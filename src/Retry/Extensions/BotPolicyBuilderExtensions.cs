using System;
using Trybot.Retry;

namespace Trybot
{
    public static class RetryBotPolicyBuilderExtensions
    {
        public static IBotPolicyBuilder Retry(this IBotPolicyBuilder builder, Action<RetryConfiguration> configuratorAction) =>
            builder.AddBot((innerBot, retryConfig) => new RetryBot(innerBot, retryConfig), configuratorAction);

        public static IBotPolicyBuilder<TResult> Retry<TResult>(this IBotPolicyBuilder<TResult> builder, Action<RetryConfiguration<TResult>> configuratorAction) =>
            builder.AddBot((innerBot, retryConfig) => new RetryBot<TResult>(innerBot, retryConfig), configuratorAction);
    }
}
