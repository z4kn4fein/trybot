using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    public interface IBotPolicy
    {
        void Execute(Action<ExecutionContext, CancellationToken> action, CancellationToken token);

        Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action, CancellationToken token);

        Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, CancellationToken token);
    }

    public interface IBotPolicy<TResult>
    {
        TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token);

        Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token);

        Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, CancellationToken token);
    }
}
