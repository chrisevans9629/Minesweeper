using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{

    public class Logger : ILogger
    {
        public virtual void Log(object obj)
        {
            Console.WriteLine(obj);
        }
    }

    public interface ILogger
    {
        void Log(object obj);
    }
    public abstract class AbstractSyntaxTreeBase
    {
        protected readonly ILogger Logger;

        public AbstractSyntaxTreeBase(ILogger logger)
        {
            Logger = logger ?? new Logger();
        }
        protected Node Expression()
        {
            var result = MultiDiv();
            while (_tokens.Current != null && _tokens.Current.Token.Name != Pascal.IntegerConst)
            {
                if (_tokens.Current.Token.Name == Pascal.Add)
                {
                    var token = _tokens.Current;
                    Eat(Pascal.Add);
                    result = new BinaryOperator(result, MultiDiv(), token);
                }

                else if (_tokens.Current.Token.Name == Pascal.Sub)
                {
                    var token = _tokens.Current;
                    Eat(Pascal.Sub);
                    result = new BinaryOperator(result, MultiDiv(), token);
                }
                else
                {
                    break;
                }
            }

            return result;
        }
        protected virtual NumberLeaf ParseNumber()
        {
            var value = new NumberLeaf(_tokens.Current);
            _tokens.Advance();
            return value;
        }

        protected virtual Node ParaAddSub()
        {
            var current = _tokens.Current;
            var par = Parenthese();
            if (par != null) return par;

            if (current.Token.Name == Pascal.Add)
            {
                Eat(Pascal.Add);
                return new UnaryOperator(ParaAddSub(), current);
            }
            if (current.Token.Name == Pascal.Sub)
            {
                Eat(Pascal.Sub);
                return new UnaryOperator(ParaAddSub(), current);
            }

            
            return ParseNumber();
        }

       

        protected virtual Node Parenthese()
        {
            if (_tokens.Current.Token.Name == Pascal.LParinth)
            {
                Eat(Pascal.LParinth);
                var result = Expression();
                Eat(Pascal.RParinth);
                var para = result;
                return para;
            }

            return null;
        }

        Node MultiDiv()
        {
            var result = ParaAddSub();

            while (_tokens.Current != null && _tokens.Current.Token.Name != Pascal.IntegerConst)
            {
                if (_tokens.Current.Token.Name == Pascal.Multi)
                {
                    var token = _tokens.Current;
                    Eat(Pascal.Multi);
                    result = new BinaryOperator(result, ParaAddSub(), token);
                }

                else if (_tokens.Current.Token.Name == Pascal.FloatDiv)
                {
                    var token = _tokens.Current;
                    Eat(Pascal.FloatDiv);
                    result = new BinaryOperator(result, ParaAddSub(), token);
                }
                else if (_tokens.Current.Token.Name == Pascal.IntDiv)
                {
                    var token = _tokens.Current;
                    Eat(Pascal.IntDiv);
                    result = new BinaryOperator(result, ParaAddSub(), token);
                }
                else
                {
                    break;
                }
            }


            return result;
        }
        protected Iterator<TokenItem> _tokens;
        
        public void Eat(string name)
        {
            if (_tokens.Current?.Token.Name == name)
            {
                Logger.Log($"Ate: {name}");
                _tokens.Advance();
            }
            else
            {
                var current = _tokens.Current;
                throw new ParserException(ErrorCode.UnexpectedToken, _tokens.Current, $"Expected an '{name}' token but was '{_tokens.Current?.Token.Name}' at index {current.Index} column {current.Column} line {current.Line}");
            }
        }

        public abstract Node Evaluate();
    }

    public class BoolNode : Node
    {
        public TokenItem TokensCurrent { get; }
        public bool Value { get; set; }
        public TokenItem TokenItem { get; set; }
        public BoolNode(TokenItem tokensCurrent)
        {
            TokensCurrent = tokensCurrent;
            Value = bool.Parse(tokensCurrent.Value.ToLower());
        }


        public override string Display()
        {
            return Value.ToString();
        }
    }

    public class GrammerException : Exception
    {
        public GrammerException(string message) : base(message)
        {
            
        }
    }
}