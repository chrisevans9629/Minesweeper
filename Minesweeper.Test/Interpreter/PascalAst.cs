using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class TypeNode : Node
    {
        public string TypeValue { get; set; }
        public TypeNode(TokenItem token)
        {
            TokenItem = token;
            TypeValue = token.Value;
        }
        public override string Display()
        {
            return $"Type({TypeValue})";
        }
    }
    public class VarDeclaration : Node
    {
        public Variable VarNode { get; set; }
        public TypeNode TypeNode { get; set; }

        public VarDeclaration(Variable varNode, TypeNode typeNode)
        {
            VarNode = varNode;
            TypeNode = typeNode;
        }

        public override string Display()
        {
            return $"VarDec({VarNode.Display()}, {TypeNode.Display()})";
        }
    }
    public class Block : Node
    {
        public IList<VarDeclaration> Declarations { get; }
        public Node CompoundStatement { get; }

        public Block(IList<VarDeclaration> declarations, Node compoundStatement)
        {
            Declarations = declarations;
            CompoundStatement = compoundStatement;
        }
        public override string Display()
        {
            return $"Block({Aggregate(Declarations)}, {CompoundStatement.Display()})";
        }
    }

    public class PascalProgram : Node
    {
        public string ProgramName { get; set; }
        public Node Block { get; set; }

        public PascalProgram(string name, Node block)
        {
            ProgramName = name;
            Block = block;
        }
        public override string Display()
        {
            return $"Program({ProgramName}, {Block.Display()})";
        }
    }

    public class PascalAst : AbstractSyntaxTreeBase
    {
        public PascalAst(IList<TokenItem> tokens)
        {
            this._tokens = tokens.GetEnumerator();
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

        Node Block()
        {
            var dec = Declarations();
            var comStat = CompoundStatement();
            var node = new Block(dec, comStat);
            return node;
        }

        private IList<VarDeclaration> Declarations()
        {
            var dec = new List<VarDeclaration>();
            if (this._tokens.Current.Token.Name == Pascal.Var)
            {
                Eat(Pascal.Var);
                while (_tokens.Current.Token.Name == Pascal.Id)
                {
                    var vardec = VariableDeclaration();
                    foreach (var varDeclaration in vardec)
                    {
                        dec.Add(varDeclaration);
                    }
                    Eat(Pascal.Semi);
                }
            }

            return dec;
        }

        private IList<VarDeclaration> VariableDeclaration()
        {
            var nodes = new List<Variable>(){Variable()};
            while (_tokens.Current.Token.Name == Pascal.Comma)
            {
                Eat(Pascal.Comma);
                nodes.Add(Variable());
            }
            Eat(Pascal.Colon);

            var type = TypeSpec();
            return nodes.Select(p => new VarDeclaration(p, type)).ToList();
        }

        private TypeNode TypeSpec()
        {
            throw new NotImplementedException();
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

        protected override Node ParaAddSub()
        {
            var current = _tokens.Current;
            var par = Parenthese();
            if (par != null) return par;

            if (current.Token.Name == SimpleTree.Add)
            {
                Eat(SimpleTree.Add);
                return new UnaryOperator(ParaAddSub(), current);
            }
            if (current.Token.Name == SimpleTree.Sub)
            {
                Eat(SimpleTree.Sub);
                return new UnaryOperator(ParaAddSub(), current);
            }

            if (current.Token.Name == Pascal.Id)
            {
                return Variable();
            }
            //else
            //{
            //    var node = Variable();
            //    return node;
            //}
            return ParseNumber();
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
            var left = Variable();
            var token = _tokens.Current;
            Eat(Pascal.Assign);
            var right = Expression();
            var node = new Assign(left, token, right);
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
            if(this._tokens.Current != null)
                throw new Exception($"did not expect {_tokens.Current.Token.Name}");
            return node;
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
            Eat(null);
            return Parse();
        }
    }
}