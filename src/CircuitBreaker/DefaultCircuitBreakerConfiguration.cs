using System;

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
}
