using System;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.UseCases
{
    /// <summary>
    /// Application-level orchestration for requesting the elevator.
    /// </summary>
    public sealed class RequestElevatorUseCase
    {
        private readonly IElevatorController _controller;
        private readonly ILogger _logger;

        public RequestElevatorUseCase(
            IElevatorController controller,
            ILogger logger)
        {
            _controller = controller;
            _logger = logger;
        }

        public void Execute(int floor, Direction direction)
        {
            try
            {
                _controller.RequestElevator(floor, direction);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.Warn($"Invalid floor request ({floor}): {ex.Message}");
            }
        }
    }
}