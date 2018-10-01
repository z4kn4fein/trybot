using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Operations
{
    internal class ParameterlessBotOperation<TResult> : IBotOperation<TResult>
    {
        private readonly Func<TResult> operation;

        public ParameterlessBotOperation(Func<TResult> operation)
        {
            this.operation = operation;
        }

        public TResult Execute(ExecutionContext context, CancellationToken token) =>
            this.operation();
    }

    internal class AsyncParameterlessBotOperation<TResult> : IAsyncBotOperation<TResult>
    {
        private readonly Func<TResult> operation;
        private readonly Func<Task<TResult>> taskOperation;

        public AsyncParameterlessBotOperation(Func<TResult> operation)
        {
            this.operation = operation;
        }

        public AsyncParameterlessBotOperation(Func<Task<TResult>> operation)
        {
            this.taskOperation = operation;
        }

        public Task<TResult> ExecuteAsync(ExecutionContext context, CancellationToken token) =>
            this.taskOperation != null ? this.taskOperation() : Task.FromResult(this.operation());
    }
}
