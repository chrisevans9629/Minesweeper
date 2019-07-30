namespace Minesweeper.Test
{

    public class IntegerNode : Node
    {
        public TokenItem Token { get; }
        public int Value { get; set; }
        public IntegerNode(TokenItem token)
        {
            Token = token;
            Value = int.Parse(token.Value);
        }
        public override string Display()
        {
            return $"Integer({Value})";
        }
    }

    public class RealNode : Node
    {
        public RealNode(TokenItem token)
        {
            Value = double.Parse(token.Value);
            TokenItem = token;
        }

        public TokenItem TokenItem { get; set; }
        public double Value { get; set; }
        public override string Display()
        {
            return $"Real({Value})";
        }
    }
}