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
            if (op.Name == Pascal.Add)
            {
                return VisitNode(op.Value);
            }

            if (op.Name == Pascal.Sub)
            {
                var value = VisitNode(op.Value);
                if (value is double d)
                {
                    return -d;
                }
                if (value is int i)
                {
                    return -i;
                }
            }
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

            if (node is BinaryOperator op) return VisitBinaryOperator(op);
            if (node is UnaryOperator un) return VisitUnary(un);
            return Fail(node);
        }
        public object VisitBinaryOperator(BinaryOperator op)
        {
            var doubleActions = new Dictionary<string, Func<double, double, double>>();
            doubleActions.Add(Pascal.Add, (d, d1) => d + d1);
            doubleActions.Add(Pascal.Sub, (d, d1) => d - d1);
            doubleActions.Add(Pascal.Multi, (d, d1) => d * d1);
            doubleActions.Add(Pascal.FloatDiv, (d, d1) => d / d1);
            doubleActions.Add(Pascal.IntDiv, (d, d1) => (int)d / (int)d1);

            var intActions = new Dictionary<string, Func<int, int, int>>();
            intActions.Add(Pascal.Add, (d, d1) => d + d1);
            intActions.Add(Pascal.Sub, (d, d1) => d - d1);
            intActions.Add(Pascal.Multi, (d, d1) => d * d1);
            intActions.Add(Pascal.FloatDiv, (d, d1) => d / d1);
            intActions.Add(Pascal.IntDiv, (d, d1) => d / d1);
            if (doubleActions.ContainsKey(op.Name) != true)
            {
                return Fail(op);
            }

            var left = VisitNode(op.Left);
            var right = VisitNode(op.Right);
            if (left is int l && right is int r)
            {
                return intActions[op.Name](l, r);
            }
            else
            {
                return doubleActions[op.Name](Convert.ToDouble(left) ,Convert.ToDouble(right) );
            }
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