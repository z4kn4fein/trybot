using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Operations
{
    public interface IBotOperation
    {
        void Execute(ExecutionContext context, CancellationToken token);
    }

    public interface IAsyncBotOperation
    {
        Task ExecuteAsync(ExecutionContext context, CancellationToken token);
    }

    public interface IBotOperation<out TResult>
    {
        TResult Execute(ExecutionContext context, CancellationToken token);
    }

    public interface IAsyncBotOperation<TResult>
    {
        Task<TResult> ExecuteAsync(ExecutionContext context, CancellationToken token);
    }
}
