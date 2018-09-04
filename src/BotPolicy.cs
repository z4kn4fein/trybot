using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    public class BotPolicy<TResult> : BotPolicyBase<Bot<TResult>>, IBotPolicy<TResult>, IBotPolicyConfigurator<TResult>
    {
        public BotPolicy(Action<IBotPolicyConfigurator<TResult>> configuratorAction = null)
        {
            configuratorAction?.Invoke(this);
            base.Bot = new DefaultBot<TResult>();
        }

        public IBotPolicyConfigurator<TResult> SetCapturedContextContinuation(bool continueOnCapturedContext = false)
        {
            base.Configuration.ContinueOnCapturedContext = continueOnCapturedContext;
            return this;
        }

        public IBotPolicyConfigurator<TResult> Configure(Action<IBotPolicyBuilder<TResult>> configuratorAction)
        {
            var configurator = new BotPolicyBuilder<TResult>();
            configuratorAction(configurator);

            base.Bot = configurator.Bot;

            return this;
        }

        public TResult Execute(Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            this.Bot.Execute(operation, ExecutionContext.New(base.Configuration), token);

        public async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            await this.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);

        public async Task<TResult> ExecuteAsync(Func<ExecutionContext, CancellationToken, Task<TResult>> operation, CancellationToken token) =>
            await this.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }

    public class BotPolicy : BotPolicyBase<Bot>, IBotPolicy, IBotPolicyConfigurator
    {
        public BotPolicy(Action<IBotPolicyConfigurator> configuratorAction = null)
        {
            configuratorAction?.Invoke(this);
            base.Bot = new DefaultBot();
        }

        public IBotPolicyConfigurator SetCapturedContextContinuation(bool continueOnCapturedContext = false)
        {
            base.Configuration.ContinueOnCapturedContext = continueOnCapturedContext;
            return this;
        }

        public IBotPolicyConfigurator Configure(Action<IBotPolicyBuilder> configuratorAction)
        {
            var configurator = new BotPolicyBuilder();
            configuratorAction(configurator);

            base.Bot = configurator.Bot;

            return this;
        }

        public void Execute(Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            base.Bot.Execute(action, ExecutionContext.New(base.Configuration), token);

        public async Task ExecuteAsync(Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            await base.Bot.ExecuteAsync(action, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);

        public async Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, CancellationToken token) =>
            await base.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }
}
