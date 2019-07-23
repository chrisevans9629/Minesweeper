namespace Minesweeper.Test
{
    public class UnaryOperator : Node
    {
        public UnaryOperator(Node value, TokenItem op)
        {
            Value = value;
            TokenItem = op;
        }

        public Node Value { get; set; }
        public override string Display()
        {
            return $"Unary({TokenItem.Value} {Value.Display()})";
        }
    }
}