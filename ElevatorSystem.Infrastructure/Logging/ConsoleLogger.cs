using System;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Infrastructure.Logging
{
    public sealed class ConsoleLogger : ILogger
    {
        private readonly object _lock = new();

        private void Log(string level, string message)
        {
            lock (_lock)
            {
                Console.WriteLine($"{DateTime.UtcNow:O} [{level}] {message}");
            }
        }

        public void Info(string message)  => Log("INFO ", message);
        public void Debug(string message) => Log("DEBUG", message);
        public void Warn(string message)  => Log("WARN ", message);
        public void Error(string message) => Log("ERROR", message);
    }
}