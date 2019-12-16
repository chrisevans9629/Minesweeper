namespace Minesweeper
{
    public interface IRandomizer
    {
        int IntInRange(IntegerRange range);
        int IntInRange(int from, int to);
        int IntLessThan(int upper);
        double GetDoubleFromZeroToOne();
    }
}