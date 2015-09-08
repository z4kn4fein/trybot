
using System;

namespace Trybot.Interfaces
{
    public interface IRetryPolicy
    {
        bool ShouldRetryAfter(Exception exception);
    }
}
