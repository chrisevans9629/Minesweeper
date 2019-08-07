using System.Collections.Generic;

namespace Minesweeper.Test
{

    public abstract class ListNode : Node
    {
        public TokenItem TokenItem { get; set; }

    }
    public class ListItemsExpressionNode : ListNode
    {
        public ListItemsExpressionNode(IList<StringNode> items, TokenItem tokenItem)
        {
            Items = items;
            TokenItem = tokenItem;
        }

        public IList<StringNode> Items { get; set; }
        public override string Display()
        {
            return $"List({Aggregate(Items)})";
        }
    }
    public class ListRangeExpressionNode : ListNode
    {
        public StringNode FromNode { get; }
        public StringNode ToNode { get; }

        public ListRangeExpressionNode(StringNode fromNode, StringNode toNode, TokenItem tokenItem)
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