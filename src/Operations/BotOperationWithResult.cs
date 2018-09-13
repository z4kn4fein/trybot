using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Operations
{
    internal class BotOperation<TResult> : IBotOperation<TResult>
    {
        private readonly Func<ExecutionContext, CancellationToken, TResult> operation;

        public BotOperation(Func<ExecutionContext, CancellationToken, TResult> operation)
        {
            this.operation = operation;
        }

        public TResult Execute(ExecutionContext context, CancellationToken token) =>
            this.operation(context, token);
    }

    internal class AsyncBotOperation<TResult> : IAsyncBotOperation<TResult>
    {
        private readonly Func<ExecutionContext, CancellationToken, TResult> operation;
        private readonly Func<ExecutionContext, CancellationToken, Task<TResult>> taskOperation;

        public AsyncBotOperation(Func<ExecutionContext, CancellationToken, TResult> operation)
        {
            this.operation = operation;
        }

        public AsyncBotOperation(Func<ExecutionContext, CancellationToken, Task<TResult>> operation)
        {
            this.taskOperation = operation;
        }

        public Task<TResult> ExecuteAsync(ExecutionContext context, CancellationToken token) =>
            this.taskOperation != null ? this.taskOperation(context, token) : Task.FromResult(this.operation(context, token));
    }
}
