using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class SimpleTree : IAbstractSyntaxTree
    {
        public void AddExpress(string expression, Func<IList<TokenItem>, double, double> eval)
        {
        }

        double Number(TokenItem token)
        {
            return double.Parse(token.Value);
        }
        public double Evaluate(IList<TokenItem> tokens)
        {
            if (tokens.Any() != true) return 0;

            using (var numerator = tokens.GetEnumerator())
            {
                numerator.MoveNext();
                var result = Number(numerator.Current);
                numerator.MoveNext();

                while (numerator.Current != null && numerator.Current.Token.Name != "Num")
                {
                    if (numerator.Current.Token.Name == "Add")
                    {
                        numerator.MoveNext();
                        result += Number(numerator.Current);
                        numerator.MoveNext();
                    }
                    else if(numerator.Current.Token.Name == "Sub")
                    {
                        numerator.MoveNext();
                        result -= Number(numerator.Current);
                        numerator.MoveNext();
                    }
                    else if (numerator.Current.Token.Name == "Multi")
                    {
                        numerator.MoveNext();
                        result *= Number(numerator.Current);
                        numerator.MoveNext();
                    }
                    else if (numerator.Current.Token.Name == "Div")
                    {
                        numerator.MoveNext();
                        result /= Number(numerator.Current);
                        numerator.MoveNext();
                    }
                    else
                    {
                        throw new Exception($"Could not find grammer for {numerator.Current}");
                    }
                }

                return result;

            }


        }
    }
}