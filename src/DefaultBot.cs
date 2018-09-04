using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    internal class DefaultBot : Bot
    {
        public override void Execute(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token) =>
            action(context, token);

        public override Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token)
        {
            action(context, token);
            return Task.FromResult<object>(null);
        }

        public override Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, ExecutionContext context, CancellationToken token) =>
            operation(context, token);
    }

    internal class DefaultBot<TResult> : Bot<TResult>
    {
        public override TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token) =>
            operation(context, token);

        public override Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token)
        {
            var result = operation(context, token);
            return Task.FromResult(result);
        }

        public override Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, ExecutionContext context, CancellationToken token) =>
            operation(context, token);
    }
}
