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
            var parameters = new List<Parameter>();
            Eat(Pascal.Function);
            var procedureId = Current.Value;
            var token = Current;
            Eat(Pascal.Id);
            if (Current.Token.Name == Pascal.LParinth)
            {
                Eat(Pascal.LParinth);
                while (Current.Token.Name != Pascal.RParinth)
                {
                    parameters.AddRange(VariableDeclaration().Select(p => new Parameter(p)));
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
        private ProcedureDeclarationNode ProcedureDeclaration()
        {
            var parameters = new List<Parameter>();
            Eat(Pascal.Procedure);
            var procedureId = Current.Value;
            Eat(Pascal.Id);
            if (Current.Token.Name == Pascal.LParinth)
            {
                Eat(Pascal.LParinth);
                while (Current.Token.Name != Pascal.RParinth)
                {
                    parameters.AddRange(VariableDeclaration().Select(p => new Parameter(p)));
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
            var name = Current.Value;
            Eat(Pascal.Id);
            Eat(Pascal.Equal);
            var value = Expression();
            Eat(Pascal.Semi);
            return new ConstantDeclarationNode(name, value);
        }

        private IList<VarDeclarationNode> VariableDeclaration()
        {
            var nodes = new List<Variable> { Variable() };
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

        protected override Node ParaUnaryOperators()
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
        private Node ParseBool()
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
            else node = Empty();
            return node;
        }

        private Node ForLoop()
        {
            Eat(Pascal.For);
            var assign = AssignmentStatement();
            Eat(Pascal.To);
            var ex = Expression();
            Eat(Pascal.Do);
            var statements = StatementList();
            return new ForLoopNode(assign, ex, statements);
        }

        private Node IfStatement()
        {
            Eat(Pascal.If);
            var equal = Expression() as EqualExpression;
            Eat(Pascal.Then);
            var tstatement = StatementList();
            IList<Node> estate = null;
            if (Current.Token.Name == Pascal.Else)
            {
                Eat(Pascal.Else);
                estate = StatementList();
            }
            return new IfStatementNode(equal, tstatement, estate);
        }

        private Node ProcedureCall()
        {
            var procedureName = Current.Value;
            var token = Current;
            Eat(Pascal.Id);
            var parameters = new List<Node>();

            if (Name == Pascal.LParinth)
            {
                Eat(Pascal.LParinth);
                while (Current.Token.Name != Pascal.RParinth)
                {
                    parameters.Add(Expression());
                    if (Current.Token.Name == Pascal.Comma)
                    {
                        Eat(Pascal.Comma);
                    }

                }
                Eat(Pascal.RParinth);
            }
           
            return new ProcedureCallNode(procedureName, parameters, token);
        }

        AssignNode AssignmentStatement()
        {
            var left = Variable();
            var token = _tokens.Current;
            Eat(Pascal.Assign);
            var right = Expression();
            var node = new AssignNode(left, token, right);
            return node;
        }

        Variable Variable()
        {
            var node = new Variable(_tokens.Current);
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

        public ConstantDeclarationNode(string constantName, Node value)
        {
            ConstantName = constantName;
            Value = value;
        }

        public override string Display()
        {
            return $"Constant({ConstantName}, {Value})";
        }
    }
}