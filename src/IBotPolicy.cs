using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

namespace Trybot
{
    public interface IBotPolicy
    {
        void Execute(IBotOperation operation, CancellationToken token);

        Task ExecuteAsync(IAsyncBotOperation operation, CancellationToken token);
    }

    public interface IBotPolicy<TResult>
    {
        TResult Execute(IBotOperation<TResult> operation, CancellationToken token);

        Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, CancellationToken token);
    }
}
