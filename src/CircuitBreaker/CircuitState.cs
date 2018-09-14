namespace Trybot.CircuitBreaker
{
    public class CircuitState
    {
        public static readonly CircuitState Closed = new CircuitState();
        public static readonly CircuitState HalfOpen = new CircuitState();
        public static readonly CircuitState Open = new CircuitState();

        private CircuitState() { }
    }
}
