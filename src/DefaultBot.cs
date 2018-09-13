using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    internal class DefaultBot : Bot
    {
        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token) =>
            operation.Execute(context, token);

        public override Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token) =>
            operation.ExecuteAsync(context, token);
    }

    internal class DefaultBot<TResult> : Bot<TResult>
    {
        public override TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token) =>
            operation.Execute(context, token);

        public override Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token) =>
            operation.ExecuteAsync(context, token);
    }
}
