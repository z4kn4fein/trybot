using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Interfaces;
using Trybot.Model;
using Trybot.Strategy;
using Trybot.Utils;

namespace Trybot
{
    /// <summary>
    /// Represents a <see cref="IRetryManager"/> implementation.
    /// </summary>
    [Obsolete("This component is not maintained anymore, check the new api: https://github.com/z4kn4fein/trybot")]
    public class RetryManager : IRetryManager
    {
        private static readonly IRetryPolicy DefaultPolicy = new DefaultRetryPolicy();

        private class DefaultRetryPolicy : IRetryPolicy
        {
            public bool ShouldRetryAfter(Exception exception) => true;
        }

        private readonly IRetryPolicy retryPolicyField;

        /// <summary>
        /// Constructs a <see cref="RetryStartegy"/>
        /// </summary>
        /// <param name="retryPolicy">A <see cref="IRetryPolicy"/> implementation.</param>
        public RetryManager(IRetryPolicy retryPolicy = null)
        {
            this.retryPolicyField = retryPolicy ?? DefaultPolicy;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(Action action, Func<Exception, bool> retryPolicy = null, CancellationToken token = default(CancellationToken), Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFilter = null)
        {
            Shield.EnsureNotNull(action, nameof(action));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            var result = TryResult.Default;
            while (!token.IsCancellationRequested && !retryStartegy.IsCompleted() && !(result = await this.TryAction(action, retryPolicy, retryFilter)).Succeeded)
            {
                retryStartegy.CalculateNextDelay();
                RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(token);
            }

            if (!result.Succeeded || result.Succeeded && result.ForceThrowException)
                if (result.Exception != null)
                    throw result.Exception;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(Func<Task> func, Func<Exception, bool> retryPolicy = null, CancellationToken token = default(CancellationToken), Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFilter = null)
        {
            Shield.EnsureNotNull(func, nameof(func));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            var result = TryResult.Default;
            while (!token.IsCancellationRequested && !retryStartegy.IsCompleted() && !(result = await this.TryFunction(func, retryPolicy, retryFilter)).Succeeded)
            {
                retryStartegy.CalculateNextDelay();
                RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(token);
            }

            if (!result.Succeeded || result.Succeeded && result.ForceThrowException)
                if (result.Exception != null)
                    throw result.Exception;
        }

        /// <inheritdoc />
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, Func<Exception, bool> retryPolicy = null, CancellationToken token = default(CancellationToken), Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFilter = null, Predicate<T> resultFilter = null)
        {
            Shield.EnsureNotNull(func, nameof(func));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            var result = TryResult.Default;
            while (!token.IsCancellationRequested && !retryStartegy.IsCompleted() && !(result = await this.TryFunction(func, retryPolicy, retryFilter, resultFilter)).Succeeded)
            {
                retryStartegy.CalculateNextDelay();
                RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(token);
            }

            if (result.Succeeded && (!result.Succeeded || !result.ForceThrowException))
                return (T)result.FunctionResult;

            if (result.Exception != null)
                throw result.Exception;

            return (T)result.FunctionResult;
        }

        private async Task<TryResult> TryAction(Action action, Func<Exception, bool> retryPolicy = null, Func<bool> retryFilter = null)
        {
            var completionSource = new TaskCompletionSource<TryResult>();

            try
            {
                action();
                this.HandleRetryFilter(completionSource, retryFilter);
            }
            catch (Exception ex)
            {
                var shouldRetry = retryPolicy?.Invoke(ex) ?? this.retryPolicyField.ShouldRetryAfter(ex);
                completionSource.SetResult(shouldRetry
                    ? new TryResult { Succeeded = false, Exception = ex }
                    : new TryResult { Succeeded = true, Exception = ex, ForceThrowException = true });
            }

            return await completionSource.Task;
        }

        private async Task<TryResult> TryFunction(Func<Task> func, Func<Exception, bool> retryPolicy = null, Func<bool> retryFilter = null)
        {
            var completionSource = new TaskCompletionSource<TryResult>();

            try
            {
                await func();
                this.HandleRetryFilter(completionSource, retryFilter);
            }
            catch (Exception ex)
            {
                var shouldRetry = retryPolicy?.Invoke(ex) ?? this.retryPolicyField.ShouldRetryAfter(ex);
                completionSource.SetResult(shouldRetry
                    ? new TryResult { Succeeded = false, Exception = ex }
                    : new TryResult { Succeeded = true, Exception = ex, ForceThrowException = true });
            }

            return await completionSource.Task;
        }

        private async Task<TryResult> TryFunction<T>(Func<Task<T>> func, Func<Exception, bool> retryPolicy = null, Func<bool> retryFilter = null, Predicate<T> resultFilter = null)
        {
            var completionSource = new TaskCompletionSource<TryResult>();

            try
            {
                var result = await func();
                if (resultFilter != null)
                {
                    if (resultFilter(result))
                        completionSource.SetResult(new TryResult { Succeeded = false, FunctionResult = result });
                    else
                        this.HandleRetryFilterWithResult(completionSource, result, retryFilter);
                }
                else
                    this.HandleRetryFilterWithResult(completionSource, result, retryFilter);
            }
            catch (Exception ex)
            {
                var shouldRetry = retryPolicy?.Invoke(ex) ?? this.retryPolicyField.ShouldRetryAfter(ex);
                completionSource.SetResult(shouldRetry
                    ? new TryResult { Succeeded = false, Exception = ex }
                    : new TryResult { Succeeded = true, Exception = ex, ForceThrowException = true });
            }

            return await completionSource.Task;
        }

        private void HandleRetryFilter(TaskCompletionSource<TryResult> completionSource, Func<bool> retryFilter = null)
        {
            if (retryFilter != null)
            {
                completionSource.SetResult(retryFilter()
                    ? new TryResult { Succeeded = false }
                    : new TryResult { Succeeded = true });
            }
            else
                completionSource.SetResult(new TryResult { Succeeded = true });
        }

        private void HandleRetryFilterWithResult(TaskCompletionSource<TryResult> completionSource, object result, Func<bool> retryFilter = null)
        {
            if (retryFilter != null)
            {
                completionSource.SetResult(retryFilter()
                    ? new TryResult { Succeeded = false, FunctionResult = result }
                    : new TryResult { Succeeded = true, FunctionResult = result });
            }
            else
                completionSource.SetResult(new TryResult { Succeeded = true, FunctionResult = result });
        }

        private static void RaiseRetryOccuredEvent(Action<int, TimeSpan> onRetryOccured, int attempt, TimeSpan nextDelay)
        {
            onRetryOccured?.Invoke(attempt + 1, nextDelay);
        }
    }
}
