using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    internal class DefaultBot : Bot
    {
        public DefaultBot(Bot innerPolicy)
            : base(innerPolicy)
        { }

        public override void Execute(Action<CancellationToken> action, CancellationToken token) =>
            this.InnerPolicy.Execute(action, token);

        public override TResult Execute<TResult>(Func<CancellationToken, TResult> operation, CancellationToken token) =>
            this.InnerPolicy.Execute(operation, token);

        public override Task ExecuteAsync(Action<CancellationToken> action, CancellationToken token) =>
            this.InnerPolicy.ExecuteAsync(action, token);

        public override Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, TResult> operation, CancellationToken token) =>
            this.InnerPolicy.ExecuteAsync(operation, token);

        public override Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken token) =>
            this.InnerPolicy.ExecuteAsync(operation, token);
    }
}
