using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    internal static class TaskDelayer
    {
        public static async Task Sleep(TimeSpan timeSpan, CancellationToken token)
        {
            try
            {
                await Task.Delay(timeSpan, token);
            }
            catch (OperationCanceledException)
            { }
        }

        public static async Task Sleep(int milliseconds, CancellationToken token)
        {
            await Sleep(TimeSpan.FromMilliseconds(milliseconds), token);
        }
    }
}
