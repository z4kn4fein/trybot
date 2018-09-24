using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    /// <summary>
    /// Represents a bot policy which can be configured with several <see cref="Bot{TResult}"/> implementations.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation passed to the bot policy.</typeparam>
    /// <example>
    /// <code>
    /// var policy = new BotPolicy&lt;int&gt;();
    /// policy.Configure(botConfig => botconfig
    ///     .Retry(...)
    ///     .Timeout(...));
    /// 
    /// policy.Execute((context, token) => 
    ///     {
    ///         someAction());
    ///         someOtherAction());
    ///         return 2;
    ///     });
    /// </code>
    /// </example>
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
        public TResult Execute(IBotOperation<TResult> operation, object correlationId, CancellationToken token) =>
            this.Bot.Execute(operation, ExecutionContext.New(base.Configuration, correlationId), token);

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, object correlationId, CancellationToken token) =>
            await this.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration, correlationId), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }

    /// <summary>
    /// Represents a bot policy which can be configured with several <see cref="Bot"/> implementations.
    /// </summary>
    /// <example>
    /// <code>
    /// var policy = new BotPolicy();
    /// policy.Configure(botConfig => botconfig
    ///     .Retry(...)
    ///     .Timeout(...));
    /// 
    /// policy.Execute((context, token) => 
    ///     {
    ///         someAction());
    ///         someOtherAction());
    ///     });
    /// </code>
    /// </example>
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
        public void Execute(IBotOperation operation, object correlationId, CancellationToken token) =>
            base.Bot.Execute(operation, ExecutionContext.New(base.Configuration, correlationId), token);

        /// <inheritdoc />
        public async Task ExecuteAsync(IAsyncBotOperation operation, object correlationId, CancellationToken token) =>
            await base.Bot.ExecuteAsync(operation, ExecutionContext.New(base.Configuration, correlationId), token)
                .ConfigureAwait(base.Configuration.ContinueOnCapturedContext);
    }
}
