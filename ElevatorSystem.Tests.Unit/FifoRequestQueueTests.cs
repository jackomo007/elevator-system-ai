using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.ValueObjects;
using ElevatorSystem.Infrastructure.Services;
using Xunit;

namespace ElevatorSystem.Tests.Unit
{
    public class FifoRequestQueueTests
    {
        [Fact]
        public void EnqueueAndDequeue_PreservesOrder()
        {
            var queue = new FifoRequestQueue();
            var req1 = new ElevatorRequest(new Floor(1), Direction.Up);
            var req2 = new ElevatorRequest(new Floor(2), Direction.Down);

            queue.Enqueue(req1);
            queue.Enqueue(req2);

            queue.TryDequeue(out var first);
            queue.TryDequeue(out var second);

            Assert.Same(req1, first);
            Assert.Same(req2, second);
        }
    }
}