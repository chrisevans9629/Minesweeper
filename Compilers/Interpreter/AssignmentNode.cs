namespace Minesweeper.Test
{
    public class AssignmentNode : Node
    {
        public VariableOrFunctionCall Left { get; set; }
        public Node Right { get; set; }
        public TokenItem TokenItem { get; set; }
        public AssignmentNode(VariableOrFunctionCall left, TokenItem item, Node right)
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