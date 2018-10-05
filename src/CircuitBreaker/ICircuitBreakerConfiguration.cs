using System;

namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents the fluent api of the circuit breaker configuration.
    /// </summary>
    public interface ICircuitBreakerConfiguration<out TConfiguration>
    {
        /// <summary>
        /// Sets the delegate which will be used to determine whether the given operation should be
        /// marked as failed by the <see cref="CircuitBreakerStrategy" /> or not when a specific exception occurs.
        /// </summary>
        /// <param name="exceptionPolicy">The determination delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.BrakeWhenExceptionOccurs(exception => exception is HttpRequestException)</code></example>
        TConfiguration BrakeWhenExceptionOccurs(Func<Exception, bool> exceptionPolicy);

        /// <summary>
        /// Sets a the underlying circuit state handler implementation.
        /// </summary>
        /// <param name="stateHandler">The state handler.</param>
        /// /// <returns>Itself because of the fluent api.</returns>
        TConfiguration WithStateHandler(ICircuitStateHandler stateHandler);

        /// <summary>
        /// Sets the delegate which will be invoked when the circuit breaker trips to the open state.
        /// </summary>
        /// <param name="openHandler">The action to be invoked.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnOpen(openDuration => holdExecutionUntil(openDuration))</code></example>
        TConfiguration OnOpen(Action<TimeSpan> openHandler);

        /// <summary>
        /// Sets the delegate which will be invoked when the circuit breaker trips to the closed state.
        /// </summary>
        /// <param name="closedHandler">The action to be invoked.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnClosed(() => onClosedAction())</code></example>
        TConfiguration OnClosed(Action closedHandler);

        /// <summary>
        /// Sets the delegate which will be invoked when the circuit breaker trips to the closed state.
        /// </summary>
        /// <param name="halfOpenHandler">The action to be invoked.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.OnHalfOpen(() => onHalfOpenAction())</code></example>
        TConfiguration OnHalfOpen(Action halfOpenHandler);

        /// <summary>
        /// Sets the amount of time of how long the circuit breaker should remain in the Open state before turning into HalfOpen.
        /// </summary>
        /// <param name="openStateDuration">The open state duration.</param>
        /// <returns></returns>
        /// <returns>Itself because of the fluent api.</returns>
        TConfiguration DurationOfOpen(TimeSpan openStateDuration);
    }

    /// <summary>
    /// Represents the fluent api of the circuit breaker configuration.
    /// </summary>
    public interface ICircuitBreakerConfiguration<out TConfiguration, out TResult> : ICircuitBreakerConfiguration<TConfiguration>
    {
        /// <summary>
        /// Sets the delegate which will be used to determine whether the given operation should be
        /// marked as failed by the <see cref="CircuitBreakerStrategy" /> or not based on its return value.
        /// </summary>
        /// <param name="resultPolicy">The determination delegate.</param>
        /// <returns>Itself because of the fluent api.</returns>
        /// <example><code>config.BrakeWhenResultIs(result => result != OperationResult.Ok)</code></example>
        TConfiguration BrakeWhenResultIs(Func<TResult, bool> resultPolicy);
    }
}