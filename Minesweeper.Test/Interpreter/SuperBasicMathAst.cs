using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;

namespace Minesweeper.Test
{
    public class Node
    {
        public string Name => TokenItem.Token.Name;
        public TokenItem TokenItem { get; set; }
    }

    public class NoOp : Node
    {

    }
    public class Variable : Node
    {
        public string VariableName { get; set; }
        public Variable(TokenItem token)
        {
            TokenItem = token;
            VariableName = token.Value;
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
    }
    public class Compound : Node
    {
        public Compound()
        {
            Nodes = new List<Node>();
        }
        public IList<Node> Nodes { get; set; }
    }
    public class NumberLeaf : Node
    {
        public NumberLeaf(TokenItem token)
        {
            Value = double.Parse(token.Value);
            TokenItem = token;
        }
        public double Value { get; set; }
    }

    public class UnaryOperator : Node
    {
        public UnaryOperator(Node value, TokenItem op)
        {
            Value = value;
            TokenItem = op;
        }

        public Node Value { get; set; }
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
    }

    public class Pascal
    {
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string Dot = "DOT";
        public const string Id = "ID";
        public const string Assign = "ASSIGN";
        public const string Semi = "SEMI";
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



    public class PascalAst : AbstractSyntaxTreeBase
    {
        public PascalAst(string str)
        {
            var lex = new PascalLexer(str);
            this._tokens = lex.Tokenize().GetEnumerator();
        }

        Node CompoundStatement()
        {
            Eat(Pascal.Begin);
            var nodes = StatementList();
            Eat(Pascal.End);
            var root = new Compound();
            root.Nodes = nodes.ToList();
            return root;
        }

        IList<Node> StatementList()
        {
            var node = Statement();

            var results = new List<Node> {node};

            while (this._tokens.Current.Token.Name == Pascal.Semi)
            {
                Eat(Pascal.Semi);
                results.Add(Statement());
            }

            if (_tokens.Current.Token.Name == Pascal.Id)
            {
                throw new NotImplementedException();
            }

            return results;
        }

        Node Statement()
        {
            Node node = null;
            if (_tokens.Current.Token.Name == Pascal.Begin)
                node = CompoundStatement();
            else if (_tokens.Current.Token.Name == Pascal.Id)
                node = AssignmentStatement();
            else node = Empty();
            return node;
        }

        Node AssignmentStatement()
        {
            var left = new Variable(_tokens.Current);
            var token = _tokens.Current;
            Eat(Pascal.Assign);
            var right = Expression();
            var node = new Assign(left, token, right);
            return node;
        }

        Node Variable()
        {
            var node = new Variable(_tokens.Current);
            Eat(Pascal.Id);
            return node;
        }
        Node Expression()
        {
            throw new NotImplementedException();
        }
        Node Empty()
        {
            return new NoOp();
        }
        Node Program()
        {
            var node = CompoundStatement();
            Eat(Pascal.Dot);
            return node;
        }
        public override Node Evaluate()
        {
            return Program();
        }
    }
    public abstract class AbstractSyntaxTreeBase : IDisposable
    {
        protected IEnumerator<TokenItem> _tokens;
        public void Dispose()
        {
            _tokens?.Dispose();
        }
        public void Eat(string name)
        {
            if (_tokens.Current?.Token.Name == name)
            {
                _tokens.MoveNext();
            }
            else
            {
                throw new Exception($"expected {name} but was {_tokens.Current?.Token.Name}");
            }
        }

        public abstract Node Evaluate();
    }
    public class SuperBasicMathAst : AbstractSyntaxTreeBase
    {
        public SuperBasicMathAst(string data)
        {
            var lex = new RegexLexer();
            AddMathTokens(lex);
            _tokens = lex.Tokenize(data).GetEnumerator();
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
            lex.Add("DIV", @"/");
        }



        NumberLeaf ParseNumber()
        {
            var value = new NumberLeaf(_tokens.Current);
            _tokens.MoveNext();
            return value;
        }

        Node Para()
        {
            var current = _tokens.Current;
            if (_tokens.Current.Token.Name == SimpleTree.LParinth)
            {
                Eat(SimpleTree.LParinth);
                var result = Expression();
                Eat(SimpleTree.RParinth);
                return result;
            }

            if (current.Token.Name == SimpleTree.Add)
            {
                Eat(SimpleTree.Add);
                return new UnaryOperator(Para(), current);
            }
            if (current.Token.Name == SimpleTree.Sub)
            {
                Eat(SimpleTree.Sub);
                return new UnaryOperator(Para(), current);
            }
            return ParseNumber();
        }
        Node MultiDiv()
        {
            var result = Para();

            while (_tokens.Current != null && _tokens.Current.Token.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Token.Name == SimpleTree.Multi)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Multi);
                    result = new BinaryOperator(result, Para(), token);
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Div)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Div);
                    result = new BinaryOperator(result, Para(), token);
                }
                else
                {
                    break;
                }
            }


            return result;
        }

        public override Node Evaluate()
        {
            Eat(null);
            var result = Expression();
            return result;
        }

        private Node Expression()
        {
            var result = MultiDiv();
            while (_tokens.Current != null && _tokens.Current.Token.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Token.Name == SimpleTree.Add)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Add);
                    result = new BinaryOperator(result, MultiDiv(), token);
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Sub)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Sub);
                    result = new BinaryOperator(result, MultiDiv(), token);
                }
                else
                {
                    break;
                }
            }

            return result;
        }


    }
}