using System;

namespace ElevatorSystem.Domain.Interfaces
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}