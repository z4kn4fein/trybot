using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;
using Trybot.Retry.Exceptions;
using Trybot.Retry.Model;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal class RetryBot<TResult> : ConfigurableBot<RetryConfiguration<TResult>, TResult>
    {
        internal RetryBot(Bot<TResult> innerPolicy, RetryConfiguration<TResult> configuration) : base(innerPolicy, configuration)
        { }

        public override TResult Execute(IBotOperation<TResult> operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult<TResult>.Default;
            while (RetryBotUtils.ShouldExecute(tryResult, base.Configuration, currentAttempt, token))
            {
                tryResult = this.Try(operation, context, token);

                if (tryResult.IsSucceeded)
                    return tryResult.OperationResult;

                if (RetryBotUtils.HasMaxAttemptsReached(base.Configuration, currentAttempt)) break;

                var nextDelay = base.Configuration.CalculateNextDelay(currentAttempt, tryResult.Exception, tryResult.OperationResult);
                base.Configuration.RaiseRetryEvent(tryResult.OperationResult, tryResult.Exception, AttemptContext.New(currentAttempt, nextDelay, context));

                RetryBotUtils.Wait(nextDelay, token);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException(Constants.MaxRetryExceptionMessage, tryResult.Exception, tryResult.OperationResult);
        }

        public override async Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation,
            ExecutionContext context, CancellationToken token)
        {
            var currentAttempt = 1;
            var tryResult = TryResult<TResult>.Default;
            while (RetryBotUtils.ShouldExecute(tryResult, base.Configuration, currentAttempt, token))
            {
                tryResult = await this.TryAsync(operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                if (tryResult.IsSucceeded)
                    return tryResult.OperationResult;

                if (RetryBotUtils.HasMaxAttemptsReached(base.Configuration, currentAttempt)) break;

                var nextDelay = base.Configuration.CalculateNextDelay(currentAttempt, tryResult.Exception, tryResult.OperationResult);
                await base.Configuration.RaiseRetryEventAsync(tryResult.OperationResult, tryResult.Exception,
                        AttemptContext.New(currentAttempt, nextDelay, context), token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                await RetryBotUtils.WaitAsync(nextDelay, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException(Constants.MaxRetryExceptionMessage, tryResult.Exception, tryResult.OperationResult);
        }

        private async Task<TryResult<TResult>> TryAsync(
            IAsyncBotOperation<TResult> operation,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                var result = await base.InnerBot.ExecuteAsync(operation, context, token)
                    .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);

                return this.HandleResult(result);
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    return TryResult<TResult>.Failed(exception);

                throw;
            }
        }

        private TryResult<TResult> Try(
            IBotOperation<TResult> operation,
            ExecutionContext context,
            CancellationToken token)
        {
            try
            {
                var result = base.InnerBot.Execute(operation, context, token);
                return this.HandleResult(result);
            }
            catch (Exception exception)
            {
                if (base.Configuration.HandlesException(exception))
                    return TryResult<TResult>.Failed(exception);

                throw;
            }
        }

        private TryResult<TResult> HandleResult(TResult result) =>
            base.Configuration.AcceptsResult(result) ? TryResult<TResult>.Succeeded(result) : TryResult<TResult>.Failed(result: result);
    }
}
