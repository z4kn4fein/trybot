using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Model;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal static class RetryBotUtils
    {
        public static bool ShouldExecute(TryResult tryResult, RetryConfigurationBase configuration, int currentAttempt, CancellationToken token) =>
            !token.IsCancellationRequested && !tryResult.IsSucceeded && !configuration.HasMaxAttemptsReached(currentAttempt);

        public static bool HasMaxAttemptsReached(RetryConfigurationBase configuration, int currentAttempt) =>
            configuration.HasMaxAttemptsReached(currentAttempt);

        public static void Wait(TimeSpan waitTime, CancellationToken token) =>
            token.WaitHandle.WaitOne(waitTime);

        public static async Task WaitAsync(TimeSpan waitTime, ExecutionContext context, CancellationToken token) =>
            await TaskDelayer.Sleep(waitTime, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
    }
}
