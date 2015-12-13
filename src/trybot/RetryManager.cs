using Ronin.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Interfaces;
using Trybot.Model;
using Trybot.Strategy;

namespace Trybot
{
    /// <summary>
    /// Represents a <see cref="IRetryManager"/> implementation.
    /// </summary>
    public class RetryManager : IRetryManager
    {
        private readonly IRetryPolicy retryPolicy;

        /// <summary>
        /// Constructs a <see cref="RetryStartegy"/>
        /// </summary>
        /// <param name="retryPolicy">A <see cref="IRetryPolicy"/> implementation.</param>
        public RetryManager(IRetryPolicy retryPolicy)
        {
            Shield.EnsureNotNull(retryPolicy, nameof(retryPolicy));

            this.retryPolicy = retryPolicy;
        }

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="action">The operation to be retried.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <returns>The Task of the operation.</returns>
        public async Task ExecuteAsync(Action action, CancellationToken token, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(action, nameof(action));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            TryResult result;
            while (!(result = await this.TryAction(action, retryFiler)).Succeeded &&
                !retryStartegy.IsCompleted() && !token.IsCancellationRequested)
            {
                retryStartegy.CalculateNextDelay();
                this.RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(token);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="action">The operation to be retried.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <returns>The Task of the operation.</returns>
        public async Task ExecuteAsync(Action action, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(action, nameof(action));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            TryResult result;
            while (!(result = await this.TryAction(action, retryFiler)).Succeeded && !retryStartegy.IsCompleted())
            {
                retryStartegy.CalculateNextDelay();
                this.RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(CancellationToken.None);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="func">The operation to be retried.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <returns>The Task of the operation.</returns>
        public async Task ExecuteAsync(Func<Task> func, CancellationToken token, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(func, nameof(func));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler)).Succeeded && !retryStartegy.IsCompleted() && !token.IsCancellationRequested)
            {
                retryStartegy.CalculateNextDelay();
                this.RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(token);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="func">The operation to be retried.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <returns>The Task of the operation.</returns>
        public async Task ExecuteAsync(Func<Task> func, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(func, nameof(func));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler)).Succeeded && !retryStartegy.IsCompleted())
            {
                retryStartegy.CalculateNextDelay();
                this.RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(CancellationToken.None);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="func">The operation to be retried.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <param name="resultFilter">The predicate which can check the result of the operation and if it's true, the operation will be retried.</param>
        /// <returns>The Task of the operation.</returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, CancellationToken token, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null, Predicate<T> resultFilter = null)
        {
            Shield.EnsureNotNull(func, nameof(func));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler, resultFilter)).Succeeded && !retryStartegy.IsCompleted() && !token.IsCancellationRequested)
            {
                retryStartegy.CalculateNextDelay();
                this.RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(token);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
            {
                if (result.Exception != null)
                    throw result.Exception;
                else
                    return (T)result.FunctionResult;
            }
            else
                return (T)result.FunctionResult;
        }

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="func">The operation to be retried.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <param name="resultFilter">The predicate which can check the result of the operation and if it's true, the operation will be retried.</param>
        /// <returns>The Task of the operation.</returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null, Predicate<T> resultFilter = null)
        {
            Shield.EnsureNotNull(func, nameof(func));
            retryStartegy = retryStartegy ?? RetryStartegy.DefaultRetryStrategy;

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler, resultFilter)).Succeeded && !retryStartegy.IsCompleted())
            {
                retryStartegy.CalculateNextDelay();
                this.RaiseRetryOccuredEvent(onRetryOccured, retryStartegy.CurrentAttempt, retryStartegy.NextDelay);
                await retryStartegy.WaitAsync(CancellationToken.None);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
            {
                if (result.Exception != null)
                    throw result.Exception;
                else
                    return (T)result.FunctionResult;
            }
            else
                return (T)result.FunctionResult;
        }

        private async Task<TryResult> TryAction(Action action, Func<bool> retryFiler = null)
        {
            var completionSource = new TaskCompletionSource<TryResult>();

            if (retryFiler != null && retryFiler())
                completionSource.SetResult(new TryResult { Succeeded = true });

            try
            {
                action();
                completionSource.SetResult(new TryResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                completionSource.SetResult(this.retryPolicy.ShouldRetryAfter(ex)
                    ? new TryResult { Succeeded = false, Exception = ex }
                    : new TryResult { Succeeded = true, Exception = ex, ForceThrowException = true });
            }

            return await completionSource.Task;
        }

        private async Task<TryResult> TryFunction(Func<Task> func, Func<bool> retryFiler = null)
        {
            var completionSource = new TaskCompletionSource<TryResult>();

            if (retryFiler != null && retryFiler())
                completionSource.SetResult(new TryResult { Succeeded = true });

            try
            {
                await func();
                completionSource.SetResult(new TryResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                completionSource.SetResult(this.retryPolicy.ShouldRetryAfter(ex)
                    ? new TryResult { Succeeded = false, Exception = ex }
                    : new TryResult { Succeeded = true, Exception = ex, ForceThrowException = true });
            }

            return await completionSource.Task;
        }

        private async Task<TryResult> TryFunction<T>(Func<Task<T>> func, Func<bool> retryFiler = null, Predicate<T> resultFilter = null)
        {
            var completionSource = new TaskCompletionSource<TryResult>();

            if (retryFiler != null && retryFiler())
                completionSource.SetResult(new TryResult { Succeeded = true });

            try
            {
                var result = await func();
                if (resultFilter != null)
                {
                    completionSource.SetResult(resultFilter(result)
                        ? new TryResult { Succeeded = false, FunctionResult = result }
                        : new TryResult { Succeeded = true, FunctionResult = result });
                }
                else
                    completionSource.SetResult(new TryResult { Succeeded = true, FunctionResult = result });
            }
            catch (Exception ex)
            {
                completionSource.SetResult(this.retryPolicy.ShouldRetryAfter(ex)
                    ? new TryResult { Succeeded = false, Exception = ex }
                    : new TryResult { Succeeded = true, Exception = ex, ForceThrowException = true });
            }

            return await completionSource.Task;
        }

        private void RaiseRetryOccuredEvent(Action<int, TimeSpan> onRetryOccured, int attempt, TimeSpan nextDelay)
        {
            if (onRetryOccured != null)
                onRetryOccured(attempt + 1, nextDelay);
        }
    }
}
