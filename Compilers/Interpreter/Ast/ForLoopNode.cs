using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class ForLoopNode : Node
    {
        public ForLoopNode(AssignNode assignFromNode, Node toNode, IList<Node> doStatements)
        {
            AssignFromNode = assignFromNode;
            ToNode = toNode;
            DoStatements = doStatements;
        }
        //for < variable-name > := < initial_value > to [down to] < final_value > do S;

        public AssignNode AssignFromNode { get;  }
        public Node ToNode { get;  }

        public IList<Node> DoStatements { get;  }

        public override string Display()
        {
            return $"For({AssignFromNode} To {ToNode} Do({Aggregate(DoStatements)}))";
        }
    }
}