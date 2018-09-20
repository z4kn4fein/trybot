using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Model;

namespace Trybot.Retry
{
    /// <summary>
    /// Contains the shared members of the different types of retry configurations.
    /// </summary>
    public class RetryConfigurationBase
    {
        internal int RetryCount { get; set; } = 1;
        internal Func<int, Exception, TimeSpan> RetryStrategy { get; set; } = (attempt, ex) => TimeSpan.Zero;
        internal Func<Exception, bool> RetryPolicy { get; set; }
        internal Action<Exception, AttemptContext> RetryHandler { get; set; }
        internal Func<Exception, AttemptContext, CancellationToken, Task> AsyncRetryHandler { get; set; }

        internal bool HandlesException(Exception exception) =>
            this.RetryPolicy?.Invoke(exception) ?? false;

        internal bool HasMaxAttemptsReached(int currentAttempt) =>
            currentAttempt > this.RetryCount;

        internal void RaiseRetryEvent(Exception exception, AttemptContext context) =>
            this.RetryHandler?.Invoke(exception, context);

        internal async Task RaiseRetryEventAsync(Exception exception, AttemptContext context, CancellationToken token)
        {
            this.RetryHandler?.Invoke(exception, context);

            if (this.AsyncRetryHandler == null)
                return;

            await this.AsyncRetryHandler(exception, context, token).ConfigureAwait(context.ExecutionContext.BotPolicyConfiguration.ContinueOnCapturedContext);
        }

        internal TimeSpan CalculateNextDelay(int currentAttempt, Exception exception) =>
            this.RetryStrategy(currentAttempt, exception);
    }
}
