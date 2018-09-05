using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Retry
{
    public class RetryBot<TResult> : ConfigurableBot<RetryConfiguration<TResult>, TResult>
    {
        private readonly RetryEngine retryEngine = new RetryEngine();

        internal RetryBot(Bot<TResult> innerPolicy, RetryConfiguration<TResult> configuration) : base(innerPolicy, configuration)
        { }

        public override TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            this.retryEngine.ExecuteRetry(base.Configuration, (ctx, t) => this.InnerBot.Execute(operation, ctx, t),
                onRetry: (tryResult, attemptContext) => base.Configuration.RaiseRetryEvent(tryResult.OperationResult, tryResult.Exception, attemptContext),
                resultChecker: result => base.Configuration.AcceptsResult(result),
                delayCalculator: (tryResult, attempt) => base.Configuration.CalculateNextDelay(attempt, tryResult.OperationResult),
                context: context,
                token: token);

        public override async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext),
                    onRetryAsync: (tryResult, attemptContext, t) => base.Configuration.RaiseRetryEventAsync(tryResult.OperationResult, tryResult.Exception, attemptContext, t),
                    resultChecker: result => base.Configuration.AcceptsResult(result),
                    delayCalculator: (tryResult, attempt) => base.Configuration.CalculateNextDelay(attempt, tryResult.OperationResult),
                    context: context,
                    token: token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

        public override async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext),
                    onRetryAsync: (tryResult, attemptContext, t) => base.Configuration.RaiseRetryEventAsync(tryResult.OperationResult, tryResult.Exception, attemptContext, t),
                    resultChecker: result => base.Configuration.AcceptsResult(result),
                    delayCalculator: (tryResult, attempt) => base.Configuration.CalculateNextDelay(attempt, tryResult.OperationResult),
                    context: context,
                    token: token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
    }

    public class RetryBot : ConfigurableBot<RetryConfiguration>
    {
        private readonly RetryEngine retryEngine = new RetryEngine();

        internal RetryBot(Bot innerPolicy, RetryConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override void Execute(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token) =>
            this.retryEngine.ExecuteRetry(base.Configuration, (ctx, t) =>
                {
                    base.InnerBot.Execute(operation, ctx, t);
                    return Constants.DummyReturnValue;
                },
                onRetry: (tryResult, attemptContext) => base.Configuration.RaiseRetryEvent(tryResult.Exception, attemptContext),
                resultChecker: result => true,
                delayCalculator: (tryResult, attempt) => base.Configuration.CalculateNextDelay(attempt),
                context: context,
                token: token);

        public override async Task ExecuteAsync(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) =>
                {
                    await base.InnerBot.ExecuteAsync(operation, ctx, t)
                        .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext);
                    return Constants.CompletedTask;
                },
                onRetryAsync: (tryResult, attemptContext, t) => base.Configuration.RaiseRetryEventAsync(tryResult.Exception, attemptContext, t),
                resultChecker: result => true,
                delayCalculator: (tryResult, attempt) => base.Configuration.CalculateNextDelay(attempt),
                context: context,
                token: token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

        public override async Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, ExecutionContext context, CancellationToken token) =>
            await this.retryEngine.ExecuteRetryAsync(base.Configuration, async (ctx, t) =>
                {
                    await base.InnerBot.ExecuteAsync(operation, ctx, t)
                        .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext);
                    return Constants.CompletedTask;
                },
                onRetryAsync: (tryResult, attemptContext, t) => base.Configuration.RaiseRetryEventAsync(tryResult.Exception, attemptContext, t),
                resultChecker: result => true,
                delayCalculator: (tryResult, attempt) => base.Configuration.CalculateNextDelay(attempt),
                context: context,
                token: token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
    }
}
