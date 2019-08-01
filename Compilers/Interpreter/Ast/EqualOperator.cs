namespace Minesweeper.Test
{
    public abstract class EqualExpression : Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public TokenItem TokenItem { get; set; }
        public string Name { get; }

        public EqualExpression(Node left, Node right, TokenItem tokenItem, string name)
        {
            Left = left;
            Right = right;
            TokenItem = tokenItem;
            Name = name;
        }
        public override string Display()
        {
            return $"{Name}({Left.Display()}, {Right.Display()})";
        }
    }
    public class NotEqualOperator : EqualExpression
    {

        public NotEqualOperator(Node left, Node right, TokenItem tokenItem) : base(left, right, tokenItem, "NotEqual")
        {
        }
       
    }
    public class EqualOperator : EqualExpression
    {
       

        public EqualOperator(Node left, Node right, TokenItem tokenItem) :base(left, right, tokenItem, "Equal")
        {
            
        }

      
    }
}