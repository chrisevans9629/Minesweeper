using System.Collections.Generic;

namespace Minesweeper.Test
{

    public class FunctionCallNode : Node
    {
        public string FunctionName { get; }
        public IList<Node> Parameters { get; }
        public TokenItem Token { get; }

        public FunctionCallNode(string functionName, IList<Node> parameters, TokenItem token)
        {
            FunctionName = functionName;
            Parameters = parameters;
            Token = token;
        }

        public override string Display()
        {
            return $"Function {FunctionName}({Node.Aggregate(Parameters)})";
        }
    }
    public class FunctionDeclarationNode : Node
    {
        public string FunctionName { get; }
        public IList<ProcedureParameter> Parameters { get; }
        public BlockNode BlockNode { get; }
        public TokenItem Token { get; }

        public FunctionDeclarationNode(string functionName, IList<ProcedureParameter> parameters, BlockNode blockNode, TokenItem token)
        {
            FunctionName = functionName;
            Parameters = parameters;
            BlockNode = blockNode;
            Token = token;
        }
        public override string Display()
        {
            return $"Function({FunctionName}, {Node.Aggregate(Parameters)})";
        }
    }
}