using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot
{
    public static class TaskDelayer
    {
        public async static Task Sleep(TimeSpan timeSpan, CancellationToken token)
        {
            try
            {
                await Task.Delay(timeSpan, token);
            }
            catch (OperationCanceledException)
            { }
        }

        public async static Task Sleep(int milliseconds, CancellationToken token)
        {
            await Sleep(TimeSpan.FromMilliseconds(milliseconds), token);
        }
    }
}
