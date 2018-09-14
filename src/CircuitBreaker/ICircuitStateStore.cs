namespace Trybot.CircuitBreaker
{
    public interface ICircuitStateStore
    {
        CircuitState Get();

        void Set(CircuitState state);
    }
}
