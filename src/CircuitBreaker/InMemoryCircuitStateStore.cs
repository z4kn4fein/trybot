using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.CircuitBreaker
{
    internal class InMemoryCircuitStateStore : ICircuitStateHandler
    {
        private CircuitState storedState = CircuitState.Closed;

        public CircuitState Read() => this.storedState;

        public void Update(CircuitState state) =>
            Swap.SwapValue(ref this.storedState, state);

        public Task<CircuitState> ReadAsync(CancellationToken token, bool continueOnCapturedContext) =>
            Task.FromResult(this.storedState);

        public Task UpdateAsync(CircuitState state, CancellationToken token, bool continueOnCapturedContext)
        {
            Swap.SwapValue(ref this.storedState, state);
            return Constants.CompletedTask;
        }
    }
}
