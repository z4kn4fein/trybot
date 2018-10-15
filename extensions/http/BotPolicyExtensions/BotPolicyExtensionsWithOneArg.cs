using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Extensions.Http.BotPolicyExtensions;

namespace Trybot
{
    /// <summary>
    /// Extensions for <see cref="IBotPolicy"/>.
    /// </summary>
    public static class BotPolicyExtensionsWithOneArg
    {
        /// <summary>
        /// Executes an action asynchronously within the bot policy and returns with its result.
        /// </summary>
        /// <typeparam name="TResult">The result type of the given operation.</typeparam>
        /// <typeparam name="TArg">The type of the extra argument passed to the operation.</typeparam>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="arg">The extra argument to pass to the operation.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task<TResult> ExecuteAsync<TArg, TResult>(this IBotPolicy<TResult> policy,
            Func<TArg, ExecutionContext, CancellationToken, Task<TResult>> operation, object correlationId,
            TArg arg, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncOneParameterBotOperationWithResult<TArg, TResult>(operation, arg),
                correlationId ?? Guid.NewGuid(), token);
    }
}
