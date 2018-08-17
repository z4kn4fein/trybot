using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Retry
{
    public partial class RetryBot
    {
        public override async Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action,
            ExecutionContext context, CancellationToken token)
        {
            await this.ExecuteRetryAsync(async (ctx, t) =>
            {
                await this.InnerBot.ExecuteAsync(action, ctx, t)
                    .ConfigureAwait(ctx.ExecutorConfiguration.ContinueOnCapturedContext);
                return Task.FromResult<object>(null);
            }, context, token, false).ConfigureAwait(context.ExecutorConfiguration.ContinueOnCapturedContext);
        }

        public override async Task<TResult> ExecuteAsync<TResult>(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            await this.ExecuteRetryAsync(async (ctx, t) => await this.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.ExecutorConfiguration.ContinueOnCapturedContext), context, token, true)
                .ConfigureAwait(context.ExecutorConfiguration.ContinueOnCapturedContext);

        public override async Task<TResult> ExecuteAsync<TResult>(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, ExecutionContext context, CancellationToken token) =>
            await this.ExecuteRetryAsync(async (ctx, t) => await this.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.ExecutorConfiguration.ContinueOnCapturedContext), context, token, true)
                .ConfigureAwait(context.ExecutorConfiguration.ContinueOnCapturedContext);

        private async Task<TResult> ExecuteRetryAsync<TResult>(Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            ExecutionContext context, CancellationToken token, bool checkResult)
        {
            var currentAttempt = 1;
            var tryResult = TryResult.Default;
            while (!token.IsCancellationRequested && !tryResult.IsSucceeded && !this.Configuration.IsMaxAttemptsReached(currentAttempt))
            {
                tryResult = await this.TryAsync(operation, context, token, checkResult)
                    .ConfigureAwait(context.ExecutorConfiguration.ContinueOnCapturedContext);

                if (tryResult.IsSucceeded)
                    return (TResult)tryResult.OperationResult;

                if (this.Configuration.IsMaxAttemptsReached(currentAttempt)) break;

                await TaskDelayer.Sleep(this.Configuration.CalculateNextDelay(currentAttempt, checkResult, tryResult.OperationResult), token)
                    .ConfigureAwait(context.ExecutorConfiguration.ContinueOnCapturedContext);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException<TResult>("Maximum number of retry attempts reached.", tryResult.Exception, (TResult)tryResult.OperationResult);
        }

        private async Task<TryResult> TryAsync<TResult>(Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            ExecutionContext context, CancellationToken token, bool checkResult)
        {
            try
            {
                var result = await operation(context, token)
                    .ConfigureAwait(context.ExecutorConfiguration.ContinueOnCapturedContext);

                if (checkResult && !this.Configuration.AcceptsResult(result))
                    return TryResult.Failed(result: result);

                return TryResult.Succeeded(result);
            }
            catch (Exception exception)
            {
                if (this.Configuration.HandlesException(exception))
                    return TryResult.Failed(exception);

                throw;
            }
        }
    }
}
