using System;

namespace Minesweeper.Test
{
    public class SuperBasicMathLispInterpreter
    {
        string VisitNum(NumberLeaf num)
        {
            return num.Value.ToString();
        }

        string VisitNode(Node node)
        {
            if (node is NumberLeaf leaf)
            {
                return VisitNum(leaf);
            }

            if (node is BinaryOperator op) return VisitBin(op);

            throw new Exception($"did not recognize node {node.Name}");
        }
        string VisitBin(BinaryOperator op)
        {
            if (op.Name == SimpleTree.Add) return "(+ " + (VisitNode(op.Left) + " " + VisitNode(op.Right) + ")");
            if (op.Name == SimpleTree.Sub) return "(- " + VisitNode(op.Left) + " " + VisitNode(op.Right) + ")";
            if (op.Name == SimpleTree.Multi) return "(* "+ VisitNode(op.Left) + " " + VisitNode(op.Right) + ")";
            if (op.Name == SimpleTree.Div) return "(/ " + VisitNode(op.Left) + " " + VisitNode(op.Right) + ")";

            throw new Exception($"did not recognize operation {op.Name}");

        }

        public string Evaluate(Node node)
        {
            return VisitNode(node).Trim();
        }
    }
}