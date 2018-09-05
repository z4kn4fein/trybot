using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Fallback
{
    public class FallbackConfiguration : FallbackConfigurationBase, IFallbackConfiguration<FallbackConfiguration>
    {
        public FallbackConfiguration WhenExceptionOccurs(Func<Exception, bool> fallbackPolicy)
        {
            base.FallbackPolicy = fallbackPolicy;
            return this;
        }

        public FallbackConfiguration OnFallback(Action<Exception, ExecutionContext> onFallbackAction)
        {
            base.FallbackHandler = onFallbackAction;
            return this;
        }

        public FallbackConfiguration OnFallbackAsync(Func<Exception, ExecutionContext, CancellationToken, Task> onFallbackFunc)
        {
            base.AsyncFallbackHandler = onFallbackFunc;
            return this;
        }
    }

    public class FallbackConfiguration<TResult> : FallbackConfigurationBase, IFallbackConfiguration<FallbackConfiguration<TResult>, TResult>
    {
        internal Func<TResult, bool> ResultPolicy { get; set; }

        internal Action<TResult, Exception, ExecutionContext> FallbackHandlerWithResult { get; set; }

        internal Func<TResult, Exception, ExecutionContext, CancellationToken, Task> AsyncFallbackHandlerWithResult { get; set; }

        public FallbackConfiguration<TResult> WhenExceptionOccurs(Func<Exception, bool> fallbackPolicy)
        {
            base.FallbackPolicy = fallbackPolicy;
            return this;
        }

        public FallbackConfiguration<TResult> OnFallback(Action<Exception, ExecutionContext> onFallbackAction)
        {
            base.FallbackHandler = onFallbackAction;
            return this;
        }

        public FallbackConfiguration<TResult> OnFallbackAsync(Func<Exception, ExecutionContext, CancellationToken, Task> onFallbackFunc)
        {
            base.AsyncFallbackHandler = onFallbackFunc;
            return this;
        }

        public FallbackConfiguration<TResult> WhenResultIs(Func<TResult, bool> resultPolicy)
        {
            this.ResultPolicy = resultPolicy;
            return this;
        }

        public FallbackConfiguration<TResult> OnFallback(Action<TResult, Exception, ExecutionContext> onFallbackAction)
        {
            this.FallbackHandlerWithResult = onFallbackAction;
            return this;
        }

        public FallbackConfiguration<TResult> OnFallbackAsync(Func<TResult, Exception, ExecutionContext, CancellationToken, Task> onFallbackFunc)
        {
            this.AsyncFallbackHandlerWithResult = onFallbackFunc;
            return this;
        }

        internal void RaiseRetryEvent(TResult result, Exception exception, ExecutionContext context) =>
            this.FallbackHandlerWithResult?.Invoke(result, exception, context);

        internal async Task RaiseRetryEventAsync(TResult result, Exception exception, ExecutionContext context, CancellationToken token)
        {
            if (this.AsyncFallbackHandlerWithResult == null)
                return;

            await this.AsyncFallbackHandlerWithResult(result, exception, context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
        }

        internal bool AcceptsResult(TResult result) =>
            !this.ResultPolicy(result);
    }
}
