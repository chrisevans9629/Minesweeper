using System;

namespace Minesweeper
{
    public class Randomizer : IRandomizer
    {
        private Random random;
        public Randomizer(Random random)
        {
            this.random = random;
        }
        public int IntInRange(IntegerRange range)
        {
            return IntInRange(range.From, range.To);
        }

        public int IntInRange(int @from, int to)
        {
            return random.Next(from,to);

        }

        public int IntLessThan(int upper)
        {
            return random.Next(upper);
        }

        public double GetDoubleFromZeroToOne()
        {
            return random.NextDouble();
        }

    }
}