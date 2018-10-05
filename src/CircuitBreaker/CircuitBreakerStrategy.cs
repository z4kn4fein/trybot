using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.CircuitBreaker.Exceptions;
using Trybot.Utils;

namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents an abstract circuit breaker strategy.
    /// </summary>
    public abstract class CircuitBreakerStrategy
    {
        private readonly ICircuitStateHandler stateHandler;
        private readonly CircuitBreakerConfigurationBase configuration;
        private long openStateEndTimeTicks;

        private const long DefaultOpenEndTimeTicks = 0;

        /// <summary>
        /// Constructs a <see cref="CircuitBreakerStrategy"/> implementation.
        /// </summary>
        /// <param name="configuration">The circuit breaker configuration.</param>
        protected CircuitBreakerStrategy(CircuitBreakerConfigurationBase configuration)
        {
            this.stateHandler = configuration.StateHandler;
            this.configuration = configuration;
        }

        internal bool PreCheckCircuitState()
        {
            var state = this.stateHandler.Read();
            if (state == CircuitState.Closed)
                return false;

            if (state == CircuitState.HalfOpen)
                return true;

            this.HandleOpenState();

            this.HalfOpen();
            return true;
        }

        internal async Task<bool> PreCheckCircuitStateAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            var state = await this.stateHandler.ReadAsync(token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);

            if (state == CircuitState.Closed)
                return false;

            if (state == CircuitState.HalfOpen)
                return true;

            this.HandleOpenState();

            await this.HalfOpenAsync(token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);

            return true;
        }

        internal void OperationSucceeded()
        {
            var state = this.stateHandler.Read();
            if (state == CircuitState.Closed)
                this.Reset();

            if (state == CircuitState.HalfOpen && this.OperationSucceededInHalfOpen())
                this.Close();
        }

        internal async Task OperationSucceededAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            var state = await this.stateHandler.ReadAsync(token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
            if (state == CircuitState.Closed)
                this.Reset();

            if (state == CircuitState.HalfOpen && this.OperationSucceededInHalfOpen())
                await this.CloseAsync(token, continueOnCapturedContext)
                     .ConfigureAwait(continueOnCapturedContext);
        }

        internal void OperationFailed()
        {
            var state = this.stateHandler.Read();
            if (state == CircuitState.Closed && this.OperationFailedInClosed() ||
                state == CircuitState.HalfOpen && this.OperationFailedInHalfOpen())
                this.Open();
        }

        internal async Task OperationFailedAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            var state = await this.stateHandler.ReadAsync(token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);

            if (state == CircuitState.Closed && this.OperationFailedInClosed() ||
                state == CircuitState.HalfOpen && this.OperationFailedInHalfOpen())
                await this.OpenAsync(token, continueOnCapturedContext)
                    .ConfigureAwait(continueOnCapturedContext);
        }

        /// <summary>
        /// Called when the underlying operation is failed within the Closed circuit state.
        /// </summary>
        /// <returns>True if the circuit should open, otherwise false.</returns>
        protected abstract bool OperationFailedInClosed();

        /// <summary>
        /// Called when the underlying operation is failed within the HalfOpen circuit state.
        /// </summary>
        /// <returns>True if the circuit should move back to open state, otherwise false.</returns>
        protected abstract bool OperationFailedInHalfOpen();

        /// <summary>
        /// Called when the underlying operation is succeeded within the HalfOpen circuit state.
        /// </summary>
        /// <returns>True if the circuit should be closed, otherwise false.</returns>
        protected abstract bool OperationSucceededInHalfOpen();

        /// <summary>
        /// Called when the underlying operation is succeeded within the Closed circuit state.
        /// </summary>
        protected abstract void Reset();

        private void Close()
        {
            this.stateHandler.Update(CircuitState.Closed);
            this.OnClose();
        }

        private async Task CloseAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            await this.stateHandler.UpdateAsync(CircuitState.Closed, token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
            this.OnClose();
        }

        private void Open()
        {
            this.stateHandler.Update(CircuitState.Open);
            this.OnOpen();
        }

        private async Task OpenAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            await this.stateHandler.UpdateAsync(CircuitState.Open, token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
            this.OnOpen();
        }

        private void HalfOpen()
        {
            this.stateHandler.Update(CircuitState.HalfOpen);
            this.OnHalfOpen();
        }

        private async Task HalfOpenAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            await this.stateHandler.UpdateAsync(CircuitState.HalfOpen, token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
            this.OnHalfOpen();
        }

        private void OnClose()
        {
            this.openStateEndTimeTicks = DefaultOpenEndTimeTicks;
            this.configuration.ClosedStateHandler?.Invoke();
        }

        private void OnOpen()
        {
            if (Interlocked.CompareExchange(ref this.openStateEndTimeTicks,
                DateTimeOffset.UtcNow.Ticks + this.configuration.OpenStateDuration.Ticks, DefaultOpenEndTimeTicks) == 0)
                this.configuration.OpenStateHandler?.Invoke(this.configuration.OpenStateDuration);
        }

        private void OnHalfOpen()
        {
            this.openStateEndTimeTicks = DefaultOpenEndTimeTicks;
            this.configuration.HalfOpenStateHandler?.Invoke();
        }

        private void HandleOpenState()
        {
            // when the state handler indicates an open state but we did not recieved 
            // a local open request we should set the the open duration here
            if (Interlocked.CompareExchange(ref this.openStateEndTimeTicks,
                DateTimeOffset.UtcNow.Ticks + this.configuration.OpenStateDuration.Ticks, DefaultOpenEndTimeTicks) == 0)
                this.configuration.OpenStateHandler?.Invoke(this.configuration.OpenStateDuration);

            var remainingOpenDuration = this.openStateEndTimeTicks - DateTimeOffset.UtcNow.Ticks;
            if (remainingOpenDuration > 0)
                throw new CircuitOpenException(Constants.CircuitOpenExceptionMessage, TimeSpan.FromTicks(remainingOpenDuration));
        }
    }
}
