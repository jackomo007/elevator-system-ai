using System.Collections.Concurrent;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Infrastructure.Services
{
    /// <summary>
    /// Thread-safe FIFO queue for elevator requests.
    /// </summary>
    public sealed class FifoRequestQueue : IRequestQueue
    {
        private readonly ConcurrentQueue<ElevatorRequest> _queue = new();

        public void Enqueue(ElevatorRequest request)
        {
            _queue.Enqueue(request);
        }

        public bool TryDequeue(out ElevatorRequest? request)
        {
            var success = _queue.TryDequeue(out var value);
            request = value; // can be null if failed
            return success;
        }

        public int Count => _queue.Count;

        public ElevatorRequest[] Snapshot() => _queue.ToArray();
    }
}