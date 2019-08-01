namespace Minesweeper.Test
{
    public class StringNode : Node
    {
        public string CurrentValue { get; }

        public StringNode(string currentValue)
        {
            CurrentValue = currentValue;
        }

        public override string Display()
        {
            return $"String({CurrentValue})";
        }
    }
}