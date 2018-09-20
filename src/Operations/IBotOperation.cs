using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Operations
{
    /// <summary>
    /// Represents a synchronous operation which can be passed to the <see cref="IBotPolicy"/>.
    /// </summary>
    public interface IBotOperation
    {
        /// <summary>
        /// This method will be used by the bots to execute the passed operation.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        void Execute(ExecutionContext context, CancellationToken token);
    }

    /// <summary>
    /// Represents a asynchronous operation which can be passed to the <see cref="IBotPolicy"/>.
    /// </summary>
    public interface IAsyncBotOperation
    {
        /// <summary>
        /// This method will be used by the bots to execute the passed operation.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The task wrapped the operation.</returns>
        Task ExecuteAsync(ExecutionContext context, CancellationToken token);
    }

    /// <summary>
    /// Represents a synchronous operation which can be passed to the <see cref="IBotPolicy{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type of the given operation.</typeparam>
    public interface IBotOperation<out TResult>
    {
        /// <summary>
        /// This method will be used by the bots to execute the passed operation.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The operations return value.</returns>
        TResult Execute(ExecutionContext context, CancellationToken token);
    }

    /// <summary>
    /// Represents a asynchronous operation which can be passed to the <see cref="IBotPolicy{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type of the given operation.</typeparam>
    public interface IAsyncBotOperation<TResult>
    {
        /// <summary>
        /// This method will be used by the bots to execute the passed operation.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The task wrapped the operation.</returns>
        Task<TResult> ExecuteAsync(ExecutionContext context, CancellationToken token);
    }
}
