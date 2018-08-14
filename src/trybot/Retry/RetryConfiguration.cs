using System;
using System.Linq;
using Trybot.Utils;

namespace Trybot.Retry
{
    internal class RetryConfiguration
    {
        public int RetryCount { get; }

        public Func<int, TimeSpan> RetryStrategy { get; }

        public Func<int, object, TimeSpan> ResultRetryStrategy { get; }

        public ArrayStore<Func<Exception, bool>> RetryPolicies { get; }

        public ArrayStore<Func<object, bool>> ResultPolicies { get; }

        public bool HandlesException(Exception exception) =>
            this.RetryPolicies.Any(policy => policy(exception));

        public bool AcceptsResult(object result) =>
            this.ResultPolicies.All(policy => policy(result));

        public bool IsMaxAttemptsReached(int currentAttempt) =>
            currentAttempt >= this.RetryCount;
    }
}
