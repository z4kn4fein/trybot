using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Fallback
{
    /// <summary>
    /// Represents the configuration of the fallback bot.
    /// </summary>
    public class FallbackConfiguration : FallbackConfigurationBase, IFallbackConfiguration<FallbackConfiguration>
    {
        /// <inheritdoc />
        public FallbackConfiguration WhenExceptionOccurs(Func<Exception, bool> fallbackPolicy)
        {
            base.FallbackPolicy = fallbackPolicy;
            return this;
        }

        /// <inheritdoc />
        public FallbackConfiguration OnFallback(Action<Exception, ExecutionContext> onFallbackAction)
        {
            base.FallbackHandler = onFallbackAction;
            return this;
        }

        /// <inheritdoc />
        public FallbackConfiguration OnFallbackAsync(Func<Exception, ExecutionContext, CancellationToken, Task> onFallbackFunc)
        {
            base.AsyncFallbackHandler = onFallbackFunc;
            return this;
        }
    }

    /// <summary>
    /// Represents the configuration of the fallback bot.
    /// </summary>
    public class FallbackConfiguration<TResult> : FallbackConfigurationBase, IFallbackConfiguration<FallbackConfiguration<TResult>, TResult>
    {
        internal Func<TResult, bool> ResultPolicy { get; set; }

        internal Func<TResult, Exception, ExecutionContext, TResult> FallbackHandlerWithResult { get; set; }

        internal Func<TResult, Exception, ExecutionContext, CancellationToken, Task<TResult>> AsyncFallbackHandlerWithResult { get; set; }

        /// <inheritdoc />
        public FallbackConfiguration<TResult> WhenExceptionOccurs(Func<Exception, bool> fallbackPolicy)
        {
            base.FallbackPolicy = fallbackPolicy;
            return this;
        }

        /// <inheritdoc />
        public FallbackConfiguration<TResult> OnFallback(Action<Exception, ExecutionContext> onFallbackAction)
        {
            base.FallbackHandler = onFallbackAction;
            return this;
        }

        /// <inheritdoc />
        public FallbackConfiguration<TResult> OnFallbackAsync(Func<Exception, ExecutionContext, CancellationToken, Task> onFallbackFunc)
        {
            base.AsyncFallbackHandler = onFallbackFunc;
            return this;
        }

        /// <inheritdoc />
        public FallbackConfiguration<TResult> WhenResultIs(Func<TResult, bool> resultPolicy)
        {
            this.ResultPolicy = resultPolicy;
            return this;
        }

        /// <inheritdoc />
        public FallbackConfiguration<TResult> OnFallback(Func<TResult, Exception, ExecutionContext, TResult> onFallbackAction)
        {
            this.FallbackHandlerWithResult = onFallbackAction;
            return this;
        }

        /// <inheritdoc />
        public FallbackConfiguration<TResult> OnFallbackAsync(Func<TResult, Exception, ExecutionContext, CancellationToken, Task<TResult>> onFallbackFunc)
        {
            this.AsyncFallbackHandlerWithResult = onFallbackFunc;
            return this;
        }

        internal TResult RaiseFallbackEvent(TResult result, Exception exception, ExecutionContext context)
        {
            base.RaiseFallbackEvent(exception, context);

            return this.FallbackHandlerWithResult == null ? result : this.FallbackHandlerWithResult(result, exception, context);
        }

        internal async Task<TResult> RaiseFallbackEventAsync(TResult result, Exception exception, ExecutionContext context, CancellationToken token)
        {
            base.RaiseFallbackEvent(exception, context);
            await base.RaiseFallbackEventAsync(exception, context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

            if (this.FallbackHandlerWithResult != null)
                return this.FallbackHandlerWithResult(result, exception, context);

            if (this.AsyncFallbackHandlerWithResult == null)
                return result;

            return await this.AsyncFallbackHandlerWithResult(result, exception, context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
        }

        internal bool AcceptsResult(TResult result) =>
            !this.ResultPolicy?.Invoke(result) ?? true;
    }
}
