using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NerdyMishka
{
    public struct DateRange : IEquatable<DateRange>
    {
        public DateRange(DateTimeOffset end)
        {
            this.Start = DateTimeOffset.UtcNow;
            this.End = end;
        }

        public DateRange(DateTimeOffset start, DateTimeOffset end)
        {
            this.Start = start;
            this.End = end;
        }

        public DateRange(TimeSpan span)
        {
            var now = DateTimeOffset.UtcNow;
            this.Start = now;
            this.End = now.Add(span);
        }

        public static DateRange Zero { get; } = new DateRange(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

        public static DateRange Max { get; } = new DateRange(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        public static implicit operator TimeSpan(DateRange range)
        {
            return range.End - range.Start;
        }

        public static implicit operator DateRange(TimeSpan span)
        {
            return new DateRange(span);
        }

        public static implicit operator DateRange(long ticks)
        {
            return new DateRange(new TimeSpan(ticks));
        }

        public static implicit operator DateRange(DateTimeOffset notAfter)
        {
            return new DateRange(DateTimeOffset.UtcNow, notAfter);
        }

        public static implicit operator DateRange(DateTime notAfter)
        {
            return new DateRange(DateTimeOffset.UtcNow, notAfter);
        }

        public static bool operator ==(DateRange left, DateRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DateRange left, DateRange right)
        {
            return !left.Equals(right);
        }

        public static DateRange AsOneYear()
        {
            return new DateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1));
        }

        public static DateRange AsFiveYears()
        {
            return new DateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5));
        }

        public static DateRange AsTenYears()
        {
            return new DateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10));
        }

        public bool Equals(DateRange other)
        {
            return this.Start == other.Start && this.End == other.End;
        }

        public override bool Equals(object obj)
        {
            if (obj is DateRange range)
                return this.Equals(range);

            return false;
        }

        public override int GetHashCode()
        {
            return this.Start.GetHashCode() ^ this.End.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Start.ToString()}-{this.End.ToString()}";
        }

        public string ToString(string format)
        {
            return $"{this.Start.ToString(format)}-{this.End.ToString(format)}";
        }
    }
}
