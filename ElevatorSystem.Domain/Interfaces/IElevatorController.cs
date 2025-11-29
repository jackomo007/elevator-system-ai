using ElevatorSystem.Domain.Enums;

namespace ElevatorSystem.Domain.Interfaces
{
    public interface IElevatorController
    {
        void RequestElevator(int floor, Direction direction);
        void AddDestination(int destinationFloor);
    }
}