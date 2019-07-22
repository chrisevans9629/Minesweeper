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
            if (op.Name == SimpleTree.FloatDiv) return ((double)VisitNode(op.Left)) / ((double)VisitNode(op.Right));
            if (op.Name == SimpleTree.IntDiv) return Convert.ToDouble(Convert.ToInt32((double)VisitNode(op.Left)) / Convert.ToInt32((double)VisitNode(op.Right))) ;

            return Fail(op);

        }

        public object Evaluate(Node node)
        {
            return VisitNode(node);
        }
    }
}