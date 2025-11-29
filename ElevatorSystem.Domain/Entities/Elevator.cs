using System;
using System.Collections.Generic;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.ValueObjects;

namespace ElevatorSystem.Domain.Entities
{
    /// <summary>
    /// Domain model for a single elevator.
    /// It does NOT handle threading; it only changes state.
    /// </summary>
    public sealed class Elevator
    {
        private readonly List<Floor> _targetFloors = new();

        public int Id { get; }
        public Floor CurrentFloor { get; private set; }
        public ElevatorState State { get; set; }

        public IReadOnlyCollection<Floor> TargetFloors => _targetFloors.AsReadOnly();

        public Elevator(int id, Floor initialFloor)
        {
            Id = id;
            CurrentFloor = initialFloor;
            State = ElevatorState.Idle;
        }

        /// <summary>
        /// Move one floor up.
        /// The caller must validate that this move is allowed.
        /// </summary>
        public void MoveUp()
        {
            if (State == ElevatorState.DoorOpen)
                throw new InvalidOperationException("Cannot move while doors are open.");

            if (CurrentFloor.Value >= Floor.MaxFloor)
                throw new InvalidOperationException("Already at maximum floor.");

            State = ElevatorState.MovingUp;
            CurrentFloor = new Floor(CurrentFloor.Value + 1);
        }

        /// <summary>
        /// Move one floor down.
        /// </summary>
        public void MoveDown()
        {
            if (State == ElevatorState.DoorOpen)
                throw new InvalidOperationException("Cannot move while doors are open.");

            if (CurrentFloor.Value <= Floor.MinFloor)
                throw new InvalidOperationException("Already at minimum floor.");

            State = ElevatorState.MovingDown;
            CurrentFloor = new Floor(CurrentFloor.Value - 1);
        }

        /// <summary>
        /// Opens the door at the current floor.
        /// </summary>
        public void OpenDoor()
        {
            State = ElevatorState.DoorOpen;
        }

        /// <summary>
        /// Closes the door and returns to idle.
        /// </summary>
        public void CloseDoor()
        {
            if (State != ElevatorState.DoorOpen)
                return;

            State = ElevatorState.Idle;
        }

        /// <summary>
        /// Adds a new target floor if not already in targets and not equal to current.
        /// </summary>
        public void AddTargetFloor(Floor floor)
        {
            if (floor.Equals(CurrentFloor))
                return;

            if (!_targetFloors.Contains(floor))
            {
                _targetFloors.Add(floor);
            }
        }

        public void RemoveTargetFloor(Floor floor)
        {
            _targetFloors.Remove(floor);
        }
    }
}
