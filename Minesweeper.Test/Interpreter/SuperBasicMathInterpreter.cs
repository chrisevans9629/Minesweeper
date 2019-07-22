using System;
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
    public class SuperBasicMathInterpreter
    {
        object VisitNum(NumberLeaf num)
        {
            return num.Value;
        }

        object VisitUnary(UnaryOperator op)
        {
            if (op.Name == SimpleTree.Add) return VisitNode(op.Value);
            if (op.Name == SimpleTree.Sub) return -((double)VisitNode(op.Value));
            return Fail(op);
        }

        object Fail(Node node)
        {
            throw new Exception($"did not recognize node {node.Name}");
        }

        public virtual object VisitNode(Node node)
        {
            if (node is NumberLeaf leaf)
            {
                return VisitNum(leaf);
            }

            if (node is BinaryOperator op) return VisitBin(op);
            if (node is UnaryOperator un) return VisitUnary(un);
            return Fail(node);
        }
        object VisitBin(BinaryOperator op)
        {
            if (op.Name == SimpleTree.Add) return ((double)VisitNode(op.Left)) + ((double)VisitNode(op.Right));
            if (op.Name == SimpleTree.Sub) return ((double)VisitNode(op.Left)) - ((double)VisitNode(op.Right));
            if (op.Name == SimpleTree.Multi) return ((double)VisitNode(op.Left)) * ((double)VisitNode(op.Right));
            if (op.Name == SimpleTree.Div) return ((double)VisitNode(op.Left)) / ((double)VisitNode(op.Right));

            return Fail(op);

        }

        public object Evaluate(Node node)
        {
            return VisitNode(node);
        }
    }
}