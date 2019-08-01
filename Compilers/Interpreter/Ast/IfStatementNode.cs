using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class IfStatementNode : Node
    {
        public Node IfCheck { get; }
        public IList<Node> IfTrue { get; }
        public IList<Node> IfFalse { get; }

        public IfStatementNode(Node ifCheck, IList<Node> IfTrue, IList<Node> IfFalse)
        {
            IfCheck = ifCheck;
            this.IfTrue = IfTrue;
            this.IfFalse = IfFalse;
        }

        public override string Display()
        {
            return $"If ({IfCheck}) then {Aggregate(IfTrue)} else {Aggregate(IfFalse)}";
        }
    }
}