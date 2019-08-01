namespace Minesweeper.Test
{
    public class PointerNode : Node
    {
        public char Value { get; }

        public PointerNode(char value)
        {
            Value = value;
        }
        public override string Display()
        {
            return $"Pointer({Value})";
        }
    }
}