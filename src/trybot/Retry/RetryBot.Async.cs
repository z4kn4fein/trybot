using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Retry
{
    public partial class RetryBot<TResult>
    {
        public override async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext), context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

        public override async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext), context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
    }

    public partial class RetryBot
    {
        public override async Task ExecuteAsync(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext), context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

        public override async Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext), context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
    }
}
