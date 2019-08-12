using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class CallNode : ExpressionNode
    {
        public string Type { get; }
        public string Name { get; }
        public IList<Node> Parameters { get; }
        public TokenItem Token { get; }

        public CallNode(string name, IList<Node> parameters, TokenItem token, string type)
        {
            Name = name;
            Parameters = parameters;
            Token = token;
            Type = type;
        }

        public override string Display()
        {
            return $"{Type} {Name}({Node.Aggregate(Parameters)})";
        }
    }
    public class FunctionCallNode : CallNode
    {
        public string FunctionName => Name;
        public FunctionCallNode(string name, IList<Node> parameters, TokenItem token) : base(name, parameters, token, "Function")
        {
        }
    }
    public abstract class DeclarationNode : Node
    {
        protected string MethodType { get; set; }
        public string Name { get; protected set; }
        public IList<ParameterNode> Parameters { get; protected set; }
        public BlockNode Block { get; protected set; }
        public TokenItem Token { get; protected set; }
        public override string Display()
        {
            return $"{MethodType}({Name}, {Node.Aggregate(Parameters)}, {Block})";
        }
    }

    public class FunctionDeclarationNode : DeclarationNode
    {
        public string FunctionName => Name;
       
        public TypeNode ReturnType { get; }

        public FunctionDeclarationNode(string functionName, IList<ParameterNode> parameters, BlockNode block, TokenItem token, TypeNode returnType)
        {
            Name = functionName;
            Parameters = parameters;
            Block = block;
            Token = token;
            ReturnType = returnType;
            MethodType = "Function";
        }
       
    }
}