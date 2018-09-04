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
        public void ExecuteRetry(RetryConfiguration configuration, Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult.Default;
            while (this.ShouldExecute(tryResult, configuration, currentAttempt, token))
            {
                tryResult = Try(configuration, operation, context, token);

                if (tryResult.IsSucceeded)
                    return;

                if (this.HasMaxAttemptsReached(configuration, currentAttempt)) break;

                var nextDelay = this.CalculateNextDelay(configuration, currentAttempt);
                configuration.RaiseRetryEvent(tryResult.Exception, AttemptContext.New(currentAttempt, nextDelay, context));

                this.Wait(nextDelay, token);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException(Constants.MaxRetryExceptionMessage, tryResult.Exception);
        }

        public async Task ExecuteRetryAsync(RetryConfiguration configuration, Func<ExecutionContext, CancellationToken, Task> operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult.Default;
            while (this.ShouldExecute(tryResult, configuration, currentAttempt, token))
            {
                tryResult = await TryAsync(configuration, operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                if (tryResult.IsSucceeded)
                    return;

                if (this.HasMaxAttemptsReached(configuration, currentAttempt)) break;

                var nextDelay = this.CalculateNextDelay(configuration, currentAttempt);
                await configuration.RaiseRetryEventAsync(tryResult.Exception, AttemptContext.New(currentAttempt, nextDelay, context), token);

                await this.WaitAsync(nextDelay, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException(Constants.MaxRetryExceptionMessage, tryResult.Exception);
        }

        private static async Task<TryResult> TryAsync(RetryConfigurationBase configuration, Func<ExecutionContext, CancellationToken, Task> operation,
            ExecutionContext context, CancellationToken token)
        {
            try
            {
                await operation(context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                return TryResult.Succeeded();
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return TryResult.Failed(exception);

                throw;
            }
        }

        private static TryResult Try(RetryConfigurationBase configuration, Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token)
        {
            try
            {
                operation(context, token);
                return TryResult.Succeeded();
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return TryResult.Failed(exception);

                throw;
            }
        }
    }
}
