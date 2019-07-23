using System.Collections.Generic;

namespace Minesweeper.Test
{

    public class GlobalMemory : Dictionary<string, object>
    {

    }

    public class PascalInterpreter : SuperBasicMathInterpreter
    {
        public object GetVar(string key)
        {
            return scope[key.ToLower()];
        }
        private GlobalMemory scope = new GlobalMemory();
        object VisitCompound(CompoundStatement compound)
        {
            foreach (var compoundNode in compound.Nodes)
            {
                VisitNode(compoundNode);
            }

            return compound;
        }

        public override object VisitNode(Node node)
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
            return base.VisitNode(node);
        }

        private object VisitProgram(PascalProgram program)
        {
            return VisitBlock(program.Block);
        }

        object VisitVarDeclaration(VarDeclaration varDeclaration)
        {
            this.scope.Add(varDeclaration.VarNode.VariableName, null);
            return null;
        }
        private object VisitBlock(Block block)
        {
            foreach (var blockDeclaration in block.Declarations)
            {
                VisitVarDeclaration(blockDeclaration);
            }

            return VisitNode(block.CompoundStatement);
        }

        object VisitAssign(Assign node)
        {
            var name = node.Left.VariableName;
            var value = VisitNode(node.Right);
            if (scope.ContainsKey(name))
            {
                scope[name] = value;
            }
            else
            {
                scope.Add(name, value);
            }

            return value;
        }

        object VisitVariable(Variable var)
        {
            var name = var.VariableName;
            var value = scope[name];
            return value;
        }

        static object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}