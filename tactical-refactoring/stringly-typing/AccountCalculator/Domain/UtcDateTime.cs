using System;

namespace AccountCalculator.Domain
{
    public struct UtcDateTime : IComparable<UtcDateTime>, IComparable
    {
        private long Ticks { get; }

        public UtcDateTime(DateTimeOffset dateTimeOffset) : this() => Ticks = dateTimeOffset.Ticks;
        public UtcDateTime(long ticks) : this() => Ticks = ticks;

        public override string ToString() => ToDateTimeOffset().ToString();

        public DateTimeOffset ToDateTimeOffset() => this;

        public DateTime ToDateTime() => this;

        public static implicit operator DateTimeOffset(UtcDateTime utcDateTime) =>
            new (utcDateTime.Ticks, TimeSpan.Zero);

        public static implicit operator UtcDateTime(DateTimeOffset dateTimeOffset) => new (dateTimeOffset.Ticks);

        public static implicit operator DateTime(UtcDateTime utcDateTime) =>
            new (utcDateTime.Ticks, DateTimeKind.Utc);

        public int CompareTo(UtcDateTime other) => Ticks.CompareTo(other.Ticks);

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is UtcDateTime other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(UtcDateTime)}");
        }

        public static bool operator <(UtcDateTime left, UtcDateTime right) => left.Ticks < right.Ticks;

        public static bool operator >(UtcDateTime left, UtcDateTime right) => left.Ticks > right.Ticks;

        public static bool operator <=(UtcDateTime left, UtcDateTime right) => left.Ticks <= right.Ticks;

        public static bool operator >=(UtcDateTime left, UtcDateTime right) => left.Ticks >= right.Ticks;
    }
}
