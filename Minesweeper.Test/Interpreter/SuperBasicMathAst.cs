using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class Node
    {

        public string Name => TokenItem.Token.Name;
        public TokenItem TokenItem { get; set; }
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

    public class SuperBasicMathAst : IDisposable
    {
        private readonly IEnumerator<TokenItem> _tokens;
        public SuperBasicMathAst(string data)
        {
            var lex = new Lexer();
            lex.Ignore(" ");
            lex.Add("LPA", @"\(");
            lex.Add("RPA", @"\)");
            lex.Add("NUM", @"\d+");
            lex.Add("ADD", @"\+");
            lex.Add("SUB", @"-");
            lex.Add("MUL", @"\*");
            lex.Add("DIV", @"/");
            _tokens = lex.Tokenize(data).GetEnumerator();
            //_tokens = GetTokens();
        }

        void Eat(string name)
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
                return new UnaryOperator(ParseNumber(), current);
            }
            if (current.Token.Name == SimpleTree.Sub)
            {
                Eat(SimpleTree.Sub);
                return new UnaryOperator(ParseNumber(), current);
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

        public Node Evaluate()
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

        public void Dispose()
        {
            _tokens?.Dispose();
        }
    }
}