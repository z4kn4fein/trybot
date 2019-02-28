using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;
using Trybot.Retry.Exceptions;
using Trybot.Retry.Model;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal class RetryBot : ConfigurableBot<RetryConfiguration>
    {
        internal RetryBot(Bot innerPolicy, RetryConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override void Execute(IBotOperation operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult.Default;
            while (RetryBotUtils.ShouldExecute(tryResult, base.Configuration, currentAttempt, token))
            {
                tryResult = this.Try(operation, context, token);

                if (tryResult.IsSucceeded)
                {
                    if (currentAttempt > 1)
                        base.Configuration.RaiseRetrySucceededEvent(AttemptContext.New(currentAttempt, TimeSpan.Zero, context));

                    return;
                }

                if (RetryBotUtils.HasMaxAttemptsReached(base.Configuration, currentAttempt)) break;

                var nextDelay = base.Configuration.CalculateNextDelay(currentAttempt, tryResult.Exception);
                base.Configuration.RaiseRetryEvent(tryResult.Exception, AttemptContext.New(currentAttempt, nextDelay, context));

                RetryBotUtils.Wait(nextDelay, token);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            base.Configuration.RaiseRetryLimitReachedEvent(tryResult.Exception, context);
            throw new MaxRetryAttemptsReachedException(Constants.MaxRetryExceptionMessage, tryResult.Exception, null);
        }

        public override async Task ExecuteAsync(IAsyncBotOperation operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult.Default;
            while (RetryBotUtils.ShouldExecute(tryResult, base.Configuration, currentAttempt, token))
            {
                tryResult = await this.TryAsync(operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                if (tryResult.IsSucceeded)
                {
                    if (currentAttempt > 1)
                        await base.Configuration.RaiseRetryEventSucceededAsync(AttemptContext.New(currentAttempt, TimeSpan.Zero, context), token)
                            .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                    return;
                }

                if (RetryBotUtils.HasMaxAttemptsReached(base.Configuration, currentAttempt)) break;

                var nextDelay = base.Configuration.CalculateNextDelay(currentAttempt, tryResult.Exception);
                await base.Configuration.RaiseRetryEventAsync(tryResult.Exception, AttemptContext.New(currentAttempt, nextDelay, context), token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                await RetryBotUtils.WaitAsync(nextDelay, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            await base.Configuration.RaiseAsyncRetryLimitReachedEvent(tryResult.Exception, context, token)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

            throw new MaxRetryAttemptsReachedException(Constants.MaxRetryExceptionMessage, tryResult.Exception, null);
        }

        private TryResult Try(
            IBotOperation operation,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                base.InnerBot.Execute(operation, context, token);
                return TryResult.Succeeded();
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    return TryResult.Failed(exception);

                throw;
            }
        }

        private async Task<TryResult> TryAsync(
            IAsyncBotOperation operation,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                await base.InnerBot.ExecuteAsync(operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                return TryResult.Succeeded();
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    return TryResult.Failed(exception);

                throw;
            }
        }
    }
}
