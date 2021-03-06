﻿using System;
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
        public static void Execute(this IBotPolicy policy, Action<ExecutionContext, CancellationToken> action, CancellationToken token = default) =>
            policy.Execute(new BotOperation(action), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task ExecuteAsync(this IBotPolicy policy, Action<ExecutionContext, CancellationToken> action, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncBotOperation(action), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task ExecuteAsync(this IBotPolicy policy, Func<ExecutionContext, CancellationToken, Task> operation, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncBotOperation(operation), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an opeariont synchronously within the bot policy and returns with its result.
        /// </summary>
        /// <typeparam name="TResult">The result type of the given operation.</typeparam>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The operations result.</returns>
        public static TResult Execute<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token = default) =>
            policy.Execute(new BotOperation<TResult>(operation), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy and returns with its result.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncBotOperation<TResult>(operation), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy and returns with its result.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, Task<TResult>> operation, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncBotOperation<TResult>(operation), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action synchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        public static void Execute(this IBotPolicy policy, Action action, CancellationToken token = default) =>
            policy.Execute(new ParameterlessBotOperation(action), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task ExecuteAsync(this IBotPolicy policy, Action action, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncParameterlessBotOperation(action), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task ExecuteAsync(this IBotPolicy policy, Func<Task> operation, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncParameterlessBotOperation(operation), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an opeariont synchronously within the bot policy and returns with its result.
        /// </summary>
        /// <typeparam name="TResult">The result type of the given operation.</typeparam>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The operations result.</returns>
        public static TResult Execute<TResult>(this IBotPolicy<TResult> policy, Func<TResult> operation, CancellationToken token = default) =>
            policy.Execute(new ParameterlessBotOperation<TResult>(operation), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy and returns with its result.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<TResult> operation, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncParameterlessBotOperation<TResult>(operation), Guid.NewGuid(), token);

        /// <summary>
        /// Executes an action asynchronously within the bot policy and returns with its result.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="token">Tha cancellation token.</param>
        /// <returns>The task to await.</returns>
        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<Task<TResult>> operation, CancellationToken token = default) =>
            policy.ExecuteAsync(new AsyncParameterlessBotOperation<TResult>(operation), Guid.NewGuid(), token);
    }
}
