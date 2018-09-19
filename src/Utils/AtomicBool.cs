using System.Threading;

namespace Trybot.Utils
{
    /// <summary>
    /// Represents an atomic boolean implementation.
    /// </summary>
    public class AtomicBool
    {

        private const int ValueTrue = 1;
        private const int ValueFalse = 0;

        private int currentValue;

        /// <summary>
        /// Constructs an <see cref="AtomicBool"/>
        /// </summary>
        /// <param name="initialValue">The initial internal value.</param>
        public AtomicBool(bool initialValue = false)
        {
            this.currentValue = BoolToInt(initialValue);
        }

        private static int BoolToInt(bool value)
        {
            return value ? ValueTrue : ValueFalse;
        }

        /// <summary>
        /// Sets the value of the AtomicBool.
        /// </summary>
        public void SetValue(bool value)
        {
            this.currentValue = BoolToInt(value);
        }

        /// <summary>
        /// Compares the internal value with the expected value and if they matches the internal value will be replaced with the new value in one atomic operation.
        /// </summary>
        /// <param name="expectedValue">The expected value of the comparison.</param>
        /// <param name="newValue">The new value, the internal value will be replaced with this.</param>
        /// <returns></returns>
        public bool CompareExchange(bool expectedValue, bool newValue)
        {
            var expectedVal = BoolToInt(expectedValue);
            var newVal = BoolToInt(newValue);
            return Interlocked.CompareExchange(ref this.currentValue, newVal, expectedVal) == expectedVal;
        }
    }
}
