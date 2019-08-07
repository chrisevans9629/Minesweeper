namespace Minesweeper.Test
{
    public class TypeNode : Node
    {
        public string TypeValue { get; set; }
        public TokenItem TokenItem { get; set; }

        public TypeNode(TokenItem token)
        {
            TokenItem = token;
            TypeValue = token.Value;
        }
        public override string Display()
        {
            return $"Type({TypeValue})";
        }
    }
}