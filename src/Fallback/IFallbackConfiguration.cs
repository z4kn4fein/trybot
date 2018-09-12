using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Fallback
{
    public interface IFallbackConfiguration<out TConfiguration>
    {
        TConfiguration WhenExceptionOccurs(Func<Exception, bool> fallbackPolicy);

        TConfiguration OnFallback(Action<Exception, ExecutionContext> onFallbackAction);

        TConfiguration OnFallbackAsync(Func<Exception, ExecutionContext, CancellationToken, Task> onFallbackFunc);
    }

    public interface IFallbackConfiguration<out TConfiguration, TResult> : IFallbackConfiguration<TConfiguration>
    {
        TConfiguration WhenResultIs(Func<TResult, bool> resultPolicy);

        TConfiguration OnFallback(Func<TResult, Exception, ExecutionContext, TResult> onFallbackAction);

        TConfiguration OnFallbackAsync(Func<TResult, Exception, ExecutionContext, CancellationToken, Task<TResult>> onFallbackFunc);
    }
}
