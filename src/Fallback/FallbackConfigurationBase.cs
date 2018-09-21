using System;

namespace Trybot.Fallback
{
    /// <summary>
    /// Contains the shared members of the different types of fallback configurations.
    /// </summary>
    public class FallbackConfigurationBase
    {
        internal Func<Exception, bool> FallbackPolicy { get; set; }

        internal bool HandlesException(Exception exception) =>
            this.FallbackPolicy?.Invoke(exception) ?? false;
    }
}
