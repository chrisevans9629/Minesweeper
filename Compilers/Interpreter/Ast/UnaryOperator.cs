namespace Minesweeper.Test
{
    public class UnaryOperator : ExpressionNode
    {
        public TokenItem TokenItem { get; set; }
        public UnaryOperator(Node value, TokenItem op)
        {
            Value = value;
            TokenItem = op;
        }

        public string Name => TokenItem.Token.Name;
        public Node Value { get; set; }
        public override string Display()
        {
            return $"Unary({TokenItem.Value} {Value.Display()})";
        }
    }
}