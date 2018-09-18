using System;
using System.Threading;
using Trybot.CircuitBreaker.Exceptions;
using Trybot.Utils;

namespace Trybot.CircuitBreaker
{
    internal class CircuitBreakerController : ICircuitBreakerStateSwitcher
    {
        private readonly ICircuitBreakerStrategy strategy;
        private readonly ICircuitStateStore stateStore;

        private DateTimeOffset openStateEndTime;

        public CircuitBreakerController(Func<ICircuitBreakerStateSwitcher, ICircuitBreakerStrategy> strategyFactory, ICircuitStateStore stateStore)
        { 
            Shield.EnsureNotNull(strategyFactory, nameof(strategyFactory));
            Shield.EnsureNotNull(stateStore, nameof(stateStore));

            this.strategy = strategyFactory(this);
            this.stateStore = stateStore;
        }

        public void PreCheckCircuitState()
        {
            var state = this.stateStore.Get();
            var openDuration = this.openStateEndTime - DateTimeOffset.UtcNow;
            if(state == CircuitState.Open && openDuration > TimeSpan.Zero)
                throw new CircuitOpenException(Constants.CircuitOpenExceptionMessage, openDuration);
            
            if(state == CircuitState.Open && openDuration <= TimeSpan.Zero)
                this.stateStore.Set(CircuitState.HalfOpen);
        }

        public void OperationSucceeded()
        {
            var state = this.stateStore.Get();
            if(state == CircuitState.Open)
                throw new InvalidOperationException(Constants.OperationExecutedInOpenStateMessage);

            if(state == CircuitState.Closed)
                this.strategy.OperationSucceededInClosed();

            if(state == CircuitState.HalfOpen)
                this.strategy.OperationFailedInHalfOpen();
        }

        public void OperationFailed()
        {
            var state = this.stateStore.Get();
            if(state == CircuitState.Open)
                throw new InvalidOperationException(Constants.OperationExecutedInOpenStateMessage);

            if(state == CircuitState.Closed)
                this.strategy.OperationSucceededInClosed();

            if(state == CircuitState.HalfOpen)
                this.strategy.OperationFailedInHalfOpen();
        }

        public void Close() =>
            this.stateStore.Set(CircuitState.Closed);

        public void Open(TimeSpan duration)
        {
            this.openStateEndTime = DateTimeOffset.UtcNow.Add(duration);
            this.stateStore.Set(CircuitState.Open);
        }
    }
}