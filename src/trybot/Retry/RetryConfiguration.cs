using System;
using System.Linq;
using Trybot.Utils;

namespace Trybot.Retry
{
    public class RetryConfiguration
    {
        internal int RetryCount { get; private set; }

        internal Func<int, TimeSpan> RetryStrategy { get; private set; }

        internal Func<int, object, TimeSpan> ResultRetryStrategy { get; private set; }

        private ArrayStore<Func<Exception, bool>> retryPolicies = ArrayStore<Func<Exception, bool>>.Empty;
        internal ArrayStore<Func<Exception, bool>> RetryPolicies => this.retryPolicies;

        private ArrayStore<Func<object, bool>> resultPolicies = ArrayStore<Func<object, bool>>.Empty;
        internal ArrayStore<Func<object, bool>> ResultPolicies => this.resultPolicies;

        internal bool HandlesException(Exception exception) =>
            this.RetryPolicies.Any(policy => policy(exception));

        internal bool AcceptsResult(object result) =>
            this.ResultPolicies.All(policy => policy(result));

        internal bool IsMaxAttemptsReached(int currentAttempt) =>
            currentAttempt >= this.RetryCount;

        internal TimeSpan CalculateNextDelay(int currentAttempt, bool checkResult, object result) =>
            checkResult && this.ResultRetryStrategy != null
                ? this.ResultRetryStrategy(currentAttempt, result)
                : this.RetryStrategy(currentAttempt);

        public RetryConfiguration UntilAttemptCountReaches(int numOfAttempts = 1)
        {
            this.RetryCount = numOfAttempts;
            return this;
        }

        public RetryConfiguration WaitBetweenAttempts(Func<int, TimeSpan> retryStrategy)
        {
            this.RetryStrategy = retryStrategy;
            return this;
        }

        public RetryConfiguration WaitBetweenAttempts<TResult>(Func<int, TResult, TimeSpan> resultRetryStrategy)
        {
            this.ResultRetryStrategy = (attempt, result) => resultRetryStrategy(attempt, (TResult)result);
            return this;
        }

        public RetryConfiguration RetryWhen(Func<Exception, bool> retryPolicy)
        {
            Swap.SwapValue(ref this.retryPolicies, policies => policies.Add(retryPolicy));
            return this;
        }

        public RetryConfiguration RetryWhenResult<TResult>(Func<TResult, bool> resultPolicy)
        {
            Swap.SwapValue(ref this.resultPolicies, policies => policies.Add(result => resultPolicy((TResult)result)));
            return this;
        }
    }
}
