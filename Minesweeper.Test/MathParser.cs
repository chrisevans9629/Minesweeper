using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class MathParser
    {

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
                    var val = new NumberValue() {Value = t, StringValue = currentValue};
                    previousValue = val;
                    
                    operators.Add(val);
                }
                var lastOp = operators.OfType<NumberOperator>().LastOrDefault();

                if (value[i] == '-')
                {
                    if (lastOp != null)
                    {
                        operators.Add(new SecondSubtract() { First = lastOp });
                    }
                    else
                    {
                        operators.Add(new Subtract() { First = previousValue });

                    }
                    continue;
                }

                if (value[i] == '+')
                {
                    if (lastOp != null)
                    {
                        operators.Add(new SecondAdd() { First = lastOp });
                    }
                    else
                    {
                        operators.Add(new Add() { First = previousValue });

                    }
                    continue;
                }

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
    }
}