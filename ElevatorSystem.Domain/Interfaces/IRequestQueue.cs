using ElevatorSystem.Domain.Entities;

namespace ElevatorSystem.Domain.Interfaces
{
    /// <summary>
    /// Scheduling abstraction: how elevator requests are queued and dequeued.
    /// FIFO, SCAN, LOOK etc. can all implement this.
    /// </summary>
    public interface IRequestQueue
    {
        void Enqueue(ElevatorRequest request);
        bool TryDequeue(out ElevatorRequest? request);
        int Count { get; }
        ElevatorRequest[] Snapshot();
    }
}