using System;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Infrastructure.Time
{
    public sealed class SystemTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}