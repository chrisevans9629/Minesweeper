using System;

namespace Minesweeper
{
    public class Range<T> where T : IComparable
    {
        public T From { get; }
        public T To { get; }

        public Range(T from, T to)
        {
            if (GreaterThan(from, to))
            {
                From = to;
                To = from;
            }
            else
            {
                From = from;
                To = to;
            }
            
        }

        bool GreaterThan(T from, T to) => from.CompareTo(to) == 1;
        public bool IsInRange(T value)
        {
            return (From.CompareTo(value) == -1 || From.CompareTo(value) == 0) && (To.CompareTo(value) == 1 || To.CompareTo(value) == 0);
        }
    }
}