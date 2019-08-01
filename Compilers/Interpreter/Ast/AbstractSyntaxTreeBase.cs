﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        protected string Name => Current?.Token?.Name;
        protected TokenItem Current => _tokens.Current;

        public Node Expression()
        {
            
            var result = AddSub();
            if (Name == Pascal.In)
            {
                var current = Current;
                Eat(Pascal.In);
                result = new InOperator(result, ListExpression(), current);
            }
            if (_tokens.Current != null && Name == Pascal.Equal)
            {
                var current = Current;
                Eat(Pascal.Equal);
                result = new EqualOperator(result, AddSub(), current);
            }
            if (_tokens.Current != null && Name == Pascal.NotEqual)
            {
                var current = Current;
                Eat(Pascal.NotEqual);
                result = new NotEqualOperator(result, AddSub(), current);
            }
            return result;
        }

        private ListExpressionNode ListExpression()
        {
            var list = Current;
            Eat(Pascal.LeftBracket);
            var from = new StringNode(Current.Value);
            Eat(Pascal.StringConst);
            Eat(Pascal.Dot);
            Eat(Pascal.Dot);
            var to = new StringNode(Current.Value);
            Eat(Pascal.StringConst);
            Eat(Pascal.RightBracket);
            return new ListExpressionNode(from, to, list);
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


       
        protected virtual Node ParseNumber()
        {
            var current = _tokens.Current;

            if (Name == Pascal.Pointer)
            {
                Eat(Pascal.Pointer);
                return new PointerNode(current.Value[0]);
            }

            if (Name == Pascal.StringConst)
            {
                var s = new StringNode(Current.Value);
                Eat(Pascal.StringConst);
                return s;
            }
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
}