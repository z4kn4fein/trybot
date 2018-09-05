using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Passthrough
{
    public class PassthroughBot : Bot
    {
        internal PassthroughBot(Bot innerBot) : base(innerBot)
        { }

        public override void Execute(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.Execute(action, context, token);

        public override Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.ExecuteAsync(action, context, token);

        public override Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.ExecuteAsync(operation, context, token);
    }

    public class PassthroughBot<TResult> : Bot<TResult>
    {
        internal PassthroughBot(Bot<TResult> innerBot) : base(innerBot)
        { }

        public override TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.Execute(operation, context, token);

        public override Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.ExecuteAsync(operation, context, token);

        public override Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, ExecutionContext context, CancellationToken token) =>
            this.InnerBot.ExecuteAsync(operation, context, token);
    }
}
