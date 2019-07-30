using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class ForLoopNode : Node
    {
        //for < variable-name > := < initial_value > to [down to] < final_value > do S;

        public AssignNode AssignFromNode { get; set; }
        public Node ToNode { get; set; }

        public IList<Node> DoStatements { get; set; }

        public override string Display()
        {
            return $"For {AssignFromNode} To {ToNode} Do {DoStatements}";
        }
    }
}