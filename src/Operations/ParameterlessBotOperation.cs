using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Operations
{
    internal class ParameterlessBotOperation : IBotOperation
    {
        private readonly Action operation;

        public ParameterlessBotOperation(Action operation)
        {
            this.operation = operation;
        }

        public void Execute(ExecutionContext context, CancellationToken token) =>
            this.operation();
    }

    internal class AsyncParameterlessBotOperation : IAsyncBotOperation
    {
        private readonly Action operation;
        private readonly Func<Task> taskOperation;

        public AsyncParameterlessBotOperation(Action operation)
        {
            this.operation = operation;
        }

        public AsyncParameterlessBotOperation(Func<Task> operation)
        {
            this.taskOperation = operation;
        }

        public Task ExecuteAsync(ExecutionContext context, CancellationToken token)
        {
            if (this.taskOperation != null)
                return this.taskOperation();

            this.operation();
            return Constants.CompletedTask;
        }
    }
}
