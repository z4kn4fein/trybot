using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot.Fallback
{
    internal class FallbackBot<TResult> : ConfigurableBot<FallbackConfiguration<TResult>, TResult>
    {
        internal FallbackBot(Bot<TResult> innerBot, FallbackConfiguration<TResult> configuration) : base(innerBot, configuration)
        {
        }

        public override TResult Execute(IBotOperation<TResult> operation,
            ExecutionContext context, CancellationToken token)
        {

            try
            {
                var result = base.InnerBot.Execute(operation, context, token);
                return base.Configuration.AcceptsResult(result) ? result : base.Configuration.RaiseFallbackEvent(result, null, context);
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    return base.Configuration.RaiseFallbackEvent(default, exception, context);

                throw;
            }
        }

        public override async Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation,
            ExecutionContext context, CancellationToken token)
        {
            try
            {
                var result = await base.InnerBot.ExecuteAsync(operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                if (!base.Configuration.AcceptsResult(result))
                    return await base.Configuration.RaiseFallbackEventAsync(result, null, context, token)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                return result;
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    return await base.Configuration.RaiseFallbackEventAsync(default, exception, context, token)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                throw;
            }
        }
    }
}
