using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class ProcedureCallNode : CallNode
    {
        public string ProcedureName => Name;

        public ProcedureCallNode(string name, IList<Node> parameters, TokenItem token) : base(name, parameters, token, "Procedure")
        {
        }
    }
}