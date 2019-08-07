using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class SuperBasicMathInterpreter
    {

        public object VisitReal(RealNode num)
        {
            return num.Value;
        }

        public object VisitUnary(UnaryOperator op)
        {
            if (op.Name == PascalTerms.Add)
            {
                return VisitNode(op.Value);
            }

            if (op.Name == PascalTerms.Sub)
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

        public virtual object VisitNode(Node node)
        {
            if (node is RealNode leaf)
            {
                return VisitReal(leaf);
            }

            if (node is IntegerNode integer)
            {
                return VisitInteger(integer);
            }

            if (node is BinaryOperator op)
            {
                return VisitBinaryOperator(op);
            }

            if (node is UnaryOperator un)
            {
                return VisitUnary(un);
            }
            return Fail(node);
        }

        public object VisitInteger(IntegerNode integer)
        {
            return integer.Value;
        }

        public object VisitBinaryOperator(BinaryOperator op)
        {
            var doubleActions = new Dictionary<string, Func<double, double, double>>();
            doubleActions.Add(PascalTerms.Add, (d, d1) => d + d1);
            doubleActions.Add(PascalTerms.Sub, (d, d1) => d - d1);
            doubleActions.Add(PascalTerms.Multi, (d, d1) => d * d1);
            doubleActions.Add(PascalTerms.FloatDiv, (d, d1) => d / d1);
            doubleActions.Add(PascalTerms.IntDiv, (d, d1) => (int)d / (int)d1);

            var intActions = new Dictionary<string, Func<int, int, int>>();
            intActions.Add(PascalTerms.Add, (d, d1) => d + d1);
            intActions.Add(PascalTerms.Sub, (d, d1) => d - d1);
            intActions.Add(PascalTerms.Multi, (d, d1) => d * d1);
            intActions.Add(PascalTerms.FloatDiv, (d, d1) => d / d1);
            intActions.Add(PascalTerms.IntDiv, (d, d1) => d / d1);


            var strActions = new Dictionary<string, Func<string, string, string>>();
            strActions.Add(PascalTerms.Add, (d, d1) => d + d1);

          

            var left = VisitNode(op.Left);
            var right = VisitNode(op.Right);
            if (left is int l && right is int r)
            {
                if (intActions.ContainsKey(op.Name) != true)
                {
                    return Fail(op);
                }
                return intActions[op.Name](l, r);
            }
            else if((left is int || left is double) && (right is int || right is double))
            {
                if (doubleActions.ContainsKey(op.Name) != true)
                {
                    return Fail(op);
                }
                return doubleActions[op.Name](Convert.ToDouble(left) ,Convert.ToDouble(right) );
            }
            else if (left is string || right is string)
            {
                if (strActions.ContainsKey(op.Name) != true)
                {
                    return Fail(op);
                }

                return strActions[op.Name](left.ToString(), right.ToString());
            }
            else
            {
               return Fail(op);
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