using System;
using System.Linq;
using Trybot.Utils;

namespace Trybot.Retry
{
    public class RetryConfiguration
    {
        private int retryCount;
        private Func<int, TimeSpan> retryStrategy;
        private Func<int, object, TimeSpan> resultRetryStrategy;
        private ArrayStore<Func<Exception, bool>> retryPolicies = ArrayStore<Func<Exception, bool>>.Empty;
        private Func<object, bool> resultPolicy;

        internal bool HandlesException(Exception exception) =>
            this.retryPolicies.Any(policy => policy(exception));

        internal bool AcceptsResult(object result) =>
            this.resultPolicy(result);

        internal bool IsMaxAttemptsReached(int currentAttempt) =>
            currentAttempt >= this.retryCount;

        internal TimeSpan CalculateNextDelay(int currentAttempt, bool checkResult, object result) =>
            checkResult && this.resultRetryStrategy != null
                ? this.resultRetryStrategy(currentAttempt, result)
                : this.retryStrategy(currentAttempt);

        public RetryConfiguration UntilAttemptCountReaches(int numOfAttempts = 1)
        {
            this.retryCount = numOfAttempts;
            return this;
        }

        public RetryConfiguration WaitBetweenAttempts(Func<int, TimeSpan> retryStrategy)
        {
            this.retryStrategy = retryStrategy;
            return this;
        }

        public RetryConfiguration WaitBetweenAttempts<TResult>(Func<int, TResult, TimeSpan> resultRetryStrategy)
        {
            this.resultRetryStrategy = (attempt, result) => resultRetryStrategy(attempt, (TResult)result);
            return this;
        }

        public RetryConfiguration RetryWhen(Func<Exception, bool> retryPolicy)
        {
            Swap.SwapValue(ref this.retryPolicies, policies => policies.Add(retryPolicy));
            return this;
        }

        public RetryConfiguration RetryWhenResult<TResult>(Func<TResult, bool> resultPolicy)
        {
            this.resultPolicy = result => resultPolicy((TResult)result);
            return this;
        }
    }
}
