using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    /// <summary>
    /// Represents a bot policy which can be configured with several <see cref="Bot"/> implementations.
    /// </summary>
    public interface IBotPolicy
    {
        /// <summary>
        /// Executes a given operation synchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="correlationId">A unique identifier for the execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="operation"/> or <paramref name="correlationId"/> is null.</exception>
        void Execute(IBotOperation operation, object correlationId, CancellationToken token);

        /// <summary>
        /// Executes a given operation asynchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="correlationId">A unique identifier for the execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        /// <returns>The asynchronous task which executed the operation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="operation"/> or <paramref name="correlationId"/> is null.</exception>
        Task ExecuteAsync(IAsyncBotOperation operation, object correlationId, CancellationToken token);
    }

    /// <summary>
    /// Represents a bot policy which can be configured with several <see cref="Bot{TResult}"/> implementations.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation passed to the bot policy.</typeparam>
    public interface IBotPolicy<TResult>
    {
        /// <summary>
        /// Executes a given operation synchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="correlationId">A unique identifier for the execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        /// <returns>The result of the given operation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="operation"/> or <paramref name="correlationId"/> is null.</exception>
        TResult Execute(IBotOperation<TResult> operation, object correlationId, CancellationToken token);

        /// <summary>
        /// Executes a given operation asynchronously.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="correlationId">A unique identifier for the execution.</param>
        /// <param name="token">The cancellation token, used to cancel the execution of the given operation.</param>
        /// <returns>The asynchronous task which executed the operation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="operation"/> or <paramref name="correlationId"/> is null.</exception>
        Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, object correlationId, CancellationToken token);
    }
}
