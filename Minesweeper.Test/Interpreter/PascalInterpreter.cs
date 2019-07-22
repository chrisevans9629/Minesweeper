using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class PascalInterpreter : SuperBasicMathInterpreter
    {
        public object GetVar(string key)
        {
            return scope[key.ToLower()];
        }
        private IDictionary<string, object> scope = new Dictionary<string, object>();
        object VisitCompound(Compound compound)
        {
            foreach (var compoundNode in compound.Nodes)
            {
                VisitNode(compoundNode);
            }

            return compound;
        }

        public override object VisitNode(Node node)
        {
            if (node is Compound compound) return VisitCompound(compound);
            if (node is Assign assign) return VisitAssign(assign);
            if (node is Variable var) return VisitVariable(var);
            if (node is NoOp no) return VisitNoOp(no);
            return base.VisitNode(node);
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

        object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}