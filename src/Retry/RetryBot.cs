using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Retry
{
    public partial class RetryBot<TResult> : ConfigurableBot<RetryConfiguration<TResult>, TResult>
    {
        private readonly RetryEngine<TResult> retryEngine = new RetryEngine<TResult>();

        public RetryBot(Bot<TResult> innerPolicy, RetryConfiguration<TResult> configuration) : base(innerPolicy, configuration)
        { }

        public override TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            this.retryEngine.ExecuteRetry(base.Configuration, (ctx, t) => this.InnerBot.Execute(operation, ctx, t), context, token);

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

    public partial class RetryBot : ConfigurableBot<RetryConfiguration>
    {
        private readonly RetryEngine retryEngine = new RetryEngine();

        public RetryBot(Bot innerPolicy, RetryConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override void Execute(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token) =>
            this.retryEngine.ExecuteRetry(base.Configuration, (ctx, t) => base.InnerBot.Execute(operation, ctx, t), context, token);

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
