using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot.Passthrough
{
    internal class PassthroughBot : Bot
    {
        internal PassthroughBot(Bot innerBot) : base(innerBot)
        { }

        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.Execute(operation, context, token);

        public override Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.ExecuteAsync(operation, context, token);
    }

    internal class PassthroughBot<TResult> : Bot<TResult>
    {
        internal PassthroughBot(Bot<TResult> innerBot) : base(innerBot)
        { }

        public override TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.Execute(operation, context, token);

        public override Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.ExecuteAsync(operation, context, token);
    }
}
