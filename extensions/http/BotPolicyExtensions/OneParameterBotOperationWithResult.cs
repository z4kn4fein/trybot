using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot.Extensions.Http.BotPolicyExtensions
{
    internal class OneParameterBotOperationWithResult<TArg, TResult> : IBotOperation<TResult>
    {
        private readonly Func<TArg, ExecutionContext, CancellationToken, TResult> operation;
        private readonly TArg arg;

        public OneParameterBotOperationWithResult(Func<TArg, ExecutionContext, CancellationToken, TResult> operation, TArg arg)
        {
            this.operation = operation;
            this.arg = arg;
        }

        public TResult Execute(ExecutionContext context, CancellationToken token) =>
            this.operation(this.arg, context, token);
    }

    internal class AsyncOneParameterBotOperationWithResult<TArg, TResult> : IAsyncBotOperation<TResult>
    {
        private readonly Func<TArg, ExecutionContext, CancellationToken, TResult> operation;
        private readonly TArg arg;
        private readonly Func<TArg, ExecutionContext, CancellationToken, Task<TResult>> taskOperation;

        public AsyncOneParameterBotOperationWithResult(Func<TArg, ExecutionContext, CancellationToken, TResult> operation, TArg arg)
        {
            this.operation = operation;
            this.arg = arg;
        }

        public AsyncOneParameterBotOperationWithResult(Func<TArg, ExecutionContext, CancellationToken, Task<TResult>> operation, TArg arg)
        {
            this.taskOperation = operation;
            this.arg = arg;
        }

        public Task<TResult> ExecuteAsync(ExecutionContext context, CancellationToken token) =>
            this.taskOperation != null ? this.taskOperation(this.arg, context, token) : Task.FromResult(this.operation(this.arg, context, token));
    }
}
