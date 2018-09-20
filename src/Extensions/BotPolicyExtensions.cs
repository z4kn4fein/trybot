using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    /// <summary>
    /// Extensions for <see cref="IBotPolicy"/>.
    /// </summary>
    public static class BotPolicyExtensions
    {
        /// <summary>
        /// Executes an action synchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        public static void Execute(this IBotPolicy policy, Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            policy.Execute(new BotOperation(action), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task ExecuteAsync(this IBotPolicy policy, Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation(action), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task ExecuteAsync(this IBotPolicy policy, Func<ExecutionContext, CancellationToken, Task> operation, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation(operation), token);

        /// <summary>
        /// Executes an opeariont synchronously within the bot policy and returns with its result.
        /// </summary>
        /// <typeparam name="TResult">The result type of the given operation.</typeparam>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The operations result.</returns>
        public static TResult Execute<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            policy.Execute(new BotOperation<TResult>(operation), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy and returns with its result.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation<TResult>(operation), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy and returns with its result.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, Task<TResult>> operation, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation<TResult>(operation), token);
    }
}
