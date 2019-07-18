using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class Node
    {
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
            if (_tokens.Current.Token.Name == SimpleTree.LParinth)
            {
                Eat(SimpleTree.LParinth);
                var result = Expression();
                Eat(SimpleTree.RParinth);
                return result;
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
                    Eat(SimpleTree.Multi);
                    result = Para();
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Div)
                {
                    Eat(SimpleTree.Div);
                    result = Para();
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
                    Eat(SimpleTree.Add);
                    result = MultiDiv();
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Sub)
                {
                    Eat(SimpleTree.Sub);
                    result = MultiDiv();
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