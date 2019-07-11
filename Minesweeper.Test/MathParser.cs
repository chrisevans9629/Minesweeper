using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class SecondMultiply : DoubleOperator
    {
        public override double Calculate(double first, double second)
        {
            return first * second;
        }
    }

    public class Multiply : NumberOperator
    {
        public override double Calculate(double first, double second)
        {
            return first * second;
        }
    }

    public class MathOperatorCollection
    {
        public void Sort()
        {

        }

        public MathValue ParseChar(char value, NumberOperator lastOp, NumberValue previousValue)
        {
            if (operators.ContainsKey(value))
            {
                var opFunc = operators[value];
                if (lastOp != null)
                {
                    var op = opFunc();
                    op.DoubleOperator.First = lastOp;
                    return op.DoubleOperator;
                }
                else
                {
                    var op = opFunc();
                    op.NumberOperator.First = previousValue;
                    return op.NumberOperator;
                }
            }

            return null;
        }
        public void AddOperator(char value, Operation op)
        {
            operators.Add(value, () => new FullOperator(op));
        }



        private Dictionary<char, Func<FullOperator>> operators = new Dictionary<char, Func<FullOperator>>();
    }

    public class NumberOperatorInject : NumberOperator
    {
        private readonly Operation _operation;

        public NumberOperatorInject(Operation operation)
        {
            _operation = operation;
        }
        public override double Calculate(double first, double second)
        {
            return _operation(first, second);
        }
    }

    public delegate double Operation(double first, double second);
    public class DoubleOperatorInject : DoubleOperator
    {
        private readonly Operation _operation;

        public DoubleOperatorInject(Operation operation)
        {
            _operation = operation;
        }
        public override double Calculate(double first, double second)
        {
            return _operation(first, second);
        }
    }

    public class FullOperator
    {
        public FullOperator(Operation operation)
        {
            DoubleOperator = new DoubleOperatorInject(operation);
            NumberOperator = new NumberOperatorInject(operation);
        }
        public DoubleOperator DoubleOperator { get; set; }
        public NumberOperator NumberOperator { get; set; }
    }

    public class MathParser
    {
        MathOperatorCollection collection = new MathOperatorCollection();
        public MathParser()
        {
            collection.AddOperator('*', (first, second) => first * second);
            collection.AddOperator('+', (first, second) => first + second);
            collection.AddOperator('-', (first, second) => first - second);
        }
        public double Calculate(string value)
        {
            var operators = new List<MathValue>();
            NumberValue previousValue = null;
            for (var i = 0; i < value.Length; i++)
            {
                var currentValue = value[i].ToString();
                var last = operators.LastOrDefault();
                if (last is NumberValue v)
                {
                    currentValue = v.StringValue + currentValue;
                    if (double.TryParse(currentValue, out var ts))
                    {
                        previousValue = v;
                        v.Value = ts;
                        v.StringValue = currentValue;
                        continue;
                    }
                }
                if (double.TryParse(currentValue, out var t) || currentValue == ".")
                {
                    var val = new NumberValue() { Value = t, StringValue = currentValue };
                    previousValue = val;

                    operators.Add(val);
                }
                var lastOp = operators.OfType<NumberOperator>().LastOrDefault();

                var tresult = collection.ParseChar(value[i], lastOp, previousValue);
                if(tresult != null) operators.Add(tresult);
                //MultiplyOperator(value[i], lastOp, operators, previousValue);
                //if (value[i] == '-')
                //{
                //    if (lastOp != null)
                //    {
                //        operators.Add(new SecondSubtract() { First = lastOp });
                //    }
                //    else
                //    {
                //        operators.Add(new Subtract() { First = previousValue });

                //    }
                //    continue;
                //}

                //if (value[i] == '+')
                //{
                //    if (lastOp != null)
                //    {
                //        operators.Add(new SecondAdd() { First = lastOp });
                //    }
                //    else
                //    {
                //        operators.Add(new Add() { First = previousValue });

                //    }
                //    continue;
                //}

                var secAdd = operators.OfType<DoubleOperator>().LastOrDefault();
                if (secAdd != null && secAdd.Second == null)
                {
                    secAdd.Second = previousValue;
                    continue;
                }
                if (lastOp != null && lastOp.Second == null)
                {
                    lastOp.Second = previousValue;
                    continue;
                }
            }

            var si = operators.OfType<NumberValue>().FirstOrDefault();
            if (si != null && operators.Count == 1)
            {
                return si.Value ?? 0;
            }

            return operators.OfType<Operator>().Sum(p => p.Calculate());
        }

        private static void MultiplyOperator(char value, NumberOperator lastOp, List<MathValue> operators,
            NumberValue previousValue)
        {
            if (value == '*')
            {
                if (lastOp != null)
                {
                    operators.Add(new SecondMultiply() { First = lastOp });
                }
                else
                {
                    operators.Add(new Multiply() { First = previousValue });
                }
            }
        }
    }
}