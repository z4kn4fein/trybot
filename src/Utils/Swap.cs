using System;
using System.Threading;

namespace Trybot.Utils
{
    internal static class Swap
    {
        public static void SwapValue<TValue>(ref TValue refValue, TValue newValue)
            where TValue : class
        {
            var currentValue = refValue;
            if (!TrySwapCurrent(ref refValue, currentValue, newValue))
                SwapCurrent(ref refValue, newValue);
        }

        public static void SwapValue<TValue, TArg>(ref TValue refValue, Func<TValue, TArg, TValue> valueFactory, TArg arg)
            where TValue : class
        {
            var currentValue = refValue;
            var newValue = valueFactory(currentValue, arg);

            if (!TrySwapCurrent(ref refValue, currentValue, newValue))
                SwapCurrent(ref refValue, valueFactory, arg);
        }

        private static bool TrySwapCurrent<TValue>(ref TValue refValue, TValue currentValue, TValue newValue)
            where TValue : class =>
            ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue);

        private static void SwapCurrent<TValue>(ref TValue refValue, TValue newValue)
            where TValue : class
        {
            var wait = new SpinWait();
            TValue currentValue;
            var counter = 0;

            do
            {
                counter++;
                CheckSwapQuota(counter, wait);
                currentValue = refValue;
            } while (!ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue));
        }

        private static void SwapCurrent<TValue, TArg>(ref TValue refValue, Func<TValue, TArg, TValue> valueFactory, TArg arg)
            where TValue : class
        {
            var wait = new SpinWait();
            TValue currentValue;
            TValue newValue;
            var counter = 0;

            do
            {
                counter++;
                CheckSwapQuota(counter, wait);
                currentValue = refValue;
                newValue = valueFactory(currentValue, arg);
            } while (!ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue));
        }

        private static void CheckSwapQuota(int counter, SpinWait wait)
        {
            if (counter > 50)
                throw new InvalidOperationException("Swap quota exceeded.");

            if (counter > 20)
                wait.SpinOnce();
        }
    }
}
