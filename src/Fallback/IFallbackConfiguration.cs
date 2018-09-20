using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Fallback
{
    /// <summary>
    /// Represents the fluent api of the fallback configuration.
    /// </summary>
    public interface IFallbackConfiguration<out TConfiguration>
    {
        /// <summary>
        /// Sets the delegate which will be used to determine whether the given fallback operation should be
        /// executed or not when a specific exception occurs.
        /// </summary>
        /// <param name="fallbackPolicy">The determination delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.WhenExceptionOccurs(exception => exception is HttpRequestException)</code></example>
        TConfiguration WhenExceptionOccurs(Func<Exception, bool> fallbackPolicy);

        /// <summary>
        /// Sets the delegate which will be invoked when the fallback operation is being executed.
        /// </summary>
        /// <param name="onFallbackAction">The action to be invoked on a fallback.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnFallback((exception, context) => fallbackAction())</code></example>
        TConfiguration OnFallback(Action<Exception, ExecutionContext> onFallbackAction);

        /// <summary>
        /// Sets the delegate which will be invoked asynchronously when the fallback operation is being executed.
        /// </summary>
        /// <param name="onFallbackFunc">The asynchronous action to be invoked on a fallback.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnFallbackAsync(async (exception, context) => await fallbackActionAsync())</code></example>
        TConfiguration OnFallbackAsync(Func<Exception, ExecutionContext, CancellationToken, Task> onFallbackFunc);
    }

    /// <summary>
    /// Represents the fluent api of the fallback configuration.
    /// </summary>
    public interface IFallbackConfiguration<out TConfiguration, TResult> : IFallbackConfiguration<TConfiguration>
    {
        /// <summary>
        /// Sets the delegate which will be used to determine whether the given fallback operation should be
        /// executed or not based on the originals return value.
        /// </summary>
        /// <param name="resultPolicy">The determination delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.WhenResultIs(result => result != OperationResult.Ok)</code></example>
        TConfiguration WhenResultIs(Func<TResult, bool> resultPolicy);

        /// <summary>
        /// Sets the delegate which will be invoked when the fallback operation is being executed.
        /// </summary>
        /// <param name="onFallbackAction">The action to be invoked on a fallback.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnFallback((result, exception, context) => fallbackAction())</code></example>
        TConfiguration OnFallback(Func<TResult, Exception, ExecutionContext, TResult> onFallbackAction);

        /// <summary>
        /// Sets the delegate which will be invoked asynchronously when the fallback operation is being executed.
        /// </summary>
        /// <param name="onFallbackFunc">The asynchronous action to be invoked on a fallback.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnFallbackAsync(async (result, exception, context) => await fallbackActionAsync())</code></example>
        TConfiguration OnFallbackAsync(Func<TResult, Exception, ExecutionContext, CancellationToken, Task<TResult>> onFallbackFunc);
    }
}
