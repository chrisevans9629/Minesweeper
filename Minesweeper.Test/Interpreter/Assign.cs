namespace Minesweeper.Test
{
    public class Assign : Node
    {
        public Variable Left { get; set; }
        public Node Right { get; set; }
        public TokenItem TokenItem { get; set; }
        public Assign(Variable left, TokenItem item, Node right)
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