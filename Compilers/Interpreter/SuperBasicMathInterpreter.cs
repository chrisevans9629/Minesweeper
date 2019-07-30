using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class SuperBasicMathInterpreter
    {

        object VisitNum(RealNode num)
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
            throw new ParserException(ErrorCode.UnexpectedToken, null, $"did not recognize node '{node}'");
        }

        protected virtual object VisitNode(Node node)
        {
            if (node is RealNode leaf)
            {
                return VisitNum(leaf);
            }

            if (node is IntegerNode integer)
            {
                return integer.Value;
            }

            if (node is BinaryOperator op) return VisitBin(op);
            if (node is UnaryOperator un) return VisitUnary(un);
            return Fail(node);
        }
        object VisitBin(BinaryOperator op)
        {
            var actions = new Dictionary<string, Func<double, double, double>>();
            actions.Add(Pascal.Add, (d, d1) => d + d1);
            actions.Add(Pascal.Sub, (d, d1) => d - d1);
            actions.Add(Pascal.Multi, (d, d1) => d * d1);
            actions.Add(Pascal.FloatDiv, (d, d1) => d / d1);
            actions.Add(Pascal.IntDiv, (d, d1) => (int)d / (int)d1);

            if (actions.ContainsKey(op.Name) != true)
            {
                return Fail(op);
            }

            var left = Convert.ToDouble(VisitNode(op.Left));
            var right = Convert.ToDouble(VisitNode(op.Right));

            return actions[op.Name](left, right);
            //if (op.Name == Pascal.Add)
            //{
            //    return ((double)VisitNode(op.Left)) + ((double)VisitNode(op.Right));
            //}

            //if (op.Name == Pascal.Sub)
            //{
            //    return ((double)VisitNode(op.Left)) - ((double)VisitNode(op.Right));
            //}

            //if (op.Name == Pascal.Multi)
            //{
            //    return ((double)VisitNode(op.Left)) * ((double)VisitNode(op.Right));
            //}

            //if (op.Name == Pascal.FloatDiv)
            //{
            //    return ((double)VisitNode(op.Left)) / ((double)VisitNode(op.Right));
            //}

            //if (op.Name == Pascal.IntDiv)
            //{
            //    return Convert.ToDouble(Convert.ToInt32((double)VisitNode(op.Left)) / Convert.ToInt32((double)VisitNode(op.Right))) ;
            //}

            //return Fail(op);

        }

        public virtual object Interpret(Node node)
        {
            return VisitNode(node);
        }
    }
}