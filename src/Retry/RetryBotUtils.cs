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

        public static TimeSpan CalculateNextDelay<TResult>(RetryConfiguration<TResult> configuration, int currentAttempt, Exception exception, TResult result) =>
            configuration.CalculateNextDelay(currentAttempt, exception, result);

        public static TimeSpan CalculateNextDelay(RetryConfiguration configuration, int currentAttempt, Exception exception) =>
            configuration.CalculateNextDelay(currentAttempt, exception);
    }
}
