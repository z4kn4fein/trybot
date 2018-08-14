using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal class RetryBot : Bot<RetryConfiguration>
    {
        protected RetryBot(Bot innerPolicy, RetryConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override void Execute(Action<CancellationToken> action, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override TResult Execute<TResult>(Func<CancellationToken, TResult> operation, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override Task ExecuteAsync(Action<CancellationToken> action, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, TResult> operation, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task<TResult> ExecuteRetryAsync<TResult>(Task<TResult> operation, CancellationToken token, bool checkResult)
        {
            int currentAttempt = 1;
            TryResult tryResult;
            do
            {
                tryResult = await this.TryAsync(operation, token, checkResult);
                if (tryResult.IsSucceeded)
                    return (TResult)tryResult.OperationResult;

                await TaskDelayer.Sleep(this.Configuration.RetryStrategy(currentAttempt), token);
                currentAttempt++;

            } while (!token.IsCancellationRequested && !tryResult.IsSucceeded && !this.Configuration.IsMaxAttemptsReached(currentAttempt));


            throw new MaxRetryAttemptsReachedException<TResult>("Maximum number of retry attempts reached.", tryResult.Exception, (TResult)tryResult.OperationResult);
        }

        private async Task<TryResult> TryAsync<TResult>(Task<TResult> operation, CancellationToken token, bool checkResult)
        {
            try
            {
                var result = await operation;
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
