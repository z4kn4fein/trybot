using Ronin.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Interfaces;
using Trybot.Model;
using Trybot.Strategy;

namespace Trybot
{
    public partial class RetryManager<TRetryPolicy> : IRetryManager<TRetryPolicy> where TRetryPolicy : class, IRetryPolicy
    {
        private readonly TRetryPolicy retryPolicy;

        public RetryManager(TRetryPolicy retryPolicy)
        {
            Shield.EnsureNotNull(retryPolicy);

            this.retryPolicy = retryPolicy;
        }

        public async Task ExecuteAsync(Action action, CancellationToken token, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(action);
            retryStartegy = retryStartegy ?? RetryStartegy.GetDefaultPolicy();

            TryResult result;
            while (!(result = await this.TryAction(action, retryFiler)).Succeeded &&
                !retryStartegy.IsCompleted() && !token.IsCancellationRequested)
            {
                await retryStartegy.WaitAsync(token);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        public async Task ExecuteAsync(Action action, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(action);
            retryStartegy = retryStartegy ?? RetryStartegy.GetDefaultPolicy();

            TryResult result;
            while (!(result = await this.TryAction(action, retryFiler)).Succeeded && !retryStartegy.IsCompleted())
            {
                await retryStartegy.WaitAsync(CancellationToken.None);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        public async Task ExecuteAsync(Func<Task> func, CancellationToken token, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(func);
            retryStartegy = retryStartegy ?? RetryStartegy.GetDefaultPolicy();

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler)).Succeeded && !retryStartegy.IsCompleted() && !token.IsCancellationRequested)
            {
                await retryStartegy.WaitAsync(token);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        public async Task ExecuteAsync(Func<Task> func, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null)
        {
            Shield.EnsureNotNull(func);
            retryStartegy = retryStartegy ?? RetryStartegy.GetDefaultPolicy();

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler)).Succeeded && !retryStartegy.IsCompleted())
            {
                await retryStartegy.WaitAsync(CancellationToken.None);
            }

            if (!result.Succeeded || (result.Succeeded && result.ForceThrowException))
                if (result.Exception != null)
                    throw result.Exception;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, CancellationToken token, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null, Predicate<T> resultFilter = null)
        {
            Shield.EnsureNotNull(func);
            retryStartegy = retryStartegy ?? RetryStartegy.GetDefaultPolicy();

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler, resultFilter)).Succeeded && !retryStartegy.IsCompleted() && !token.IsCancellationRequested)
            {
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

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, RetryStartegy retryStartegy = null, Func<bool> retryFiler = null, Predicate<T> resultFilter = null)
        {
            Shield.EnsureNotNull(func);
            retryStartegy = retryStartegy ?? RetryStartegy.GetDefaultPolicy();

            TryResult result;
            while (!(result = await this.TryFunction(func, retryFiler, resultFilter)).Succeeded && !retryStartegy.IsCompleted())
            {
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
    }
}
