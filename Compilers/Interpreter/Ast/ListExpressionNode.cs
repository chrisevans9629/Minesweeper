namespace Minesweeper.Test
{
    public class ListExpressionNode : Node
    {
        public StringNode FromNode { get; }
        public StringNode ToNode { get; }
        public TokenItem TokenItem { get; }

        public ListExpressionNode(StringNode fromNode, StringNode toNode, TokenItem tokenItem)
        {
            FromNode = fromNode;
            ToNode = toNode;
            TokenItem = tokenItem;
        }
        public override string Display()
        {
            return $"List({FromNode}..{ToNode})";
        }
    }
}