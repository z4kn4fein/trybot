using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Timeout.Exceptions;
using Trybot.Utils;

namespace Trybot.Timeout
{
    internal class TimeoutEngine
    {
        public TResult Execute<TResult>(TimeoutConfiguration configuration,
            Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context,
            CancellationToken token)
        {
            using (var timeoutTokenSource = new CancellationTokenSource())
            using (var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutTokenSource.Token))
            {
                try
                {
                    timeoutTokenSource.CancelAfter(configuration.Timeout);
                    return operation(context, combinedTokenSource.Token);
                }
                catch (Exception ex)
                {
                    if (!timeoutTokenSource.IsCancellationRequested) throw;

                    configuration.RaiseTimeoutEvent(context);
                    throw new OperationTimeoutException(Constants.TimeoutExceptionMessage, ex);

                }
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(TimeoutConfiguration configuration,
            Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            ExecutionContext context,
            CancellationToken token)
        {
            using (var timeoutTokenSource = new CancellationTokenSource())
            using (var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutTokenSource.Token))
            {
                try
                {
                    timeoutTokenSource.CancelAfter(configuration.Timeout);
                    return await operation(context, combinedTokenSource.Token)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                }
                catch (Exception ex)
                {
                    if (!timeoutTokenSource.IsCancellationRequested) throw;

                    await configuration.RaiseAsyncTimeoutEvent(context)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                    throw new OperationTimeoutException(Constants.TimeoutExceptionMessage, ex);

                }
            }
        }
    }
}
