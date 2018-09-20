using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    /// <inheritdoc cref="BotPolicyBase{TBot}" />
    /// <inheritdoc cref="IBotPolicy{TResult}" />
    /// <inheritdoc cref="IBotPolicyConfigurator{TResult}" />
    public class BotPolicy<TResult> : BotPolicyBase<Bot<TResult>>, IBotPolicy<TResult>, IBotPolicyConfigurator<TResult>
    {
        /// <summary>
        /// Constructs a <see cref="BotPolicy{TResult}"/>.
        /// </summary>
        /// <param name="configuratorAction">An optional configurator action.</param>
        public BotPolicy(Action<IBotPolicyConfigurator<TResult>> configuratorAction = null)
            : base(new DefaultBot<TResult>())
        {
            configuratorAction?.Invoke(this);
        }

        /// <inheritdoc />
        public IBotPolicyConfigurator<TResult> SetCapturedContextContinuation(bool continueOnCapturedContext = false)
        {
            base.Configuration.ContinueOnCapturedContext = continueOnCapturedContext;
            return this;
        }

        /// <inheritdoc />
        public IBotPolicyConfigurator<TResult> Configure(Action<IBotPolicyBuilder<TResult>> configuratorAction)
        {
            var configurator = new BotPolicyBuilder<TResult>();
            configuratorAction(configurator);

            base.Bot = configurator.Bot;

            return this;
        }

        /// <inheritdoc />
        public TResult Execute(IBotOperation<TResult> operation, CancellationToken token) =>
            this.Bot.Execute(operation, ExecutionContext.New(base.Configuration), token);

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, CancellationToken token) =>
            await this.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }

    /// <inheritdoc cref="BotPolicyBase{TBot}" />
    /// <inheritdoc cref="IBotPolicy" />
    /// <inheritdoc cref="IBotPolicyConfigurator" />
    public class BotPolicy : BotPolicyBase<Bot>, IBotPolicy, IBotPolicyConfigurator
    {
        /// <summary>
        /// Constructs a <see cref="BotPolicy"/>.
        /// </summary>
        /// <param name="configuratorAction">An optional configurator action.</param>
        public BotPolicy(Action<IBotPolicyConfigurator> configuratorAction = null)
            : base(new DefaultBot())
        {
            configuratorAction?.Invoke(this);
        }

        /// <inheritdoc />
        public IBotPolicyConfigurator SetCapturedContextContinuation(bool continueOnCapturedContext = false)
        {
            base.Configuration.ContinueOnCapturedContext = continueOnCapturedContext;
            return this;
        }

        /// <inheritdoc />
        public IBotPolicyConfigurator Configure(Action<IBotPolicyBuilder> configuratorAction)
        {
            var configurator = new BotPolicyBuilder();
            configuratorAction(configurator);

            base.Bot = configurator.Bot;

            return this;
        }

        /// <inheritdoc />
        public void Execute(IBotOperation operation, CancellationToken token) =>
            base.Bot.Execute(operation, ExecutionContext.New(base.Configuration), token);

        /// <inheritdoc />
        public async Task ExecuteAsync(IAsyncBotOperation operation, CancellationToken token) =>
            await base.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }
}
