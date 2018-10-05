using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trybot.CircuitBreaker;

namespace Trybot.DistributedCB
{
    public class DistributedCircuitStateHandler : ICircuitStateHandler
    {
        private enum InternalState
        {
            None,
            DistributedOpen,
            RecoveredFromDistributedOpen
        }

        private readonly ConcurrentDictionary<string, CircuitState> distributedStates;
        private readonly string key;
        private readonly double openStatePercentageIndicator;
        private readonly TimeSpan healDuration;

        private long healEndTimeTicks;

        private CircuitState localCircuitState = CircuitState.Closed;
        private InternalState internalState = InternalState.None;

        public DistributedCircuitStateHandler(ConcurrentDictionary<string, CircuitState> distributedStates, string key,
            double openStatePercentageIndicator, TimeSpan healDuration)
        {
            this.distributedStates = distributedStates;
            this.key = key;
            this.openStatePercentageIndicator = openStatePercentageIndicator;
            this.healDuration = healDuration;
            this.distributedStates.AddOrUpdate(this.key, CircuitState.Closed, (s, circuitState) => CircuitState.Closed);
        }

        public CircuitState Read()
        {
            // when the local state is not closed we don't want to check
            // the distributed states
            if (this.localCircuitState != CircuitState.Closed)
                return this.localCircuitState;

            // when we are within the healing state we are skipping
            // the distributed state check
            if (this.healEndTimeTicks - DateTimeOffset.UtcNow.Ticks > 0)
                return this.localCircuitState;

            // check whether we should react to the actual distributed state
            var distributedCheckResult = this.CheckDistributedState();

            // set it's result to the local variant
            this.Update(distributedCheckResult);

            return distributedCheckResult;
        }

        public void Update(CircuitState state)
        {
            // check whether we are in a recovery phase
            this.CheckDistributedRecovery(state);

            this.localCircuitState = state;
            this.distributedStates.AddOrUpdate(this.key, state, (s, circuitState) => state);
        }

        public Task<CircuitState> ReadAsync(CancellationToken token, bool continueOnCapturedContext)
        {
            return Task.FromResult(this.Read());
        }

        public Task UpdateAsync(CircuitState state, CancellationToken token, bool continueOnCapturedContext)
        {
            this.Update(state);
            return Task.FromResult(0);
        }

        private CircuitState CheckDistributedState()
        {
            var count = (double)this.distributedStates.Count;
            var closedCount = (double)this.distributedStates.Count(s => s.Value == CircuitState.Open);
            var percentage = closedCount / count;
            if (percentage > this.openStatePercentageIndicator)
            {
                this.internalState = InternalState.DistributedOpen;
                return CircuitState.Open;
            }

            return CircuitState.Closed;
        }

        private void CheckDistributedRecovery(CircuitState state)
        {
            if (this.internalState == InternalState.DistributedOpen && state == CircuitState.Closed)
            {
                this.internalState = InternalState.RecoveredFromDistributedOpen;
                this.healEndTimeTicks = DateTimeOffset.UtcNow.Add(this.healDuration).Ticks;
            }
        }
    }
}
