using Trybot.Utils;

namespace Trybot.CircuitBreaker
{
    internal class InMemoryCircuitStateStore : ICircuitStateStore
    {
        private CircuitState storedState = CircuitState.Closed;

        public CircuitState Get() => this.storedState;

        public void Set(CircuitState state) =>
            Swap.SwapValue(ref this.storedState, state);
    }
}
