using System;

namespace Minesweeper.Test
{
    public class SuperBasicMathInterpreter
    {

        object VisitNum(NumberLeaf num)
        {
            return num.Value;
        }

        object VisitUnary(UnaryOperator op)
        {
            if (op.Name == Pascal.Add) return VisitNode(op.Value);
            if (op.Name == Pascal.Sub) return -((double)VisitNode(op.Value));
            return Fail(op);
        }

        object Fail(Node node)
        {
            throw new Exception($"did not recognize node {node}");
        }

        protected virtual object VisitNode(Node node)
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
            if (op.Name == Pascal.Add) return ((double)VisitNode(op.Left)) + ((double)VisitNode(op.Right));
            if (op.Name == Pascal.Sub) return ((double)VisitNode(op.Left)) - ((double)VisitNode(op.Right));
            if (op.Name == Pascal.Multi) return ((double)VisitNode(op.Left)) * ((double)VisitNode(op.Right));
            if (op.Name == Pascal.FloatDiv) return ((double)VisitNode(op.Left)) / ((double)VisitNode(op.Right));
            if (op.Name == Pascal.IntDiv) return Convert.ToDouble(Convert.ToInt32((double)VisitNode(op.Left)) / Convert.ToInt32((double)VisitNode(op.Right))) ;

            return Fail(op);

        }

        public virtual object Interpret(Node node)
        {
            return VisitNode(node);
        }
    }
}