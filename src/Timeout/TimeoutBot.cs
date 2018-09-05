using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Timeout
{
    public class TimeoutBot : ConfigurableBot<TimeoutConfiguration>
    {
        private readonly TimeoutEngine timeoutEngine = new TimeoutEngine();

        internal TimeoutBot(Bot innerPolicy, TimeoutConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override async Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action,
            ExecutionContext context, CancellationToken token) =>
            await this.timeoutEngine.ExecuteAsync(base.Configuration, async (ctx, t) =>
                {
                    await base.InnerBot.ExecuteAsync(action, ctx, t)
                        .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext);
                    return Constants.CompletedTask;
                }, context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

        public override async Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation,
            ExecutionContext context, CancellationToken token)
        {
            await this.timeoutEngine.ExecuteAsync(base.Configuration, async (ctx, t) =>
                {
                    await base.InnerBot.ExecuteAsync(operation, ctx, t)
                        .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext);
                    return Constants.DummyReturnValue;
                }, context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
        }

        public override void Execute(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token)
        {
            this.timeoutEngine.Execute(base.Configuration, (ctx, t) =>
            {
                base.InnerBot.Execute(operation, ctx, t);
                return Constants.DummyReturnValue;
            }, context, token);
        }
    }

    public class TimeoutBot<TResult> : ConfigurableBot<TimeoutConfiguration, TResult>
    {
        private readonly TimeoutEngine timeoutEngine = new TimeoutEngine();

        internal TimeoutBot(Bot<TResult> innerPolicy, TimeoutConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            this.timeoutEngine.Execute(base.Configuration, (ctx, t) => this.InnerBot.Execute(operation, ctx, t), context, token);

        public override async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            await this.timeoutEngine.ExecuteAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext), context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

        public override async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation,
            ExecutionContext context, CancellationToken token) =>
            await this.timeoutEngine.ExecuteAsync(base.Configuration, async (ctx, t) => await base.InnerBot.ExecuteAsync(operation, ctx, t)
                    .ConfigureAwait(ctx.BotPolicyConfiguration.ContinueOnCapturedContext), context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
    }
}