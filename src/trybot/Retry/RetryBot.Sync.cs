using System;
using System.Threading;

namespace Trybot.Retry
{
    public partial class RetryBot : Bot<RetryConfiguration>
    {
        public RetryBot(Bot innerPolicy, RetryConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override void Execute(Action<ExecutionContext, CancellationToken> action, ExecutionContext context, CancellationToken token)
        {
            this.ExecuteRetry<object>((ctx, t) =>
            {
                this.InnerBot.Execute(action, ctx, t);
                return null;
            }, context, token, false);
        }

        public override TResult Execute<TResult>(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token) =>
            this.ExecuteRetry((ctx, t) => this.InnerBot.Execute(operation, ctx, t), context, token, true);

        private TResult ExecuteRetry<TResult>(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token, bool checkResult)
        {
            var currentAttempt = 1;
            var tryResult = TryResult.Default;
            while (!token.IsCancellationRequested && !tryResult.IsSucceeded && !this.Configuration.IsMaxAttemptsReached(currentAttempt))
            {
                tryResult = this.Try(operation, context, token, checkResult);

                if (tryResult.IsSucceeded)
                    return (TResult)tryResult.OperationResult;

                if (this.Configuration.IsMaxAttemptsReached(currentAttempt)) break;

                token.WaitHandle.WaitOne(this.Configuration.CalculateNextDelay(currentAttempt, checkResult, tryResult.OperationResult));
                currentAttempt++;
            }

            token.ThrowIfCancellationRequested();

            throw new MaxRetryAttemptsReachedException<TResult>("Maximum number of retry attempts reached.", tryResult.Exception, (TResult)tryResult.OperationResult);
        }

        private TryResult Try<TResult>(Func<ExecutionContext, CancellationToken, TResult> operation,
            ExecutionContext context, CancellationToken token, bool checkResult)
        {
            try
            {
                var result = operation(context, token);

                if (checkResult && !this.Configuration.AcceptsResult(result))
                    return TryResult.Failed(result: result);

                return TryResult.Succeeded(result);
            }
            catch (Exception exception)
            {
                if (this.Configuration.HandlesException(exception))
                    return TryResult.Failed(exception);

                throw;
            }
        }
    }
}
