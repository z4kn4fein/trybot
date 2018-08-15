using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    public interface IPolicyExecutor
    {
        void Execute(object policyIdentifier, Action<ExecutionContext, CancellationToken> action, CancellationToken token);

        TResult Execute<TResult>(object policyIdentifier, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token);

        Task ExecuteAsync(object policyIdentifier, Action<ExecutionContext, CancellationToken> action, CancellationToken token);

        Task<TResult> ExecuteAsync<TResult>(object policyIdentifier, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token);

        Task<TResult> ExecuteAsync<TResult>(object policyIdentifier, Func<ExecutionContext, CancellationToken, Task<TResult>> operation, CancellationToken token);
    }
}
