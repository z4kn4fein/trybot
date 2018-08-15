using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    public abstract class Bot
    {
        protected Bot InnerBot { get; }

        protected Bot(Bot innerBot)
        {
            this.InnerBot = innerBot;
        }

        internal Bot()
        { }

        public abstract void Execute(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token);

        public abstract TResult Execute<TResult>(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token);

        public abstract Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token);

        public abstract Task<TResult> ExecuteAsync<TResult>(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token);

        public abstract Task<TResult> ExecuteAsync<TResult>(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, ExecutionContext context, CancellationToken token);
    }

    public abstract class Bot<TConfiguration> : Bot
    {
        protected TConfiguration Configuration { get; }

        protected Bot(Bot innerBot, TConfiguration configuration) : base(innerBot)
        {
            this.Configuration = configuration;
        }
    }
}
