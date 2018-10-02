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
    public abstract class CircuitBreakerStrategy : ICircuitBreakerStrategy
    {
        private readonly ICircuitStateHandler stateHandler;
        private readonly CircuitBreakerConfigurationBase configuration;
        private DateTimeOffset openStateEndTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        protected CircuitBreakerStrategy(CircuitBreakerConfigurationBase configuration)
        {
            this.stateHandler = configuration.StateHandler;
            this.configuration = configuration;
        }

        /// <inheritdoc />
        public bool PreCheckCircuitState()
        {
            var state = this.stateHandler.Read();
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

        /// <inheritdoc />
        public async Task<bool> PreCheckCircuitStateAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            var state = await this.stateHandler.ReadAsync(token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);

            if (state == CircuitState.Closed)
                return false;

            if (state == CircuitState.HalfOpen)
                return true;

            var openDuration = this.openStateEndTime - DateTimeOffset.UtcNow;
            if (openDuration > TimeSpan.Zero)
                throw new CircuitOpenException(Constants.CircuitOpenExceptionMessage, openDuration);

            await this.HalfOpenAsync(token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);

            return true;
        }

        /// <inheritdoc />
        public void OperationSucceeded()
        {
            var state = this.stateHandler.Read();
            if (state == CircuitState.Closed)
                this.Reset();

            if (state == CircuitState.HalfOpen && this.OperationSucceededInHalfOpen())
                this.Close();
        }

        /// <inheritdoc />
        public async Task OperationSucceededAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            var state = await this.stateHandler.ReadAsync(token, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
            if (state == CircuitState.Closed)
                this.Reset();

            if (state == CircuitState.HalfOpen && this.OperationSucceededInHalfOpen())
                await this.CloseAsync(token, continueOnCapturedContext)
                     .ConfigureAwait(continueOnCapturedContext);
        }

        /// <inheritdoc />
        public void OperationFailed()
        {
            var state = this.stateHandler.Read();
            if (state == CircuitState.Closed && this.OperationFailedInClosed() ||
                state == CircuitState.HalfOpen && this.OperationFailedInHalfOpen())
                this.Open();
        }

        /// <inheritdoc />
        public async Task OperationFailedAsync(CancellationToken token, bool continueOnCapturedContext)
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

        private void OnClose() =>
            this.configuration.ClosedStateHandler?.Invoke();

        private void OnOpen()
        {
            this.openStateEndTime = DateTimeOffset.UtcNow.Add(this.configuration.OpenStateDuration);
            this.configuration.OpenStateHandler?.Invoke(this.configuration.OpenStateDuration);
        }

        private void OnHalfOpen()
        {
            this.configuration.HalfOpenStateHandler?.Invoke();
        }
    }
}
