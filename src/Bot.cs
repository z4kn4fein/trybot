using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot
{
    public abstract class Bot
    {
        protected Bot InnerBot { get; }

        protected Bot(Bot innerBot)
        {
            Shield.EnsureNotNull(innerBot, nameof(innerBot));

            this.InnerBot = innerBot;
        }

        internal Bot()
        { }

        public abstract void Execute(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token);

        public abstract Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token);

        public abstract Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, ExecutionContext context, CancellationToken token);
    }

    public abstract class Bot<TResult>
    {
        protected Bot<TResult> InnerBot { get; }

        protected Bot(Bot<TResult> innerBot)
        {
            Shield.EnsureNotNull(innerBot, nameof(innerBot));

            this.InnerBot = innerBot;
        }

        internal Bot()
        { }

        public abstract TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token);

        public abstract Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation, ExecutionContext context, CancellationToken token);

        public abstract Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, ExecutionContext context, CancellationToken token);
    }

    public abstract class ConfigurableBot<TConfiguration> : Bot
    {
        protected TConfiguration Configuration { get; }

        protected ConfigurableBot(Bot innerBot, TConfiguration configuration) : base(innerBot)
        {
            this.Configuration = configuration;
        }
    }

    public abstract class ConfigurableBot<TConfiguration, TResult> : Bot<TResult>
    {
        protected TConfiguration Configuration { get; }

        protected ConfigurableBot(Bot<TResult> innerBot, TConfiguration configuration) : base(innerBot)
        {
            this.Configuration = configuration;
        }
    }
}
