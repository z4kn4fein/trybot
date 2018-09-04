using System;
using System.Threading;

namespace Trybot.Retry
{
    public partial class RetryBot<TResult> : ConfigurableBot<RetryConfiguration<TResult>, TResult>
    {
        private readonly RetryEngine<TResult> retryEngine = new RetryEngine<TResult>();

        public RetryBot(Bot<TResult> innerPolicy, RetryConfiguration<TResult> configuration) : base(innerPolicy, configuration)
        { }

        public override TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            this.retryEngine.ExecuteRetry(base.Configuration, (ctx, t) => this.InnerBot.Execute(operation, ctx, t), context, token);
    }

    public partial class RetryBot : ConfigurableBot<RetryConfiguration>
    {
        private readonly RetryEngine retryEngine = new RetryEngine();

        public RetryBot(Bot innerPolicy, RetryConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override void Execute(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token) =>
            this.retryEngine.ExecuteRetry(base.Configuration, (ctx, t) => base.InnerBot.Execute(operation, ctx, t), context, token);
    }
}
