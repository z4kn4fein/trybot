using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Fallback
{
    public class FallbackBot : ConfigurableBot<FallbackConfiguration>
    {
        private readonly FallbackEngine fallbackEngine = new FallbackEngine();

        internal FallbackBot(Bot innerBot, FallbackConfiguration configuration) : base(innerBot, configuration)
        { }

        public override void Execute(Action<ExecutionContext, CancellationToken> action,
            ExecutionContext context, CancellationToken token) =>
            this.fallbackEngine.Execute(base.Configuration, (ctx, t) =>
                {
                    base.InnerBot.Execute(action, ctx, t);
                    return Constants.DummyReturnValue;
                },
                onFallback: (result, exception, ctx) => base.Configuration.FallbackHandler(exception, ctx),
                resultChecker: result => true,
                context: context,
                token: token);

        public override Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action,
            ExecutionContext context, CancellationToken token) =>
            this.fallbackEngine.ExecuteAsync(base.Configuration, async (ctx, t) =>
                {
                    await base.InnerBot.ExecuteAsync(action, ctx, t);
                    return Constants.CompletedTask;
                },
                onFallbackAsync: (result, exception, ctx, t) => base.Configuration.AsyncFallbackHandler(exception, ctx, t),
                resultChecker: result => true,
                context: context,
                token: token);

        public override Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation,
            ExecutionContext context, CancellationToken token) =>
            this.fallbackEngine.ExecuteAsync(base.Configuration, async (ctx, t) =>
                {
                    await base.InnerBot.ExecuteAsync(operation, ctx, t);
                    return Constants.CompletedTask;
                },
                onFallbackAsync: (result, exception, ctx, t) => base.Configuration.AsyncFallbackHandler(exception, ctx, t),
                resultChecker: result => true,
                context: context,
                token: token);
    }

    public class FallbackBot<TResult> : ConfigurableBot<FallbackConfiguration<TResult>, TResult>
    {
        private readonly FallbackEngine fallbackEngine = new FallbackEngine();

        internal FallbackBot(Bot<TResult> innerBot, FallbackConfiguration<TResult> configuration) : base(innerBot, configuration)
        {
        }

        public override TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            this.fallbackEngine.Execute(base.Configuration, (ctx, t) => this.InnerBot.Execute(operation, ctx, t),
                onFallback: (result, exception, ctx) => base.Configuration.FallbackHandlerWithResult(result, exception, ctx),
                resultChecker: result => base.Configuration.AcceptsResult(result),
                context: context,
                token: token);

        public override Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            this.fallbackEngine.ExecuteAsync(base.Configuration, (ctx, t) => this.InnerBot.ExecuteAsync(operation, ctx, t),
                onFallbackAsync: (result, exception, ctx, t) => base.Configuration.AsyncFallbackHandlerWithResult(result, exception, ctx, t),
                resultChecker: result => base.Configuration.AcceptsResult(result),
                context: context,
                token: token);

        public override Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            ExecutionContext context, CancellationToken token) =>
            this.fallbackEngine.ExecuteAsync(base.Configuration, (ctx, t) => this.InnerBot.ExecuteAsync(operation, ctx, t),
                onFallbackAsync: (result, exception, ctx, t) => base.Configuration.AsyncFallbackHandlerWithResult(result, exception, ctx, t),
                resultChecker: result => base.Configuration.AcceptsResult(result),
                context: context,
                token: token);
    }
}
