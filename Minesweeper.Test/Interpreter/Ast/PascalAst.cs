using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class PascalAst : AbstractSyntaxTreeBase
    {
        private TokenItem Current => _tokens.Current;

        private IList<TokenItem> items;
        public PascalAst(IList<TokenItem> tokens)
        {
            items = tokens;
            this._tokens = tokens.GetEnumerator();
        }

        public PascalAst()
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

        private Block Block()
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
            var nodes = new List<Variable>{Variable()};
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
            var result = new TypeNode(Current);

            if (Current.Token.Name == Pascal.Int)
            {
                Eat(Pascal.Int);
            }
            if(Current.Token.Name == Pascal.Real) Eat(Pascal.Real);
            return result;
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
            {
                node = CompoundStatement();
            }
            else if (_tokens.Current.Token.Name == Pascal.Id)
            {
                node = AssignmentStatement();
            }
            else node = Empty();
            return node;
        }

        Assign AssignmentStatement()
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
        NoOp Empty()
        {
            return new NoOp();
        }
        Node Program()
        {
            Eat(Pascal.Program);
            var name = _tokens.Current;
            Eat(Pascal.Id);
            Eat(Pascal.Semi);
            var node = Block();
            Eat(Pascal.Dot);
            var programNode = new PascalProgram(name, node);
            return programNode;
        }
        public override Node Evaluate()
        {
            return Evaluate(items);
        }

        public Node Evaluate(IList<TokenItem> tokens)
        {
            _tokens = tokens.GetEnumerator();
            Eat(null);
            return Parse();
        }
    }
}