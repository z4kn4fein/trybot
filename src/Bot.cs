using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;
using Trybot.Utils;

namespace Trybot
{
    /// <summary>
    /// Represents a bot abstraction whichs implementation can be used with the <see cref="IBotPolicy"/>
    /// </summary>
    public abstract class Bot
    {
        /// <summary>
        /// The nested bot whichs calls should be wrapped by the actual implementation.
        /// </summary>
        protected Bot InnerBot { get; }

        /// <summary>
        /// Constructs a new <see cref="Bot"/> implementation.
        /// </summary>
        /// <param name="innerBot">The nested bot.</param>
        protected Bot(Bot innerBot)
        {
            Shield.EnsureNotNull(innerBot, nameof(innerBot));

            this.InnerBot = innerBot;
        }

        internal Bot()
        { }

        /// <summary>
        /// Executes a given operation synchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The execution context which holds additional information about the current execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        public abstract void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token);

        /// <summary>
        /// Executes a given operation asynchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The execution context which holds additional information about the current execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        /// <returns>The asynchronous task which executed the operation.</returns>
        public abstract Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token);
    }

    /// <summary>
    /// Represents a bot abstraction whichs implementation can be used with the <see cref="IBotPolicy{TResult}"/>
    /// </summary>
    /// <typeparam name="TResult">The result type of the delegate passed to execution.</typeparam>
    public abstract class Bot<TResult>
    {
        /// <summary>
        /// The nested bot whichs calls should be wrapped by the actual implementation.
        /// </summary>
        protected Bot<TResult> InnerBot { get; }

        /// <summary>
        /// Constructs a new <see cref="Bot{TResult}"/> implementation.
        /// </summary>
        /// <param name="innerBot">The nested bot.</param>
        protected Bot(Bot<TResult> innerBot)
        {
            Shield.EnsureNotNull(innerBot, nameof(innerBot));

            this.InnerBot = innerBot;
        }

        internal Bot()
        { }

        /// <summary>
        /// Executes a given operation synchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The execution context which holds additional information about the current execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        /// <returns>The result of the given operation.</returns>
        public abstract TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token);

        /// <summary>
        /// Executes a given operation asynchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The execution context which holds additional information about the current execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        /// <returns>The asynchronous task which executed the operation.</returns>
        public abstract Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token);
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents a configurable bot abstraction whichs implementation can be used with the <see cref="IBotPolicy" />.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of the bots configuration.</typeparam>
    public abstract class ConfigurableBot<TConfiguration> : Bot
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        protected TConfiguration Configuration { get; }

        /// <inheritdoc />
        /// <summary>
        /// Constructs a new <see cref="ConfigurableBot{TConfiguration}" /> implementation.
        /// </summary>
        /// <param name="innerBot">The nested bot.</param>
        /// <param name="configuration">The configuration.</param>
        protected ConfigurableBot(Bot innerBot, TConfiguration configuration) : base(innerBot)
        {
            this.Configuration = configuration;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents a configurable bot abstraction whichs implementation can be used with the <see cref="IBotPolicy{TResult}" />.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of the bots configuration.</typeparam>
    /// <typeparam name="TResult">The result type of the delegate passed to execution.</typeparam>
    public abstract class ConfigurableBot<TConfiguration, TResult> : Bot<TResult>
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        protected TConfiguration Configuration { get; }

        /// <inheritdoc />
        /// <summary>
        /// Constructs a new <see cref="ConfigurableBot{TConfiguration, TResult}" /> implementation.
        /// </summary>
        /// <param name="innerBot">The nested bot.</param>
        /// <param name="configuration">The configuration.</param>
        protected ConfigurableBot(Bot<TResult> innerBot, TConfiguration configuration) : base(innerBot)
        {
            this.Configuration = configuration;
        }
    }
}
