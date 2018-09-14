using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Model;

namespace Trybot.Retry
{
    public interface IRetryConfiguration<out TConfiguration>
    {
        TConfiguration WithMaxAttemptCount(int numOfAttempts);

        TConfiguration RetryIndefinitely();

        TConfiguration WaitBetweenAttempts(Func<int, Exception, TimeSpan> retryStrategy);

        TConfiguration WhenExceptionOccurs(Func<Exception, bool> retryPolicy);

        TConfiguration OnRetry(Action<Exception, AttemptContext> onRetryAction);

        TConfiguration OnRetryAsync(Func<Exception, AttemptContext, CancellationToken, Task> onRetryFunc);
    }

    public interface IRetryConfiguration<out TConfiguration, out TResult> : IRetryConfiguration<TConfiguration>
    {
        TConfiguration WaitBetweenAttempts(Func<int, Exception, TResult, TimeSpan> resultRetryStrategy);

        TConfiguration WhenResultIs(Func<TResult, bool> resultPolicy);

        TConfiguration OnRetry(Action<TResult, Exception, AttemptContext> onRetryAction);

        TConfiguration OnRetryAsync(Func<TResult, Exception, AttemptContext, CancellationToken, Task> onRetryFunc);
    }
}
