﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class SimpleTree : IAbstractSyntaxTree
    {
        private const string Num = "NUM";
        public void AddExpress(string expression, Func<IList<TokenItem>, double, double> eval)
        {
        }

        double Number(IEnumerator<TokenItem> token)
        {
            var result = double.Parse(token.Current.Value);
            Eat(token, Num);
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


        private const string LParinth = "LPA";
        private const string RParinth = "RPA";
        private const string Multi = "MUL";
        private const string Div = "DIV";

        double Par(IEnumerator<TokenItem> numerator)
        {
            if (numerator.Current.Token.Name == LParinth)
            {
                Eat(numerator, LParinth);
                var result = Expr(numerator);
                Eat(numerator, RParinth);
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

            while (numerator.Current != null && numerator.Current.Token.Name != Num)
            {
                if (numerator.Current.Token.Name == Multi)
                {
                    Eat(numerator, Multi);
                    result *= Par(numerator);
                    //numerator.MoveNext();
                }
                else if (numerator.Current.Token.Name == Div)
                {
                    Eat(numerator, Div);
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

        private const string Add = "ADD";
        private const string Sub = "SUB";
        private double Expr(IEnumerator<TokenItem> numerator)
        {

            var result = Term(numerator);

            while (numerator.Current != null && numerator.Current.Token.Name != Num)
            {
                if (numerator.Current.Token.Name == Add)
                {
                    Eat(numerator, Add);
                    result += Term(numerator);
                    //numerator.MoveNext();
                }
                else if (numerator.Current.Token.Name == Sub)
                {
                    Eat(numerator, Sub);
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