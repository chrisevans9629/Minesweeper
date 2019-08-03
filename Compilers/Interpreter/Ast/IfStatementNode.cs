using System.Collections.Generic;

namespace Minesweeper.Test
{
    public interface INode
    {
         string Display();

    }
    public interface IStatementNode : INode
    {

    }
    public class IfStatementNode : Node, IStatementNode
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