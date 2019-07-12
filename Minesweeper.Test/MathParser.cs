using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    //public class SecondMultiply : DoubleOperator
    //{
    //    public override double Calculate(double first, double second)
    //    {
    //        return first * second;
    //    }
    //}

    //public class Multiply : NumberOperator
    //{
    //    public override double Calculate(double first, double second)
    //    {
    //        return first * second;
    //    }
    //}

    public class MathOperatorCollection
    {
        public void Sort()
        {

        }

        public IMathValue ParseChar(char value, NumberOperator lastOp, NumberValue previousValue)
        {
            if (operators.ContainsKey(value))
            {
                var opFunc = operators[value];
                if (lastOp != null)
                {
                    var op = opFunc();
                    op.First = lastOp;
                    return op;
                }
                else
                {
                    var op = opFunc();
                    op.First = previousValue;
                    return op;
                }
            }

            return null;
        }
        public void AddOperator(char value, Operation op)
        {
            operators.Add(value, () => new NumberOperatorInject(op));
        }



        private Dictionary<char, Func<NumberOperator>> operators = new Dictionary<char, Func<NumberOperator>>();
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
    //public class DoubleOperatorInject : DoubleOperator
    //{
    //    private readonly Operation _operation;

    //    public DoubleOperatorInject(Operation operation)
    //    {
    //        _operation = operation;
    //    }
    //    public override double Calculate(double first, double second)
    //    {
    //        return _operation(first, second);
    //    }
    //}

    //public class FullOperator
    //{
    //    public FullOperator(Operation operation)
    //    {
    //        NumberOperator = new NumberOperatorInject(operation);
    //    }
    //    public NumberOperator NumberOperator { get; set; }
    //}
    public interface IMathParser
    {
        double Calculate(string value);
    }
    public class MathParserTree : IMathParser
    {
        public double Calculate(string value)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberParser
    {
        public static (bool, NumberValue) ParseNumber(IMathValue lastNode, string currentValue, List<IMathValue> operators)
        {
            NumberValue previousValue = null;
            //add to the number value
            if (lastNode is NumberValue v)
            {
                currentValue = v.StringValue + currentValue;
                if (Double.TryParse(currentValue, out var ts))
                {
                    previousValue = v;
                    v.Value = ts;
                    v.StringValue = currentValue;
                    return (true, previousValue);
                }
            }
            else if (Double.TryParse(currentValue, out var t) || currentValue == ".")
            {
                var val = new NumberValue() { Value = t, StringValue = currentValue };
                previousValue = val;

                operators.Add(val);
            }

            return (false, previousValue);
        }
    }
    public class MathParser : IMathParser
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
            
            var operators = new List<IMathValue>();
            NumberValue previousValue = null;
            foreach (char valueChars in value)
            {
                var currentValue = valueChars.ToString();
                var last = operators.LastOrDefault();
                var parseResult = NumberParser.ParseNumber(last, currentValue, operators);
                if (parseResult.Item2 != null)
                {
                    previousValue = parseResult.Item2;
                }
                if (parseResult.Item1)
                {
                    continue;
                }
                var lastOp = operators.OfType<NumberOperator>().LastOrDefault();

                var tresult = collection.ParseChar(valueChars, lastOp, previousValue);
                if(tresult != null) operators.Add(tresult);
                

                //var secAdd = operators.OfType<DoubleOperator>().LastOrDefault();
                var secAdd = operators.OfType<NumberOperator>().LastOrDefault(p => p.First is NumberOperator);
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

            return operators.OfType<Operator>().Sum(p => p.Value ?? 0);
        }
    }
}