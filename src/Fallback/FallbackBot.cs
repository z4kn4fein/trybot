using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot.Fallback
{
    public class FallbackBot : ConfigurableBot<FallbackConfiguration>
    {
        internal FallbackBot(Bot innerBot, FallbackConfiguration configuration) : base(innerBot, configuration)
        { }

        public override void Execute(IBotOperation operation,
            ExecutionContext context, CancellationToken token)
        {
            try
            {
                base.InnerBot.Execute(operation, context, token);
            }
            catch (Exception exception)
            {
                if (!base.Configuration.HandlesException(exception)) throw;
                base.Configuration.RaiseFallbackEvent(exception, context);
            }
        }

        public override async Task ExecuteAsync(IAsyncBotOperation operation,
            ExecutionContext context, CancellationToken token)
        {
            try
            {
                await base.InnerBot.ExecuteAsync(operation, context, token);
            }
            catch (Exception exception)
            {
                if (!base.Configuration.HandlesException(exception)) throw;

                await base.Configuration.RaiseFallbackEventAsync(exception, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

            }
        }
    }
}
