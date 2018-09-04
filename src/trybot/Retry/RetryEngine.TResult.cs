using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Exceptions;
using Trybot.Retry.Model;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal class RetryEngine<TResult> : RetryEngineBase
    {
        public TResult ExecuteRetry(RetryConfiguration<TResult> configuration, Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult<TResult>.Default;
            while (base.ShouldExecute(tryResult, configuration, currentAttempt, token))
            {
                tryResult = Try(configuration, operation, context, token);

                if (tryResult.IsSucceeded)
                    return tryResult.OperationResult;

                if (this.HasMaxAttemptsReached(configuration, currentAttempt)) break;

                var nextDelay = this.CalculateNextDelay(configuration, currentAttempt, tryResult.OperationResult);
                configuration.RaiseRetryEvent(tryResult.OperationResult, tryResult.Exception, AttemptContext.New(currentAttempt, nextDelay, context));

                this.Wait(nextDelay, token);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException<TResult>(Constants.MaxRetryExceptionMessage, tryResult.Exception, tryResult.OperationResult);
        }

        public async Task<TResult> ExecuteRetryAsync(RetryConfiguration<TResult> configuration, Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult<TResult>.Default;
            while (base.ShouldExecute(tryResult, configuration, currentAttempt, token))
            {
                tryResult = await TryAsync(configuration, operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                if (tryResult.IsSucceeded)
                    return tryResult.OperationResult;

                if (this.HasMaxAttemptsReached(configuration, currentAttempt)) break;

                var nextDelay = this.CalculateNextDelay(configuration, currentAttempt, tryResult.OperationResult);
                await configuration.RaiseRetryEventAsync(tryResult.OperationResult, tryResult.Exception, AttemptContext.New(currentAttempt, nextDelay, context), token);

                await this.WaitAsync(nextDelay, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException<TResult>(Constants.MaxRetryExceptionMessage, tryResult.Exception, tryResult.OperationResult);
        }

        private static async Task<TryResult<TResult>> TryAsync(RetryConfiguration<TResult> configuration, Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            ExecutionContext context, CancellationToken token)
        {
            try
            {
                var result = await operation(context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                return HandleResult(configuration, result);
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return TryResult<TResult>.Failed(exception);

                throw;
            }
        }

        private static TryResult<TResult> Try(RetryConfiguration<TResult> configuration, Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token)
        {
            try
            {
                var result = operation(context, token);
                return HandleResult(configuration, result);
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return TryResult<TResult>.Failed(exception);

                throw;
            }
        }

        private static TryResult<TResult> HandleResult(RetryConfiguration<TResult> configuration, TResult result) =>
            !configuration.AcceptsResult(result) ? TryResult<TResult>.Failed(result: result) : TryResult<TResult>.Succeeded(result);
    }
}
