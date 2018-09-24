using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Strategy;

namespace Trybot.Interfaces
{
    /// <summary>
    /// Represents an interface for retry manager implementations.
    /// </summary>
    [Obsolete("This component is not maintained anymore, check the new api: https://github.com/z4kn4fein/trybot")]
    public interface IRetryManager
    {
        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="action">The operation to be retried.</param>
        /// <param name="retryPolicy">The retry policy delegate, defines when should the operation retried.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <returns>The Task of the operation.</returns>
        Task ExecuteAsync(Action action, Func<Exception, bool> retryPolicy = null, CancellationToken token = default(CancellationToken), Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null);

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="func">The operation to be retried.</param>
        /// <param name="retryPolicy">The retry policy delegate, defines when should the operation retried.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <returns>The Task of the operation.</returns>
        Task ExecuteAsync(Func<Task> func, Func<Exception, bool> retryPolicy = null, CancellationToken token = default(CancellationToken), Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null);

        /// <summary>
        /// Executes and retries an operation if it's failed.
        /// </summary>
        /// <param name="func">The operation to be retried.</param>
        /// <param name="retryPolicy">The retry policy delegate, defines when should the operation retried.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="onRetryOccured">The callback which will be called when a retry occures.</param>
        /// <param name="retryStartegy">A <see cref="RetryStartegy"/> implementation.</param>
        /// <param name="retryFiler">The predicate which will be called before every retry operation. With this parameter you can set conditional retries.</param>
        /// <param name="resultFilter">The predicate which can check the result of the operation and if it's true, the operation will be retried.</param>
        /// <returns>The Task of the operation.</returns>
        Task<T> ExecuteAsync<T>(Func<Task<T>> func, Func<Exception, bool> retryPolicy = null, CancellationToken token = default(CancellationToken), Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null, Predicate<T> resultFilter = null);
    }
}
