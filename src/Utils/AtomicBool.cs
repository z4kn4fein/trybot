using System.Threading;

namespace Trybot.Utils
{
    internal class AtomicBool
    {

        private const int ValueTrue = 1;
        private const int ValueFalse = 0;

        private int currentValue;
        
        public AtomicBool(bool initialValue = false)
        {
            this.currentValue = BoolToInt(initialValue);
        }

        private static int BoolToInt(bool value)
        {
            return value ? ValueTrue : ValueFalse;
        }
        
        public void SetValue(bool value)
        {
            this.currentValue = BoolToInt(value);
        }
        
        public bool CompareExchange(bool expectedValue, bool newValue)
        {
            var expectedVal = BoolToInt(expectedValue);
            var newVal = BoolToInt(newValue);
            return Interlocked.CompareExchange(ref this.currentValue, newVal, expectedVal) == expectedVal;
        }
    }
}
