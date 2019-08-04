using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Minesweeper.Test
{
    public abstract class ExpressionNode : Node
    {

    }

   
    public class CaseItemNode : Node
    {
        public CaseItemNode(IList<ExpressionNode> cases, Node statement)
        {
            
            Cases = cases;
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
        }

        public IList<ExpressionNode> Cases { get; set; }
        public Node Statement { get; set; }
        public override string Display()
        {
            return $"{Aggregate(Cases)} : {Statement}";
        }
    }
    public class CaseStatementNode : Node, IStatementNode
    {
        public CaseStatementNode(ExpressionNode compareExpression, IList<CaseItemNode> caseItemNodes, Node elseStatement)
        {
            CompareExpression = compareExpression;
            CaseItemNodes = caseItemNodes;
            ElseStatement = elseStatement;
        }

        public ExpressionNode CompareExpression { get; set; }
        public IList<CaseItemNode> CaseItemNodes { get; set; }

        public Node ElseStatement { get; set; }
        public override string Display()
        {
            return $"Case({CompareExpression} of {Aggregate(CaseItemNodes)} else {ElseStatement}";
        }
    }
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

        public ExpressionNode Expression()
        {
            
            var result = AddSub();
            if (Name == Pascal.In)
            {
                var current = Current;
                Eat(Pascal.In);
                result = new InOperator(result, ListNode(), current);
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

        public ListNode ListNode()
        {
            var list = Current;
            Eat(Pascal.LeftBracket);
            var from = new StringNode(Current);
            Eat(Pascal.StringConst);
            if (Name == Pascal.Dot)
            {
                Eat(Pascal.Dot);
                Eat(Pascal.Dot);
                var to = new StringNode(Current);
                Eat(Pascal.StringConst);
                Eat(Pascal.RightBracket);
                return new ListRangeExpressionNode(from, to, list);
            }
            else
            {
                var items = new List<StringNode>(){from};
                while (Name == Pascal.Comma)
                {
                    Eat(Pascal.Comma);
                    items.Add(new StringNode(Current));
                    Eat(Pascal.StringConst);
                }
                Eat(Pascal.RightBracket);
                return new ListItemsExpressionNode(items, list);
            }
            
        }

        private ExpressionNode AddSub()
        {
            ExpressionNode Action()
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


       
        protected virtual ExpressionNode ParseNumber()
        {
            var current = _tokens.Current;

            if (Name == Pascal.Pointer)
            {
                Eat(Pascal.Pointer);
                return new PointerNode( current);
            }

            if (Name == Pascal.StringConst)
            {
                var s = new StringNode(Current);
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

        protected virtual ExpressionNode ParaUnaryOperators()
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

       

        protected virtual ExpressionNode Parenthese()
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



        
        ExpressionNode MultiDiv()
        {
            ExpressionNode Action()
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