namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents the configuration of the default circuit breaker strategy used by the circuit breaker bot.
    /// </summary>
    public class DefaultCircuitBreakerStrategyConfiguration
    {
        internal int MaxFailureCountBeforeOpen { get; private set; }

        internal int MinSuccessCountBeforeClose { get; private set; }

        /// <summary>
        /// Sets the maximum number of failed operations before the circuit breaker turns into the Open state.
        /// </summary>
        /// <param name="failureThresholdBeforeOpen">The maximum number of failed calls.</param>
        /// <returns>Itself because of the fluent api.</returns>
        public DefaultCircuitBreakerStrategyConfiguration FailureThresholdBeforeOpen(int failureThresholdBeforeOpen)
        {
            this.MaxFailureCountBeforeOpen = failureThresholdBeforeOpen;
            return this;
        }

        /// <summary>
        /// Sets the minimum number of succeeded operations should be reached when the circuit breaker 
        /// is in HalfOpen state before turning into Closed.
        /// </summary>
        /// <param name="minSuccessCountBeforeClose"></param>
        /// <returns></returns>
        public DefaultCircuitBreakerStrategyConfiguration SuccessThresholdInHalfOpen(int minSuccessCountBeforeClose)
        {
            this.MinSuccessCountBeforeClose = minSuccessCountBeforeClose;
            return this;
        }
    }
}
