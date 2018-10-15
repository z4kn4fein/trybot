using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot.Extensions.Http.BotPolicyExtensions
{
    internal class OneParameterBotOperation<TArg> : IBotOperation
    {
        private readonly Action<TArg, ExecutionContext, CancellationToken> operation;
        private readonly TArg arg;

        public OneParameterBotOperation(Action<TArg, ExecutionContext, CancellationToken> operation, TArg arg)
        {
            this.operation = operation;
            this.arg = arg;
        }

        public void Execute(ExecutionContext context, CancellationToken token) =>
            this.operation(this.arg, context, token);
    }

    internal class AsyncOneParameterBotOperation<TArg> : IAsyncBotOperation
    {
        private readonly Action<TArg, ExecutionContext, CancellationToken> operation;
        private readonly Func<TArg, ExecutionContext, CancellationToken, Task> taskOperation;
        private readonly TArg arg;

        public AsyncOneParameterBotOperation(Action<TArg, ExecutionContext, CancellationToken> operation, TArg arg)
        {
            this.operation = operation;
            this.arg = arg;
        }

        public AsyncOneParameterBotOperation(Func<TArg, ExecutionContext, CancellationToken, Task> operation, TArg arg)
        {
            this.arg = arg;
            this.taskOperation = operation;
        }

        public Task ExecuteAsync(ExecutionContext context, CancellationToken token)
        {
            if (this.taskOperation != null)
                return this.taskOperation(this.arg, context, token);

            this.operation(this.arg, context, token);
            return Task.FromResult(0);
        }
    }
}
