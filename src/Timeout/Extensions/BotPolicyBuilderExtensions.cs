using System;
using Trybot.Timeout;

namespace Trybot
{
    public static class TimeoutBotPolicyBuilderExtensions
    {
        public static IBotPolicyBuilder Timeout(this IBotPolicyBuilder builder, Action<TimeoutConfiguration> configuratorAction) =>
            builder.AddBot((innerBot, config) => new TimeoutBot(innerBot, config), configuratorAction);

        public static IBotPolicyBuilder<TResult> Timeout<TResult>(this IBotPolicyBuilder<TResult> builder, Action<TimeoutConfiguration> configuratorAction) =>
            builder.AddBot((innerBot, config) => new TimeoutBot<TResult>(innerBot, config), configuratorAction);
    }
}
