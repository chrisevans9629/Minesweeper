using System.Collections.Generic;

namespace Minesweeper.Test
{

    public class GlobalMemory : Dictionary<string, object>
    {

    }

    public class PascalInterpreter : SuperBasicMathInterpreter
    {
        public override object Interpret(Node node)
        {
            _scope = new GlobalMemory();
            var result = base.Interpret(node);
            return _scope;
        }

        public object GetVar(string key)
        {
            return _scope[key.ToLower()];
        }
        private GlobalMemory _scope = new GlobalMemory();
        object VisitCompound(CompoundStatement compound)
        {
            foreach (var compoundNode in compound.Nodes)
            {
                VisitNode(compoundNode);
            }

            return compound;
        }

        protected override object VisitNode(Node node)
        {
            if (node is CompoundStatement compound)
            {
                return VisitCompound(compound);
            }

            if (node is Assign assign)
            {
                return VisitAssign(assign);
            }

            if (node is Variable var)
            {
                return VisitVariable(var);
            }

            if (node is NoOp no)
            {
                return VisitNoOp(no);
            }

            if (node is PascalProgram program)
            {
                return VisitProgram(program);
            }

            if (node is VarDeclaration declaration)
            {
                return VisitVarDeclaration(declaration);
            }
            return base.VisitNode(node);
        }

        private object VisitProgram(PascalProgram program)
        {
            return VisitBlock(program.Block);
        }

        object VisitVarDeclaration(VarDeclaration varDeclaration)
        {
            this._scope.Add(varDeclaration.VarNode.VariableName, null);
            return null;
        }
        private object VisitBlock(Block block)
        {
            foreach (var blockDeclaration in block.Declarations)
            {
                VisitNode(blockDeclaration);
            }

            return VisitNode(block.CompoundStatement);
        }

        object VisitAssign(Assign node)
        {
            var name = node.Left.VariableName;
            var value = VisitNode(node.Right);
            if (_scope.ContainsKey(name))
            {
                _scope[name] = value;
            }
            else
            {
                _scope.Add(name, value);
            }

            return value;
        }

        object VisitVariable(Variable var)
        {
            var name = var.VariableName;
            var value = _scope[name];
            return value;
        }

        static object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}