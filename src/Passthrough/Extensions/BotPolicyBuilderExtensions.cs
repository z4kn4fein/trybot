using Trybot.Passthrough;

namespace Trybot
{
    /// <summary>
    /// Passthrough related <see cref="IBotPolicyBuilder"/> extensions.
    /// </summary>
    public static class PassthroughBotPolicyBuilderExtensions
    {
        /// <summary>
        /// Adds a Passthrough bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// This bot has a really simple purpose, it only proxies the given operation to its nested bot.
        /// </summary>
        /// <param name="builder">The policy builder.</param>
        /// <returns>The policy builder.</returns>
        public static IBotPolicyBuilder Passthrough(this IBotPolicyBuilder builder) =>
            builder.AddBot(innerBot => new PassthroughBot(innerBot));

        /// <summary>
        /// Adds a Passthrough bot to a <see cref="IBotPolicy"/> with the given configuration.
        /// This bot has a really simple purpose, it only proxies the given operation to its nested bot.
        /// </summary>
        /// <typeparam name="TResult">The result type of the passed operation.</typeparam>
        /// <param name="builder">The policy builder.</param>
        /// <returns>The policy builder.</returns>
        public static IBotPolicyBuilder<TResult> Passthrough<TResult>(this IBotPolicyBuilder<TResult> builder) =>
            builder.AddBot(innerBot => new PassthroughBot<TResult>(innerBot));
    }
}
