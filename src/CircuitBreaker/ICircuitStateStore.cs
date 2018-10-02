using System.Threading;
using System.Threading.Tasks;

namespace Trybot.CircuitBreaker
{
    /// <summary>
    /// Represents a circuite state handler which will be used to update and read the circuit state.
    /// </summary>
    public interface ICircuitStateHandler
    {
        /// <summary>
        /// Reads the circuit state.
        /// </summary>
        /// <returns>The state.</returns>
        CircuitState Read();

        /// <summary>
        /// Updates the circuit state.
        /// </summary>
        /// <param name="state">The state.</param>
        void Update(CircuitState state);

        /// <summary>
        /// Reads the circuit state asynchronously.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <param name="continueOnCapturedContext">True if the underlying async call should continue on a captured synchronization context.</param>
        /// <returns>The state.</returns>
        Task<CircuitState> ReadAsync(CancellationToken token, bool continueOnCapturedContext);

        /// <summary>
        /// Updates the circuit state asynchronously.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="continueOnCapturedContext">True if the underlying async call should continue on a captured synchronization context.</param>
        Task UpdateAsync(CircuitState state, CancellationToken token, bool continueOnCapturedContext);
    }
}
