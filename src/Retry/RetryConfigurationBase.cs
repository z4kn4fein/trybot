using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Model;

namespace Trybot.Retry
{
    public class RetryConfigurationBase
    {
        protected int RetryCount { get; set; } = 1;
        protected Func<int, TimeSpan> RetryStrategy { get; set; } = attempt => TimeSpan.Zero;
        protected Func<Exception, bool> RetryPolicy { get; set; }
        protected Action<Exception, AttemptContext> RetryHandler { get; set; }
        protected Func<Exception, AttemptContext, CancellationToken, Task> AsyncRetryHandler { get; set; }

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

        internal TimeSpan CalculateNextDelay(int currentAttempt) =>
            this.RetryStrategy(currentAttempt);
    }
}
