using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    public static class BotPolicyExtensions
    {
        public static void Execute(this IBotPolicy policy, Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            policy.Execute(new BotOperation(action), token);

        public static Task ExecuteAsync(this IBotPolicy policy, Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation(action), token);

        public static Task ExecuteAsync(this IBotPolicy policy, Func<ExecutionContext, CancellationToken, Task> operation, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation(operation), token);

        public static TResult Execute<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            policy.Execute(new BotOperation<TResult>(operation), token);

        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation<TResult>(operation), token);

        public static Task<TResult> ExecuteAsync<TResult>(this IBotPolicy<TResult> policy, Func<ExecutionContext, CancellationToken, Task<TResult>> operation, CancellationToken token) =>
            policy.ExecuteAsync(new AsyncBotOperation<TResult>(operation), token);
    }
}
