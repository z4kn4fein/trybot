using System.Threading;

namespace Trybot.CircuitBreaker
{
    internal class DefaultCircuitBreakerStrategy : CircuitBreakerStrategy
    {
        private readonly int maxFailureCountBeforeOpen;
        private readonly int minSuccessCountBeforeClose;

        private int currentFailedOperationCount;
        private int currentSuccededOperationCount;

        public DefaultCircuitBreakerStrategy(CircuitBreakerConfigurationBase configBase,
            DefaultCircuitBreakerStrategyConfiguration config) : base(configBase)
        {
            this.maxFailureCountBeforeOpen = config.MaxFailureCountBeforeOpen;
            this.minSuccessCountBeforeClose = config.MinSuccessCountBeforeClose;
        }

        protected override bool OperationFailedInClosed()
        {
            if (Interlocked.Increment(ref this.currentFailedOperationCount) < this.maxFailureCountBeforeOpen)
                return false;

            this.Reset();
            return true;

        }

        protected override bool OperationFailedInHalfOpen() => true;

        protected override bool OperationSucceededInHalfOpen()
        {
            if (Interlocked.Increment(ref this.currentSuccededOperationCount) < this.minSuccessCountBeforeClose)
                return false;

            this.Reset();
            return true;
        }

        protected override void Reset()
        {
            this.currentSuccededOperationCount = 0;
            this.currentFailedOperationCount = 0;
        }
    }
}