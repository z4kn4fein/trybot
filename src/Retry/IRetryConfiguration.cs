using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Model;

namespace Trybot.Retry
{
    /// <summary>
    /// Represents the fluent api of the retry configuration.
    /// </summary>
    public interface IRetryConfiguration<out TConfiguration>
    {
        /// <summary>
        /// Sets the maximum number of retry attempts.
        /// </summary>
        /// <param name="numOfAttempts"></param>
        /// <returns>Itself because of the fluent api.</returns>
        TConfiguration WithMaxAttemptCount(int numOfAttempts);

        /// <summary>
        /// The given operation will be reexecuted indefinitely.
        /// </summary>
        /// <returns>Itself because of the fluent api.</returns>
        TConfiguration RetryIndefinitely();

        /// <summary>
        /// Sets the delegate which will be used to calculate the wait time between the retry attempts.
        /// </summary>
        /// <param name="retryStrategy">The time calculator delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.WaitBetweenAttempts((currentAttempt, exception) => TimeSpan.FromSeconds(2))</code></example>
        TConfiguration WaitBetweenAttempts(Func<int, Exception, TimeSpan> retryStrategy);

        /// <summary>
        /// Sets the delegate which will be used to determine whether the given operation should be
        /// re-executed or not when a specific exception occurs.
        /// </summary>
        /// <param name="retryPolicy">The determination delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.WhenExceptionOccurs(exception => exception is HttpRequestException)</code></example>
        TConfiguration WhenExceptionOccurs(Func<Exception, bool> retryPolicy);

        /// <summary>
        /// Sets the delegate which will be invoked when the given operation is being re-executed.
        /// </summary>
        /// <param name="onRetryAction">The action to be invoked on a re-execution.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetry((exception, attemptContext) => Console.WriteLine($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}"))</code></example>
        TConfiguration OnRetry(Action<Exception, AttemptContext> onRetryAction);

        /// <summary>
        /// Sets the delegate which will be invoked when the given operation is healed from re-execution.
        /// </summary>
        /// <param name="onRetrySucceededAction">The delegate action.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetrySucceeded((attemptContext) => Console.WriteLine($"Retry succeeded after {attemptContext.CurrentAttempt} re-execution."))</code></example>
        TConfiguration OnRetrySucceeded(Action<AttemptContext> onRetrySucceededAction);

        /// <summary>
        /// Sets the delegate which will be invoked asynchronously when the given operation is being re-executed.
        /// </summary>
        /// <param name="onRetryFunc">The asynchronous action to be invoked on a re-execution.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetryAsync(async (exception, attemptContext, token) => await LogAsync($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}", token))</code></example>
        TConfiguration OnRetryAsync(Func<Exception, AttemptContext, CancellationToken, Task> onRetryFunc);

        /// <summary>
        /// Sets the delegate which will be invoked asynchronously when the given operation is healed from re-execution.
        /// </summary>
        /// <param name="onRetrySucceededFunc">The asynchronous action.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetryAsync(async (exception, attemptContext, token) => await LogAsync($"Retry succeeded after {attemptContext.CurrentAttempt} re-execution.", token))</code></example>
        TConfiguration OnRetrySucceededAsync(Func<AttemptContext, CancellationToken, Task> onRetrySucceededFunc);
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents the fluent api of the retry configuration.
    /// </summary>
    public interface IRetryConfiguration<out TConfiguration, out TResult> : IRetryConfiguration<TConfiguration>
    {
        /// <summary>
        /// Sets the delegate which will be used to calculate the wait time between the retry attempts.
        /// </summary>
        /// <param name="resultRetryStrategy">The time calculator delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.WaitBetweenAttempts((currentAttempt, result, exception) => TimeSpan.FromSeconds(2))</code></example>
        TConfiguration WaitBetweenAttempts(Func<int, Exception, TResult, TimeSpan> resultRetryStrategy);

        /// <summary>
        /// Sets the delegate which will be used to determine whether the given operation should be
        /// re-executed or not based on its return value.
        /// </summary>
        /// <param name="resultPolicy">The determination delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.WhenResultIs(result => result != OperationResult.Ok)</code></example>
        TConfiguration WhenResultIs(Func<TResult, bool> resultPolicy);

        /// <summary>
        /// Sets the delegate which will be invoked when the given operation is being re-executed.
        /// </summary>
        /// <param name="onRetryAction">The action to be invoked on a re-execution.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetry((exception, result, attemptContext) => Console.WriteLine($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}, result: {result}"))</code></example>
        TConfiguration OnRetry(Action<TResult, Exception, AttemptContext> onRetryAction);

        /// <summary>
        /// Sets the delegate which will be invoked when the given operation is healed from re-execution.
        /// </summary>
        /// <param name="onRetrySucceededAction">The action.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetry((exception, result, attemptContext) => Console.WriteLine($"Retry succeeded after {attemptContext.CurrentAttempt} re-execution."))</code></example>
        TConfiguration OnRetrySucceeded(Action<TResult, AttemptContext> onRetrySucceededAction);

        /// <summary>
        /// Sets the delegate which will be invoked asynchronously when the given operation is being re-executed.
        /// </summary>
        /// <param name="onRetryFunc">The asynchronous action to be invoked on a re-execution.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetryAsync(async (exception, attemptContext, token) => await LogAsync($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}, result: {result}", token))</code></example>
        TConfiguration OnRetryAsync(Func<TResult, Exception, AttemptContext, CancellationToken, Task> onRetryFunc);

        /// <summary>
        /// Sets the delegate which will be invoked asynchronously when the given operation is healed from re-execution.
        /// </summary>
        /// <param name="onRetryFunc">The asynchronous action.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnRetryAsync(async (exception, attemptContext, token) => await LogAsync($"Retry succeeded after {attemptContext.CurrentAttempt} re-execution.", token))</code></example>
        TConfiguration OnRetrySucceededAsync(Func<TResult, AttemptContext, CancellationToken, Task> onRetryFunc);
    }
}
