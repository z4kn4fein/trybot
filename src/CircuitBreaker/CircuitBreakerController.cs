using System;
using Trybot.CircuitBreaker.Exceptions;
using Trybot.Utils;

namespace Trybot.CircuitBreaker
{
    internal class CircuitBreakerController : ICircuitBreakerStateSwitcher
    {
        private readonly CircuitBreakerConfigurationBase configuration;
        private readonly ICircuitBreakerStrategy strategy;
        private readonly ICircuitStateStore stateStore;

        private DateTimeOffset openStateEndTime;

        public CircuitBreakerController(Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory, CircuitBreakerConfigurationBase configuration)
        {
            this.configuration = configuration;
            Shield.EnsureNotNull(strategyFactory, nameof(strategyFactory));
            Shield.EnsureNotNull(configuration, nameof(configuration));
            Shield.EnsureNotNull(configuration.StateStore, nameof(configuration.StateStore));

            this.strategy = strategyFactory(this);
            this.stateStore = configuration.StateStore;
        }

        public bool PreCheckCircuitState()
        {
            var state = this.stateStore.Get();
            if (state == CircuitState.Closed)
                return false;

            if (state == CircuitState.HalfOpen)
                return true;

            var openDuration = this.openStateEndTime - DateTimeOffset.UtcNow;
            if (openDuration > TimeSpan.Zero)
                throw new CircuitOpenException(Constants.CircuitOpenExceptionMessage, openDuration);

            this.HalfOpen();
            return true;
        }

        public void OperationSucceeded()
        {
            var state = this.stateStore.Get();
            if (state == CircuitState.Closed)
                this.strategy.OperationSucceededInClosed();

            if (state == CircuitState.HalfOpen)
                this.strategy.OperationSucceededInHalfOpen();
        }

        public void OperationFailed()
        {
            var state = this.stateStore.Get();
            if (state == CircuitState.Closed)
                this.strategy.OperationFailedInClosed();

            if (state == CircuitState.HalfOpen)
                this.strategy.OperationFailedInHalfOpen();
        }

        public void Close()
        {
            this.stateStore.Set(CircuitState.Closed);
            this.configuration.ClosedStateHandler?.Invoke();
        }

        public void Open(TimeSpan duration)
        {
            this.openStateEndTime = DateTimeOffset.UtcNow.Add(duration);
            this.stateStore.Set(CircuitState.Open);
            this.configuration.OpenStateHandler?.Invoke(duration);
        }

        private void HalfOpen()
        {
            this.stateStore.Set(CircuitState.HalfOpen);
            this.configuration.HalfOpenStateHandler?.Invoke();
        }
    }
}