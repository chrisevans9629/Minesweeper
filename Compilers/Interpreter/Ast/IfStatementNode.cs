namespace Minesweeper.Test
{
    public class IfStatementNode : Node
    {
        public EqualOperator IfCheck { get; }
        public Node IfTrue { get; }
        public Node IfFalse { get; }

        public IfStatementNode(EqualOperator ifCheck, Node IfTrue, Node IfFalse)
        {
            IfCheck = ifCheck;
            this.IfTrue = IfTrue;
            this.IfFalse = IfFalse;
        }

        public override string Display()
        {
            return $"If ({IfCheck}) then {IfTrue} else {IfFalse}";
        }
    }
}