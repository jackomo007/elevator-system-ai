using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.ValueObjects;
using Xunit;

namespace ElevatorSystem.Tests.Unit
{
    public class ElevatorTests
    {
        [Fact]
        public void MoveUp_IncrementsFloorAndSetsState()
        {
            var elevator = new Elevator(id: 1, initialFloor: new Floor(1));

            elevator.MoveUp();

            Assert.Equal(2, elevator.CurrentFloor.Value);
            Assert.Equal(ElevatorState.MovingUp, elevator.State);
        }

        [Fact]
        public void MoveDown_DecrementsFloorAndSetsState()
        {
            var elevator = new Elevator(id: 1, initialFloor: new Floor(5));

            elevator.MoveDown();

            Assert.Equal(4, elevator.CurrentFloor.Value);
            Assert.Equal(ElevatorState.MovingDown, elevator.State);
        }

        [Fact]
        public void OpenDoor_SetsDoorOpenState()
        {
            var elevator = new Elevator(id: 1, initialFloor: new Floor(3));

            elevator.OpenDoor();

            Assert.Equal(ElevatorState.DoorOpen, elevator.State);
        }

        [Fact]
        public void CloseDoor_ReturnsToIdle()
        {
            var elevator = new Elevator(id: 1, initialFloor: new Floor(3));
            elevator.OpenDoor();

            elevator.CloseDoor();

            Assert.Equal(ElevatorState.Idle, elevator.State);
        }
    }
}