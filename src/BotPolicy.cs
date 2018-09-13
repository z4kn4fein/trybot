using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    public class BotPolicy<TResult> : BotPolicyBase<Bot<TResult>>, IBotPolicy<TResult>, IBotPolicyConfigurator<TResult>
    {
        public BotPolicy(Action<IBotPolicyConfigurator<TResult>> configuratorAction = null)
            : base(new DefaultBot<TResult>())
        {
            configuratorAction?.Invoke(this);
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
        public TResult Execute(IBotOperation<TResult> operation, CancellationToken token) =>
            this.Bot.Execute(operation, ExecutionContext.New(base.Configuration), token);

        public async Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, CancellationToken token) =>
            await this.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }

    public class BotPolicy : BotPolicyBase<Bot>, IBotPolicy, IBotPolicyConfigurator
    {
        public BotPolicy(Action<IBotPolicyConfigurator> configuratorAction = null)
            : base(new DefaultBot())
        {
            configuratorAction?.Invoke(this);
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

        public void Execute(IBotOperation operation, CancellationToken token) =>
            base.Bot.Execute(operation, ExecutionContext.New(base.Configuration), token);

        public async Task ExecuteAsync(IAsyncBotOperation operation, CancellationToken token) =>
            await base.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }
}
