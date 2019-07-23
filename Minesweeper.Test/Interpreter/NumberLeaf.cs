namespace Minesweeper.Test
{
    public class NumberLeaf : Node
    {
        public NumberLeaf(TokenItem token)
        {
            Value = double.Parse(token.Value);
            TokenItem = token;
        }
        public double Value { get; set; }
        public override string Display()
        {
            return $"Number({Value})";
        }
    }
}