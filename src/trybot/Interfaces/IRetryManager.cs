using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Strategy;

namespace Trybot.Interfaces
{
    public interface IRetryManager
    {
        Task ExecuteAsync(Action action, CancellationToken token, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null);
        Task ExecuteAsync(Action action, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null);
        Task ExecuteAsync(Func<Task> func, CancellationToken token, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null);
        Task ExecuteAsync(Func<Task> func, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null);

        Task<T> ExecuteAsync<T>(Func<Task<T>> func, CancellationToken token, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null, Predicate<T> resultFilter = null);
        Task<T> ExecuteAsync<T>(Func<Task<T>> func, Action<int, TimeSpan> onRetryOccured = null, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null, Predicate<T> resultFilter = null);
    }
}
