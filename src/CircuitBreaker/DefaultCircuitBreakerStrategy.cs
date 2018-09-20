using System;
using System.Threading;

namespace Trybot.CircuitBreaker
{
    internal class DefaultCircuitBreakerStrategy : CircuitBreakerStrategy
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
            if (Interlocked.Increment(ref this.currentFailedOperationCount) < this.maxFailureCountBeforeOpen) return;
            this.Reset();
            base.Switcher.Open(this.openStateDuration);
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
            if (Interlocked.Increment(ref this.currentSuccededOperationCount) < this.minSuccessCountBeforeClose) return;
            this.Reset();
            base.Switcher.Close();
        }

        private void Reset()
        {
            this.currentSuccededOperationCount = 0;
            this.currentFailedOperationCount = 0;
        }
    }
}