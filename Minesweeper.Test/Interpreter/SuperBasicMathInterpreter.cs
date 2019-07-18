using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class NumberLeaf
    {
        public NumberLeaf(TokenItem token)
        {
            Value = double.Parse(token.Value);
            TokenItem = token;
        }
        public double Value { get; set; }
        public TokenItem TokenItem { get; set; }
    }
    public class BinaryOperator
    {
        public BinaryOperator Left { get; set; }
        public BinaryOperator Right { get; set; }
        public TokenItem TokenItem { get; set; }
    }
    public class AbstractSyntaxTree
    {

    }
    public class SuperBasicMathInterpreter : IDisposable
    {
        private readonly string _data;
        private readonly IEnumerator<TokenItem> _tokens;
        public SuperBasicMathInterpreter(string data)
        {
            this._data = data;
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

        double ParseNumber()
        {
            var value = double.Parse(_tokens.Current.Value);
            _tokens.MoveNext();
            return value;
        }

        double Para()
        {
            var result = 0.0;
            if (_tokens.Current.Token.Name == SimpleTree.LParinth)
            {
                Eat(SimpleTree.LParinth);
                result = Expression();
                Eat(SimpleTree.RParinth);
                return result;
            }
            return ParseNumber();
        }
        double MultiDiv()
        {
            var result = Para();

            while (_tokens.Current != null && _tokens.Current.Token.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Token.Name == SimpleTree.Multi)
                {
                    Eat(SimpleTree.Multi);
                    result *= Para();
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Div)
                {
                    Eat(SimpleTree.Div);
                    result /= Para();
                }
                else
                {
                    break;
                }
            }


            return result;
        }

        public double Evaluate()
        {
            Eat(null);
            var result = Expression();
            return result;
        }

        private double Expression()
        {
            var result = MultiDiv();
            while (_tokens.Current != null && _tokens.Current.Token.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Token.Name == SimpleTree.Add)
                {
                    Eat(SimpleTree.Add);
                    result += MultiDiv();
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Sub)
                {
                    Eat(SimpleTree.Sub);
                    result -= MultiDiv();
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