namespace Trybot.CircuitBreaker
{
    public abstract class CircuitBreakerStrategy : ICircuitBreakerStrategy
    {
        protected ICircuitBreakerStateSwitcher Switcher { get; }

        protected CircuitBreakerStrategy(ICircuitBreakerStateSwitcher switcher)
        {
            this.Switcher = switcher;
        }

        public abstract void OperationFailedInClosed();
        public abstract void OperationFailedInHalfOpen();
        public abstract void OperationSucceededInClosed();
        public abstract void OperationSucceededInHalfOpen();
    }
}
