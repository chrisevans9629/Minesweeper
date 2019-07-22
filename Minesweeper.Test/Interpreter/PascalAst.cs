using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
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