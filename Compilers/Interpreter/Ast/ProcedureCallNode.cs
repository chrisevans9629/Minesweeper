using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class ProcedureCallNode : Node
    {
        public string ProcedureName { get; }
        public IList<Node> Parameters { get; }
        public TokenItem Token { get; }

        public ProcedureCallNode(string procedureName, IList<Node> parameters, TokenItem token)
        {
            ProcedureName = procedureName;
            Parameters = parameters;
            Token = token;
        }

        public override string Display()
        {
            return $"Procedure {ProcedureName}({Node.Aggregate(Parameters)})";
        }
    }
}