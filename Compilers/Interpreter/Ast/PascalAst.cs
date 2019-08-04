using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
   

    public class PascalAst : AbstractSyntaxTreeBase
    {

        private IList<TokenItem> items;


        public PascalAst(ILogger logger = null) : base(logger)
        {

        }

        CompoundStatement CompoundStatement()
        {
            Eat(Pascal.Begin);
            var nodes = StatementList();
            Eat(Pascal.End);
            var root = new CompoundStatement();
            root.Nodes = nodes.ToList();
            return root;
        }

        private BlockNode Block()
        {
            var dec = Declarations();
            var comStat = CompoundStatement();
            var node = new BlockNode(dec, comStat);
            return node;
        }

        private IList<Node> Declarations()
        {
            var dec = new List<Node>();

            while (Name == Pascal.Const)
            {
                Eat(Pascal.Const);
                while (Name == Pascal.Id)
                {
                    var vardec = ConstantDeclarations();
                    foreach (var varDeclarationNode in vardec)
                    {
                        dec.Add(varDeclarationNode);
                    }
                }

            }

            while (Name == Pascal.Var)
            {
                Eat(Pascal.Var);
                while (Name == Pascal.Id)
                {
                    var vardec = VariableDeclaration();
                    foreach (var varDeclaration in vardec)
                    {
                        dec.Add(varDeclaration);
                    }
                    Eat(Pascal.Semi);
                }
            }

            while (Name == Pascal.Procedure || Name == Pascal.Function)
            {
                if (Name == Pascal.Procedure)
                {
                    var proc = ProcedureDeclaration();
                    dec.Add(proc);
                }

                if (Name == Pascal.Function)
                {
                    var fun = FunctionDeclaration();
                    dec.Add(fun);
                }
            }

            return dec;
        }
        public FunctionDeclarationNode FunctionDeclaration()
        {
            var parameters = new List<ParameterNode>();
            Eat(Pascal.Function);
            var procedureId = Current.Value;
            var token = Current;
            Eat(Pascal.Id);
            if (Current.Token.Name == Pascal.LParinth)
            {
                Eat(Pascal.LParinth);
                while (Current.Token.Name != Pascal.RParinth)
                {
                    parameters.AddRange(VariableDeclaration().Select(p => new ParameterNode(p)));
                    if (Current.Token.Name == Pascal.Semi)
                    {
                        Eat(Pascal.Semi);
                    }
                }

                Eat(Pascal.RParinth);
            }
            Eat(Pascal.Colon);
            var returnType = TypeSpec();
            Eat(Pascal.Semi);
            var block = Block();
            Eat(Pascal.Semi);
            var proc = new FunctionDeclarationNode(procedureId, parameters, block, token, returnType);
            return proc;
        }

        public ProcedureDeclarationNode ProcedureDeclaration()
        {
            var parameters = new List<ParameterNode>();
            Eat(Pascal.Procedure);
            var procedureId = Current.Value;
            Eat(Pascal.Id);
            if (Current.Token.Name == Pascal.LParinth)
            {
                Eat(Pascal.LParinth);
                while (Current.Token.Name != Pascal.RParinth)
                {
                    parameters.AddRange(VariableDeclaration().Select(p => new ParameterNode(p)));
                    if (Current.Token.Name == Pascal.Semi)
                    {
                        Eat(Pascal.Semi);
                    }
                }

                Eat(Pascal.RParinth);
            }

            Eat(Pascal.Semi);
            var block = Block();
            Eat(Pascal.Semi);
            var proc = new ProcedureDeclarationNode(procedureId, block, parameters);
            return proc;
        }

        IList<ConstantDeclarationNode> ConstantDeclarations()
        {
            var nodes = new List<ConstantDeclarationNode>() { ConstantDeclaration() };
            while (Name == Pascal.Id)
            {
                nodes.Add(ConstantDeclaration());
            }
            return nodes;
        }

        private ConstantDeclarationNode ConstantDeclaration()
        {
            var current = Current;
            Eat(Pascal.Id);
            Eat(Pascal.Equal);
            var value = Expression();
            Eat(Pascal.Semi);
            return new ConstantDeclarationNode(current.Value, value, current);
        }

        private IList<VarDeclarationNode> VariableDeclaration()
        {
            var nodes = new List<VariableOrFunctionCall> { Variable() };
            while (Name == Pascal.Comma)
            {
                Eat(Pascal.Comma);
                nodes.Add(Variable());
            }
            Eat(Pascal.Colon);

            var type = TypeSpec();
            return nodes.Select(p => new VarDeclarationNode(p, type)).ToList();
        }

        private TypeNode TypeSpec()
        {
            if (Current == null)
                Error("Type");
            var result = new TypeNode(Current);

            if (Pascal.BuiltInTypes.Contains(result.TypeValue.ToUpper()))
            {
                Eat(Name);
            }

            return result;
        }

        IList<Node> StatementList()
        {
            var node = Statement();

            var results = new List<Node> { node };

            while (Name == Pascal.Semi)
            {
                Eat(Pascal.Semi);
                results.Add(Statement());
            }

            //if (_tokens.Current?.Token?.Name == Pascal.Id)
            //{
            //    Error();
            // }

            return results;
        }

        protected override ExpressionNode ParaUnaryOperators()
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

            if (Name == Pascal.Id && this._tokens.Peek().Token.Name == Pascal.LParinth)
            {
                return FunctionCall();
            }
            if (current.Token.Name == Pascal.Id)
            {
                return Variable();
            }
            if (current.Token.Name == Pascal.Equal)
            {
                Eat(Pascal.Equal);
            }

            if (current.Token.Name == Pascal.BoolConst)
            {
                return ParseBool();
            }
            return base.ParaUnaryOperators();
            //else
            //{
            //    var node = Variable();
            //    return node;
            //}
        }
        private ExpressionNode ParseBool()
        {
            var b = new BoolNode(_tokens.Current);
            _tokens.Advance();
            return b;
        }
        private FunctionCallNode FunctionCall()
        {
            var procedureName = Current.Value;
            var token = Current;
            Eat(Pascal.Id);
            Eat(Pascal.LParinth);
            var parameters = new List<Node>();
            while (Current.Token.Name != Pascal.RParinth)
            {
                parameters.Add(Expression());
                if (Current.Token.Name == Pascal.Comma)
                {
                    Eat(Pascal.Comma);
                }

            }
            Eat(Pascal.RParinth);
            return new FunctionCallNode(procedureName, parameters, token);
        }

        public Node Statement()
        {
            Node node = null;
            if (Name == Pascal.For)
            {
                node = ForLoop();
            }
            else if (Name == Pascal.Case)
            {
                node = CaseStatement();
            }
            else if (Name == Pascal.If)
            {
                node = IfStatement();
            }
            else if (Name == Pascal.Begin)
            {
                node = CompoundStatement();
            }
            else if (Name == Pascal.Id && _tokens.Peek()?.Token?.Name == Pascal.Assign)
            {
                node = AssignmentStatement();
            }
            else if (Name == Pascal.Id)
            {
                node = ProcedureCall();
            }
            else if (Name == Pascal.While)
            {
                node = WhileLoop();
            }
            else
            {
                node = Empty();
            }
            return node;
        }

        private WhileLoopNode WhileLoop()
        {
            Eat(Pascal.While);

            var exp = BoolExpression();
            Eat(Pascal.Do);

            var statement = Statement();
            return new WhileLoopNode(exp, statement);
        }

        private CaseStatementNode CaseStatement()
        {
            Eat(Pascal.Case);
            var exp = Expression();
            Eat(Pascal.Of);
            var items = new List<CaseItemNode>();
            while (Name != Pascal.End && Name != Pascal.Else)
            {
                items.Add(CaseItem());
            }

            Node elseExpressionNode = null;
            if (Name == Pascal.Else)
            {
                Eat(Pascal.Else);
                elseExpressionNode = Statement();
                Eat(Pascal.Semi);
            }
            Eat(Pascal.End);
            return new CaseStatementNode(exp, items, elseExpressionNode);
        }

        private CaseItemNode CaseItem()
        {
            var items = new List<ExpressionNode>(){Expression()};
            while (Name == Pascal.Comma)
            {
                Eat(Pascal.Comma);
                items.Add(Expression());
            }
            Eat(Pascal.Colon);
            var statement = Statement();
            Eat(Pascal.Semi);
            return new CaseItemNode(items, statement);

        }

        private Node ForLoop()
        {
            Eat(Pascal.For);
            var assign = AssignmentStatement();
            Eat(Pascal.To);
            var ex = Expression();
            Eat(Pascal.Do);
            var statements = Statement();
            return new ForLoopNode(assign, ex, statements);
        }

        Node BoolExpression()
        {
            if (Name == Pascal.Not)
            {
                var current = Current;
                Eat(Pascal.Not);
                return new NegationOperator(Expression(),current);
            }
            return Expression();
        }
        private IfStatementNode IfStatement()
        {
            Eat(Pascal.If);
            var equal = BoolExpression();
            Eat(Pascal.Then);
            
            var tstatement = Statement();
            IList<Node> estate = null;
            if (Current.Token.Name == Pascal.Else)
            {
                Eat(Pascal.Else);
                estate = new List<Node>(){ Statement() }; ;
            }
            return new IfStatementNode(equal, new List<Node>(){tstatement}, estate);
        }

        private ProcedureCallNode ProcedureCall()
        {
            var procedureName = Current.Value;
            var token = Current;
            Eat(Pascal.Id);
            var parameters = new List<Node>();

            if (Name == Pascal.LParinth)
            {
                Eat(Pascal.LParinth);
                while (Name != Pascal.RParinth)
                {
                    parameters.Add(Expression());
                    if (Name == Pascal.Comma)
                    {
                        Eat(Pascal.Comma);
                    }
                    else
                    {
                        break;
                    }

                }
                Eat(Pascal.RParinth);
            }
           
            return new ProcedureCallNode(procedureName, parameters, token);
        }

        AssignmentNode AssignmentStatement()
        {
            var left = Variable();
            var token = _tokens.Current;
            Eat(Pascal.Assign);
            var right = Expression();
            var node = new AssignmentNode(left, token, right);
            return node;
        }

        VariableOrFunctionCall Variable()
        {
            var node = new VariableOrFunctionCall(_tokens.Current);
            Eat(Pascal.Id);
            return node;
        }
        Node Parse()
        {
            var node = Program();
            if (this._tokens.Current != null)
            {
                Error("null");
            }
            return node;
        }
        NoOp Empty()
        {
            return new NoOp();
        }
        Node Program()
        {
            Eat(Pascal.Program);
            var name = _tokens?.Current;
            Eat(Pascal.Id);
            Eat(Pascal.Semi);
            var node = Block();
            Eat(Pascal.Dot);
            var programNode = new PascalProgramNode(name, node);
            return programNode;
        }
        public override Node Evaluate()
        {
            return Evaluate(items);
        }

        public Node Evaluate(IList<TokenItem> tokens)
        {
            CreateIterator(tokens);
            return Parse();
        }

        public void CreateIterator(IList<TokenItem> tokens)
        {
            _tokens = new Iterator<TokenItem>(tokens.ToArray());
        }

        
    }

    public class ConstantDeclarationNode : Node
    {
        public string ConstantName { get; }
        public Node Value { get; }
        public TokenItem TokenItem { get; }

        public ConstantDeclarationNode(string constantName, Node value, TokenItem tokenItem)
        {
            ConstantName = constantName;
            Value = value;
            TokenItem = tokenItem;
        }

        public override string Display()
        {
            return $"Constant({ConstantName}, {Value})";
        }
    }
}