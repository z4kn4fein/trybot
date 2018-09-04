namespace Trybot
{
    public class BotPolicyBase<TBot>
    {
        protected BotPolicyConfiguration Configuration { get; } = new BotPolicyConfiguration();

        protected TBot Bot { get; set; }
    }
}
