using System;
using System.Threading;

namespace Trybot.Utils
{
    internal static class Swap
    {
        public static TResult SwapValue<TValue, TResult>(ref TValue refValue, Func<TValue, Tuple<TValue, TResult>> valueFactory)
            where TValue : class
        {
            var currentValue = refValue;
            var tuple = valueFactory(currentValue);

            return !TrySwapCurrent(ref refValue, currentValue, tuple.Item1)
                ? SwapCurrent(ref refValue, valueFactory)
                : tuple.Item2;
        }

        public static void SwapValue<TValue>(ref TValue refValue, Func<TValue, TValue> valueFactory)
            where TValue : class
        {
            var currentValue = refValue;
            var newValue = valueFactory(currentValue);

            if (!TrySwapCurrent(ref refValue, currentValue, newValue))
                SwapCurrent(ref refValue, valueFactory);
        }

        private static bool TrySwapCurrent<TValue>(ref TValue refValue, TValue currentValue, TValue newValue)
            where TValue : class =>
            ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue);

        private static TResult SwapCurrent<TValue, TResult>(ref TValue refValue, Func<TValue, Tuple<TValue, TResult>> valueFactory)
            where TValue : class
        {
            var wait = new SpinWait();
            TValue currentValue;
            TValue newValue;
            TResult result;
            var counter = 0;

            do
            {
                counter++;
                CheckQuota(counter, wait);

                currentValue = refValue;
                var tuple = valueFactory(currentValue);
                newValue = tuple.Item1;
                result = tuple.Item2;
            } while (!ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue));

            return result;
        }

        private static void SwapCurrent<TValue>(ref TValue refValue, Func<TValue, TValue> valueFactory)
            where TValue : class
        {
            var wait = new SpinWait();
            TValue currentValue;
            TValue newValue;
            var counter = 0;

            do
            {
                counter++;
                CheckQuota(counter, wait);

                currentValue = refValue;
                newValue = valueFactory(currentValue);
            } while (!ReferenceEquals(Interlocked.CompareExchange(ref refValue, newValue, currentValue), currentValue));
        }

        private static void CheckQuota(int counter, SpinWait wait)
        {
            if (counter > 50)
                throw new InvalidOperationException("Swap quota exceeded.");

            if (counter > 20)
                wait.SpinOnce();
        }
    }
}
