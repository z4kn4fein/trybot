using System.Threading.Tasks;

namespace Trybot
{
    /// <summary>
    /// Represents a bot policy configuration.
    /// </summary>
    public class BotPolicyConfiguration
    {
        /// <summary>
        /// Represents the value which will be passed to the <see cref="Task.ConfigureAwait"/> method in case of awaited asynchronous calls.
        /// </summary>
        public bool ContinueOnCapturedContext { get; internal set; }
    }
}
