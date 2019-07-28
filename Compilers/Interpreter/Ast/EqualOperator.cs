namespace Minesweeper.Test
{
    public class EqualOperator : Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public TokenItem TokenItem { get; set; }

        public EqualOperator(Node left, Node right, TokenItem tokenItem)
        {
            Left = left;
            Right = right;
            TokenItem = tokenItem;
        }

        public override string Display()
        {
            return $"Equal({Left.Display()}, {Right.Display()})";
        }
    }
}