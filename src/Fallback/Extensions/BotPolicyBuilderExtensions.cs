using System;
using Trybot.Fallback;

namespace Trybot
{
    public static class FallbackBotPolicyBuilderExtensions
    {
        public static IBotPolicyBuilder Fallback(this IBotPolicyBuilder builder, Action<FallbackConfiguration> configuratorAction) =>
            builder.AddBot((innerBot, config) => new FallbackBot(innerBot, config), configuratorAction);

        public static IBotPolicyBuilder<TResult> Fallback<TResult>(this IBotPolicyBuilder<TResult> builder, Action<FallbackConfiguration<TResult>> configuratorAction) =>
            builder.AddBot((innerBot, config) => new FallbackBot<TResult>(innerBot, config), configuratorAction);
    }
}
