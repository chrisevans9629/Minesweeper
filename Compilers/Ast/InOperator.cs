namespace Minesweeper.Test
{
    public class InOperator : ExpressionNode
    {
        public Node CompareNode { get; }
        public Node ListExpression { get; }
        public TokenItem TokenItem { get; }

        public InOperator(Node compareNode, Node listExpression, TokenItem tokenItem)
        {
            CompareNode = compareNode;
            ListExpression = listExpression;
            TokenItem = tokenItem;
        }

        public override string Display()
        {
            return $"In({CompareNode},{ListExpression})";
        }
    }
}