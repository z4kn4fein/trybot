using Trybot.Passthrough;

namespace Trybot
{
    public static class PassthroughBotPolicyBuilderExtensions
    {
        public static IBotPolicyBuilder Passthrough(this IBotPolicyBuilder builder) =>
            builder.AddBot(innerBot => new PassthroughBot(innerBot));

        public static IBotPolicyBuilder<TResult> Passthrough<TResult>(this IBotPolicyBuilder<TResult> builder) =>
            builder.AddBot(innerBot => new PassthroughBot<TResult>(innerBot));
    }
}
