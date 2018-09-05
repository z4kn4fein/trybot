using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Exceptions;
using Trybot.Retry.Model;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal class RetryEngine : RetryEngineBase
    {
        public TResult ExecuteRetry<TResult>(RetryConfigurationBase configuration,
            Func<ExecutionContext, CancellationToken, TResult> operation,
            Action<TryResult<TResult>, AttemptContext> onRetry,
            Func<TResult, bool> resultChecker,
            Func<TryResult<TResult>, int, TimeSpan> delayCalculator,
            ExecutionContext context,
            CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult<TResult>.Default;
            while (base.ShouldExecute(tryResult, configuration, currentAttempt, token))
            {
                tryResult = Try(configuration, operation, resultChecker, context, token);

                if (tryResult.IsSucceeded)
                    return tryResult.OperationResult;

                if (this.HasMaxAttemptsReached(configuration, currentAttempt)) break;

                var nextDelay = delayCalculator(tryResult, currentAttempt);
                onRetry(tryResult, AttemptContext.New(currentAttempt, nextDelay, context));

                this.Wait(nextDelay, token);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException<TResult>(Constants.MaxRetryExceptionMessage, tryResult.Exception, tryResult.OperationResult);
        }

        public async Task<TResult> ExecuteRetryAsync<TResult>(RetryConfigurationBase configuration,
            Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            Func<TryResult<TResult>, AttemptContext, CancellationToken, Task> onRetryAsync,
            Func<TResult, bool> resultChecker,
            Func<TryResult<TResult>, int, TimeSpan> delayCalculator,
            ExecutionContext context,
            CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult<TResult>.Default;
            while (base.ShouldExecute(tryResult, configuration, currentAttempt, token))
            {
                tryResult = await TryAsync(configuration, operation, resultChecker, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                if (tryResult.IsSucceeded)
                    return tryResult.OperationResult;

                if (this.HasMaxAttemptsReached(configuration, currentAttempt)) break;

                var nextDelay = delayCalculator(tryResult, currentAttempt);
                await onRetryAsync(tryResult, AttemptContext.New(currentAttempt, nextDelay, context), token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                await this.WaitAsync(nextDelay, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException<TResult>(Constants.MaxRetryExceptionMessage, tryResult.Exception, tryResult.OperationResult);
        }

        private static async Task<TryResult<TResult>> TryAsync<TResult>(RetryConfigurationBase configuration,
            Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            Func<TResult, bool> resultChecker,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                var result = await operation(context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                return HandleResult(resultChecker, result);
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return TryResult<TResult>.Failed(exception);

                throw;
            }
        }

        private static TryResult<TResult> Try<TResult>(RetryConfigurationBase configuration,
            Func<ExecutionContext, CancellationToken, TResult> operation,
            Func<TResult, bool> resultChecker,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                var result = operation(context, token);
                return HandleResult(resultChecker, result);
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return TryResult<TResult>.Failed(exception);

                throw;
            }
        }

        private static TryResult<TResult> HandleResult<TResult>(Func<TResult, bool> resultChecker, TResult result) =>
            resultChecker(result) ? TryResult<TResult>.Succeeded(result) : TryResult<TResult>.Failed(result: result);
    }
}
