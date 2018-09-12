using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Fallback
{
    internal class FallbackEngine
    {
        public TResult Execute<TResult>(FallbackConfigurationBase configuration,
            Func<ExecutionContext, CancellationToken, TResult> operation,
            Func<TResult, Exception, ExecutionContext, TResult> onFallback,
            Func<TResult, bool> resultChecker,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                var result = operation(context, token);
                return resultChecker(result) ? result : onFallback(result, null, context);
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return onFallback(default, exception, context);

                throw;
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(FallbackConfigurationBase configuration,
            Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            Func<TResult, Exception, ExecutionContext, CancellationToken, Task<TResult>> onFallbackAsync,
            Func<TResult, bool> resultChecker,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                var result = await operation(context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                if (!resultChecker(result))
                    return await onFallbackAsync(result, null, context, token)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                return result;
            }
            catch (Exception exception)
            {
                if (configuration.HandlesException(exception))
                    return await onFallbackAsync(default, exception, context, token)
                         .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                throw;
            }
        }
    }
}
