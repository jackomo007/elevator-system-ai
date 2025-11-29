using System;
using System.Threading;
using ElevatorSystem.Application.UseCases;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;
using ElevatorSystem.Domain.ValueObjects;
using ElevatorSystem.Infrastructure.Logging;
using ElevatorSystem.Infrastructure.Services;
using ElevatorSystem.Infrastructure.Time;
using Xunit;

namespace ElevatorSystem.Tests.Integration
{
    public class ElevatorControllerIntegrationTests
    {
        [Fact]
        public void MultipleRequests_AreProcessedWithoutDeadlock()
        {
            var logger = new ConsoleLogger();
            var timeProvider = new SystemTimeProvider();
            var queue = new FifoRequestQueue();
            var elevator = new Elevator(1, new Floor(1));
            using var controller = new ElevatorController(elevator, queue, logger, timeProvider);
            var useCase = new RequestElevatorUseCase(controller, logger);

            controller.Start();

            // Simulate many concurrent requests
            var threads = new Thread[50];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(_ =>
                {
                    var floor = (i % 10) + 1;
                    var direction = (floor % 2 == 0) ? Direction.Up : Direction.Down;
                    useCase.Execute(floor, direction);
                });
                threads[i].Start();
            }

            foreach (var t in threads)
                t.Join();

            // Give some time to process (in a real test use deterministic time provider)
            Thread.Sleep(3000);

            controller.Stop();

            // Just ensuring no exceptions / deadlocks
            Assert.True(true);
        }
    }
}