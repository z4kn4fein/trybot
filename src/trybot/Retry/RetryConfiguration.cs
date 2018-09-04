using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Model;

namespace Trybot.Retry
{
    public class RetryConfigurationBase
    {
        protected int RetryCount { get; set; } = 1;
        protected Func<int, TimeSpan> RetryStrategy { get; set; }
        protected Func<Exception, bool> RetryPolicy { get; set; }
        protected Action<Exception, AttemptContext> RetryHandler { get; set; }
        protected Func<Exception, AttemptContext, CancellationToken, Task> AsyncRetryHandler { get; set; }

        internal bool HandlesException(Exception exception) =>
            this.RetryPolicy?.Invoke(exception) ?? false;

        internal bool HasMaxAttemptsReached(int currentAttempt) =>
            currentAttempt >= this.RetryCount;

        internal void RaiseRetryEvent(Exception exception, AttemptContext context) =>
            this.RetryHandler?.Invoke(exception, context);

        internal async Task RaiseRetryEventAsync(Exception exception, AttemptContext context, CancellationToken token)
        {
            if (this.AsyncRetryHandler == null)
                return;

            await this.AsyncRetryHandler(exception, context, token).ConfigureAwait(context.ExecutionContext.BotPolicyConfiguration.ContinueOnCapturedContext);
        }

        internal TimeSpan CalculateNextDelay(int currentAttempt) =>
            this.RetryStrategy(currentAttempt);
    }

    public class RetryConfiguration : RetryConfigurationBase, IRetryConfiguration<RetryConfiguration>
    {
        public RetryConfiguration WithMaxAttemptCount(int numOfAttempts)
        {
            this.RetryCount = numOfAttempts;
            return this;
        }

        public RetryConfiguration RetryIndefinitely()
        {
            this.RetryCount = int.MaxValue;
            return this;
        }

        public RetryConfiguration WaitBetweenAttempts(Func<int, TimeSpan> retryStrategy)
        {
            this.RetryStrategy = retryStrategy;
            return this;
        }

        public RetryConfiguration WhenExceptionOccurs(Func<Exception, bool> retryPolicy)
        {
            this.RetryPolicy = retryPolicy;
            return this;
        }

        public RetryConfiguration OnRetry(Action<Exception, AttemptContext> onRetryAction)
        {
            this.RetryHandler = onRetryAction;
            return this;
        }

        public RetryConfiguration OnRetryAsync(Func<Exception, AttemptContext, CancellationToken, Task> onRetryFunc)
        {
            this.AsyncRetryHandler = onRetryFunc;
            return this;
        }
    }

    public class RetryConfiguration<TResult> : RetryConfigurationBase, IRetryConfiguration<RetryConfiguration<TResult>, TResult>
    {
        private Func<int, TResult, TimeSpan> resultRetryStrategy;
        private Func<TResult, bool> resultPolicy;
        private Action<TResult, Exception, AttemptContext> retryHandlerWithResult;
        private Func<TResult, Exception, AttemptContext, CancellationToken, Task> asyncRetryHandlerWithResult;

        public RetryConfiguration<TResult> WithMaxAttemptCount(int numOfAttempts)
        {
            this.RetryCount = numOfAttempts;
            return this;
        }

        public RetryConfiguration<TResult> RetryIndefinitely()
        {
            this.RetryCount = int.MaxValue;
            return this;
        }

        public RetryConfiguration<TResult> WaitBetweenAttempts(Func<int, TimeSpan> retryStrategy)
        {
            this.RetryStrategy = retryStrategy;
            return this;
        }

        public RetryConfiguration<TResult> WhenExceptionOccurs(Func<Exception, bool> retryPolicy)
        {
            this.RetryPolicy = retryPolicy;
            return this;
        }

        public RetryConfiguration<TResult> OnRetry(Action<Exception, AttemptContext> onRetryAction)
        {
            this.RetryHandler = onRetryAction;
            return this;
        }

        public RetryConfiguration<TResult> OnRetryAsync(Func<Exception, AttemptContext, CancellationToken, Task> onRetryFunc)
        {
            this.AsyncRetryHandler = onRetryFunc;
            return this;
        }

        public RetryConfiguration<TResult> WaitBetweenAttempts(Func<int, TResult, TimeSpan> resultRetryStrategy)
        {
            this.resultRetryStrategy = resultRetryStrategy;
            return this;
        }

        public RetryConfiguration<TResult> WhenResultIs(Func<TResult, bool> resultPolicy)
        {
            this.resultPolicy = resultPolicy;
            return this;
        }

        public RetryConfiguration<TResult> OnRetry(Action<TResult, Exception, AttemptContext> onRetryAction)
        {
            this.retryHandlerWithResult = onRetryAction;
            return this;
        }

        public RetryConfiguration<TResult> OnRetryAsync(Func<TResult, Exception, AttemptContext, CancellationToken, Task> onRetryFunc)
        {
            this.asyncRetryHandlerWithResult = onRetryFunc;
            return this;
        }

        internal bool AcceptsResult(TResult result) =>
            !this.resultPolicy(result);

        internal TimeSpan CalculateNextDelay(int currentAttempt, TResult result) =>
            this.resultRetryStrategy != null
                ? this.resultRetryStrategy(currentAttempt, result)
                : this.RetryStrategy(currentAttempt);

        internal void RaiseRetryEvent(TResult result, Exception exception, AttemptContext context) =>
            this.retryHandlerWithResult?.Invoke(result, exception, context);

        internal async Task RaiseRetryEventAsync(TResult result, Exception exception, AttemptContext context, CancellationToken token)
        {
            if (this.asyncRetryHandlerWithResult == null)
                return;

            await this.asyncRetryHandlerWithResult(result, exception, context, token).ConfigureAwait(context.ExecutionContext.BotPolicyConfiguration.ContinueOnCapturedContext);
        }
    }
}
