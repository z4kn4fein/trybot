namespace Trybot
{
    /// <summary>
    /// Represents a shared base class for the <see cref="BotPolicy"/>. Contains the policy configuration and an inital bot.
    /// </summary>
    /// <typeparam name="TBot">The type of the accepted bots in the policy. Could be <see cref="Bot"/> or <see cref="Bot{TResult}"/>.</typeparam>
    public class BotPolicyBase<TBot>
    {
        /// <summary>
        /// The bot policy configuration.
        /// </summary>
        protected BotPolicyConfiguration Configuration { get; } = new BotPolicyConfiguration();

        /// <summary>
        /// The initial bot.
        /// </summary>
        protected TBot Bot { get; set; }

        /// <summary>
        /// Stricts the implementors to pass a nested bot to each policy before the first use.
        /// </summary>
        /// <param name="bot">The nested bot.</param>
        protected BotPolicyBase(TBot bot)
        {
            this.Bot = bot;
        }
    }
}
