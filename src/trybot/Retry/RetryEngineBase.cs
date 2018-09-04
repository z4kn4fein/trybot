using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Retry.Model;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal class RetryEngineBase
    {
        public bool ShouldExecute(TryResult tryResult, RetryConfigurationBase configuration, int currentAttempt, CancellationToken token) =>
            !token.IsCancellationRequested && !tryResult.IsSucceeded && !configuration.HasMaxAttemptsReached(currentAttempt);

        public bool HasMaxAttemptsReached(RetryConfigurationBase configuration, int currentAttempt) =>
            configuration.HasMaxAttemptsReached(currentAttempt);

        public void Wait(TimeSpan waitTime, CancellationToken token) =>
            token.WaitHandle.WaitOne(waitTime);

        public async Task WaitAsync(TimeSpan waitTime, ExecutionContext context, CancellationToken token) =>
            await TaskDelayer.Sleep(waitTime, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

        public TimeSpan CalculateNextDelay<TResult>(RetryConfiguration<TResult> configuration, int currentAttempt, TResult result) =>
            configuration.CalculateNextDelay(currentAttempt, result);

        public TimeSpan CalculateNextDelay(RetryConfiguration configuration, int currentAttempt) =>
            configuration.CalculateNextDelay(currentAttempt);
    }
}
