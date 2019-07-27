namespace Minesweeper.Test
{
    public class AssignNode : Node
    {
        public Variable Left { get; set; }
        public Node Right { get; set; }
        public TokenItem TokenItem { get; set; }
        public AssignNode(Variable left, TokenItem item, Node right)
        {
            Left = left;
            Right = right;
            TokenItem = item;
        }

        public override string Display()
        {
            return $"Assign({Left.Display()}, {Right.Display()})";
        }
    }
}