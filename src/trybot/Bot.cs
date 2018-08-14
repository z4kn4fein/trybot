using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    public abstract class Bot
    {
        protected Bot InnerPolicy { get; }

        protected Bot(Bot innerPolicy)
        {
            this.InnerPolicy = innerPolicy;
        }

        public abstract void Execute(Action<CancellationToken> action, CancellationToken token);

        public abstract TResult Execute<TResult>(Func<CancellationToken, TResult> operation, CancellationToken token);

        public abstract Task ExecuteAsync(Action<CancellationToken> action, CancellationToken token);

        public abstract Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, TResult> operation, CancellationToken token);

        public abstract Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken token);
    }

    public abstract class Bot<TConfiguration> : Bot
    {
        protected TConfiguration Configuration { get; }

        protected Bot(Bot innerPolicy, TConfiguration configuration) : base(innerPolicy)
        {
            this.Configuration = configuration;
        }
    }
}
