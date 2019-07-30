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
        public Node Expression()
        {
            var result = AddSub();

            if (_tokens.Current != null && _tokens.Current.Token.Name == Pascal.Equal)
            {
                var current = _tokens.Current;
                Eat(Pascal.Equal);
                result = new EqualOperator(result, AddSub(), current);
            }
            return result;
        }

        private Node AddSub()
        {
            Node Action()
            {
                return MultiDiv();
            }

            var result = Action();
            while (_tokens.Current != null && _tokens.Current.Token.Name != Pascal.IntegerConst)
            {
                if (_tokens.Current.Token.Name == Pascal.Add)
                {
                    var token = _tokens.Current;
                    Eat(Pascal.Add);
                    result = new BinaryOperator(result, Action(), token);
                }

                else if (_tokens.Current.Token.Name == Pascal.Sub)
                {
                    var token = _tokens.Current;
                    Eat(Pascal.Sub);
                    result = new BinaryOperator(result, Action(), token);
                }
                else
                {
                    break;
                }
            }

            return result;
        }


        Node Equal()
        {
            var result = Expression();
            var current = _tokens.Current;
            if (_tokens.Current.Token.Name == Pascal.Equal)
            {
                Eat(Pascal.Equal);
                return new EqualOperator(result, Expression(), current);
            }

            return ParseNumber();
        }
        protected virtual Node ParseNumber()
        {
            var current = _tokens.Current;

            
            if (current.Token.Name == Pascal.RealConst)
            {
                Eat(Pascal.RealConst);
                var value = new RealNode(current);
                return value;
            }

            if (current.Token.Name == Pascal.IntegerConst)
            {
                Eat(Pascal.IntegerConst);
                var value = new IntegerNode(current);
                return value;
            }
           
            Error("Number");
            return null;
        }

        protected virtual Node ParaUnaryOperators()
        {
            var current = _tokens.Current;
            var par = Parenthese();
            if (par != null) return par;
            
            if (current.Token.Name == Pascal.Add)
            {
                Eat(Pascal.Add);
                return new UnaryOperator(ParaUnaryOperators(), current);
            }
            if (current.Token.Name == Pascal.Sub)
            {
                Eat(Pascal.Sub);
                return new UnaryOperator(ParaUnaryOperators(), current);
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
            Node Action()
            {
               return ParaUnaryOperators();
            }

            var result = Action();

            while (_tokens.Current != null && _tokens.Current.Token.Name != Pascal.IntegerConst)
            {
                var token = _tokens.Current;
                
                if (_tokens.Current.Token.Name == Pascal.Multi)
                {
                    Eat(Pascal.Multi);
                    result = new BinaryOperator(result, Action(), token);
                }

                else if (_tokens.Current.Token.Name == Pascal.FloatDiv)
                {
                    Eat(Pascal.FloatDiv);
                    result = new BinaryOperator(result, Action(), token);
                }
                else if (_tokens.Current.Token.Name == Pascal.IntDiv)
                {
                    Eat(Pascal.IntDiv);
                    result = new BinaryOperator(result, Action(), token);
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
                Error(name);
            }
        }

        protected void Error(string expectedName)
        {
            var current = _tokens.CurrentOrPrevious();
            
            throw new ParserException(ErrorCode.UnexpectedToken,
                current,
                $"Expected an '{expectedName}' token but was '{current?.Token.Name}'");
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