using System;

namespace ElevatorSystem.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a floor, with validation.
    /// </summary>
    public readonly struct Floor : IEquatable<Floor>, IComparable<Floor>
    {
        public int Value { get; }
        public static int MinFloor => 1;
        public static int MaxFloor => 10;

        public Floor(int value)
        {
            if (value < MinFloor || value > MaxFloor)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Floor must be between {MinFloor} and {MaxFloor}.");
            }

            Value = value;
        }

        public int CompareTo(Floor other) => Value.CompareTo(other.Value);
        public bool Equals(Floor other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Floor f && Equals(f);
        public override int GetHashCode() => Value;
        public override string ToString() => Value.ToString();

        public static implicit operator int(Floor floor) => floor.Value;
        public static implicit operator Floor(int value) => new Floor(value);
    }
}