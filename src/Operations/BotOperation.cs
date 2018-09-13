using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Operations
{
    internal class BotOperation : IBotOperation
    {
        private readonly Action<ExecutionContext, CancellationToken> operation;

        public BotOperation(Action<ExecutionContext, CancellationToken> operation)
        {
            this.operation = operation;
        }

        public void Execute(ExecutionContext context, CancellationToken token) =>
            this.operation(context, token);
    }

    internal class AsyncBotOperation : IAsyncBotOperation
    {
        private readonly Action<ExecutionContext, CancellationToken> operation;
        private readonly Func<ExecutionContext, CancellationToken, Task> taskOperation;

        public AsyncBotOperation(Action<ExecutionContext, CancellationToken> operation)
        {
            this.operation = operation;
        }

        public AsyncBotOperation(Func<ExecutionContext, CancellationToken, Task> operation)
        {
            this.taskOperation = operation;
        }

        public Task ExecuteAsync(ExecutionContext context, CancellationToken token)
        {
            if (this.taskOperation != null)
                return this.taskOperation(context, token);

            this.operation(context, token);
            return Constants.CompletedTask;
        }
    }
}
