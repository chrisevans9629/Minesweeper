namespace Minesweeper.Test
{
    public class InOperator : ExpressionNode
    {
        public Node CompareNode { get; }
        public ListNode ListExpression { get; }
        public TokenItem TokenItem { get; }

        public InOperator(Node compareNode, ListNode listExpression, TokenItem tokenItem)
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