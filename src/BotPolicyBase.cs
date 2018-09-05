namespace Trybot
{
    public class BotPolicyBase<TBot>
    {
        protected BotPolicyConfiguration Configuration { get; } = new BotPolicyConfiguration();

        protected TBot Bot { get; set; }

        protected BotPolicyBase(TBot bot)
        {
            this.Bot = bot;
        }
    }
}
