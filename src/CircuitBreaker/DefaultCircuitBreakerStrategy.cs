using System;
using System.Threading;

namespace Trybot.CircuitBreaker
{
    public class DefaultCircuitBreakerStrategyConfiguration
    {
        internal int MaxFailureCountBeforeOpen { get; private set; }

        internal TimeSpan OpenStateDuration { get; private set; }

        internal int MinSuccessCountBeforeClose { get; private set; }

        public DefaultCircuitBreakerStrategyConfiguration FailureThresholdBeforeOpen(int failureThresholdBeforeOpen)
        {
            this.MaxFailureCountBeforeOpen = failureThresholdBeforeOpen;
            return this;
        }

        public DefaultCircuitBreakerStrategyConfiguration DurationOfOpen(TimeSpan openStateDuration)
        {
            this.OpenStateDuration = openStateDuration;
            return this;
        }

        public DefaultCircuitBreakerStrategyConfiguration SuccessThresholdInHalfOpen(int minSuccessCountBeforeClose)
        {
            this.MinSuccessCountBeforeClose = minSuccessCountBeforeClose;
            return this;
        }
    }

    public class DefaultCircuitBreakerStrategy : CircuitBreakerStrategy
    {
        private readonly int maxFailureCountBeforeOpen;
        private readonly TimeSpan openStateDuration;
        private readonly int minSuccessCountBeforeClose;

        private int currentFailedOperationCount;
        private int currentSuccededOperationCount;

        public DefaultCircuitBreakerStrategy(ICircuitBreakerStateSwitcher switcher,
            DefaultCircuitBreakerStrategyConfiguration config) : base(switcher)
        { 
            this.maxFailureCountBeforeOpen = config.MaxFailureCountBeforeOpen;
            this.openStateDuration = config.OpenStateDuration;
            this.minSuccessCountBeforeClose = config.MinSuccessCountBeforeClose;
        }

        public override void OperationFailedInClosed()
        {
            if(Interlocked.Increment(ref this.currentFailedOperationCount) >= this.maxFailureCountBeforeOpen)
            {
                this.Reset();
                base.Switcher.Open(this.openStateDuration);
            }
        }

        public override void OperationFailedInHalfOpen()
        {
            this.Reset();
            base.Switcher.Open(this.openStateDuration);
        }

        public override void OperationSucceededInClosed() =>
            this.Reset();

        public override void OperationSucceededInHalfOpen()
        {
            if(Interlocked.Increment(ref this.currentSuccededOperationCount) >= this.minSuccessCountBeforeClose)
            {
                this.Reset();
                base.Switcher.Close();
            }
        }

        private void Reset()
        {
            this.currentSuccededOperationCount = 0;
            this.currentFailedOperationCount = 0;
        }
    }
}