using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;

namespace Minesweeper.Test
{
    public abstract class Node
    {
        public static string Aggregate(IEnumerable<Node> nodes)
        {
            return (nodes.Any() ? nodes.Select(p => p.Display()).Aggregate((f, s) => $"{f}, {s}") : "");
        }

        public string Name => TokenItem.Token.Name;
        public TokenItem TokenItem { get; set; }

        public abstract string Display();
    }

    public class NoOp : Node
    {
        public override string Display()
        {
            return "NoOp";
        }
    }
    public class Variable : Node
    {
        public string VariableName { get; set; }
        public Variable(TokenItem token)
        {
            TokenItem = token;
            VariableName = token.Value;
        }

        public override string Display()
        {
            return $"Variable({VariableName})";
        }
    }
    public class Assign : Node
    {
        public Variable Left { get; set; }
        public Node Right { get; set; }
        public Assign(Variable left, TokenItem item, Node right)
        {
            Left = left;
            Right = right;
            TokenItem = item;
        }

        public override string Display()
        {
            return $"Assign({Left.Display()}, {Right.Display()})";
        }
    }
    public class Compound : Node
    {
        public Compound()
        {
            Nodes = new List<Node>();
        }
        public IList<Node> Nodes { get; set; }
        public override string Display()
        {
            return $"Compound({Aggregate(Nodes)}";
        }
    }
    public class NumberLeaf : Node
    {
        public NumberLeaf(TokenItem token)
        {
            Value = double.Parse(token.Value);
            TokenItem = token;
        }
        public double Value { get; set; }
        public override string Display()
        {
            return $"Number({Value})";
        }
    }

    public class UnaryOperator : Node
    {
        public UnaryOperator(Node value, TokenItem op)
        {
            Value = value;
            TokenItem = op;
        }

        public Node Value { get; set; }
        public override string Display()
        {
            return $"Unary({TokenItem.Value} {Value.Display()})";
        }
    }

    public class BinaryOperator : Node
    {
        public BinaryOperator(Node left, Node right, TokenItem @operator)
        {
            Left = left;
            Right = right;
            TokenItem = @operator;
        }

        public Node Left { get; set; }
        public Node Right { get; set; }
        public override string Display()
        {
            return $"{Left.Display()} {TokenItem.Value} {Right.Display()}";
        }
    }

    public class Pascal
    {
        public const string Real = "REAL";
        public const string Colon = "SEMICOLON";
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string Dot = "DOT";
        public const string Id = "ID";
        public const string Assign = "ASSIGN";
        public const string Semi = "SEMI";
        public const string Comma = "COMMA";
        public const string Var = "VAR";
        public const string Int = "INTEGER";
        public const string RealConst = "Real_Const";

        public static void AddPascalTokens(RegexLexer lex)
        {
            lex.Add(Begin, Begin);
            lex.Add(End, End);
            lex.Add(Dot, ".");
            lex.Add(Assign, ":=");
            lex.Add(Semi, ":");
            lex.Add(Id, "[a-zA-Z]+");

        }
    }


    public class SuperBasicMathAst : AbstractSyntaxTreeBase
    {
        public SuperBasicMathAst(IList<TokenItem> data)
        {
           
            _tokens = data.GetEnumerator();
            //_tokens = GetTokens();
        }

        public static void AddMathTokens(RegexLexer lex)
        {
            lex.Ignore(" ");
            lex.Add("LPA", @"\(");
            lex.Add("RPA", @"\)");
            lex.Add("NUM", @"\d+");
            lex.Add("ADD", @"\+");
            lex.Add("SUB", @"-");
            lex.Add("MUL", @"\*");
            lex.Add(SimpleTree.FloatDiv, @"/");
        }

        

        public override Node Evaluate()
        {
            Eat(null);
            var result = Expression();
            return result;
        }

       


    }
}