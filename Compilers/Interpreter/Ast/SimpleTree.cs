using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class SimpleTree : IAbstractSyntaxTree
    {
        double Number(IEnumerator<TokenItem> token)
        {
            var result = double.Parse(token.Current.Value);
            Eat(token, Pascal.IntegerConst);
            return result;
        }
        public double Evaluate(IList<TokenItem> tokens)
        {
            if (tokens.Any() != true) return 0;

            using (var numerator = tokens.GetEnumerator())
            {
                Eat(numerator, null);

                return Expr(numerator);
            }
        }


      
        double Par(IEnumerator<TokenItem> numerator)
        {
            if (numerator.Current.Token.Name == Pascal.LParinth)
            {
                Eat(numerator, Pascal.LParinth);
                var result = Expr(numerator);
                Eat(numerator, Pascal.RParinth);
                return result;
            }
            var num = Number(numerator);
            return num;
        }
        private double Term(IEnumerator<TokenItem> numerator)
        {
            //var result = 0.0;
            //numerator.MoveNext();
            var result = Par(numerator);

            while (numerator.Current != null && numerator.Current.Token.Name != Pascal.IntegerConst)
            {
                if (numerator.Current.Token.Name == Pascal.Multi)
                {
                    Eat(numerator, Pascal.Multi);
                    result *= Par(numerator);
                    //numerator.MoveNext();
                }
                else if (numerator.Current.Token.Name == Pascal.FloatDiv)
                {
                    Eat(numerator, Pascal.FloatDiv);
                    result /= Par(numerator);
                    //numerator.MoveNext();
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        void Eat(IEnumerator<TokenItem> num, string item)
        {
            var name = num.Current?.Token?.Name;
            if (name != item) throw new Exception($"tried to eat '{item}' but was {name}");
            num.MoveNext();
        }

       
        private double Expr(IEnumerator<TokenItem> numerator)
        {

            var result = Term(numerator);

            while (numerator.Current != null && numerator.Current.Token.Name != Pascal.IntegerConst)
            {
                if (numerator.Current.Token.Name == Pascal.Add)
                {
                    Eat(numerator, Pascal.Add);
                    result += Term(numerator);
                    //numerator.MoveNext();
                }
                else if (numerator.Current.Token.Name == Pascal.Sub)
                {
                    Eat(numerator, Pascal.Sub);
                    result -= Term(numerator);
                    //numerator.MoveNext();
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}