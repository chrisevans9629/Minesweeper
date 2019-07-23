namespace Minesweeper.Test
{
    public class BinaryOperator : Node
    {
        public BinaryOperator(Node left, Node right, TokenItem @operator)
        {
            Left = left;
            Right = right;
            TokenItem = @operator;
        }

        public Node Left { get; set; }
        public Node Right { get; set; }
        public override string Display()
        {
            return $"{Left.Display()} {TokenItem.Value} {Right.Display()}";
        }
    }
}