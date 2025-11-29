using System;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.ValueObjects;

namespace ElevatorSystem.Domain.Entities
{
    /// <summary>
    /// Represents a passenger request: pickup + optional destination.
    /// </summary>
    public sealed class ElevatorRequest
    {
        public Floor PickupFloor { get; }
        public Floor? DestinationFloor { get; }
        public Direction Direction { get; }
        public DateTime CreatedAt { get; }

        public ElevatorRequest(
            Floor pickupFloor,
            Direction direction,
            Floor? destinationFloor = null,
            DateTime? createdAt = null)
        {
            PickupFloor = pickupFloor;
            Direction = direction;
            DestinationFloor = destinationFloor;
            CreatedAt = createdAt ?? DateTime.UtcNow;
        }

        public override string ToString()
        {
            var dest = DestinationFloor.HasValue ? DestinationFloor.Value.Value.ToString() : "-";
            return $"Pickup={PickupFloor.Value}, Dest={dest}, Dir={Direction}, At={CreatedAt:O}";
        }
    }
}