using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class PascalLexer
    {
        private readonly string _str;
        private int index = 0;

        public PascalLexer(string str)
        {
            _str = str;
        }


        public char? Peek()
        {
            if (index + 1 < _str.Length - 1)
                return _str[index + 1];
            return null;
        }

        public char Current => (index <= _str.Length - 1) ? _str[index] : default(char);
        public void Advance()
        {
            index++;
        }

        TokenItem Id()
        {
            var result = "";
            while (char.IsLetterOrDigit(Current))
            {
                result += Current;
                Advance();
            }
            //uwu


            var token = CreateToken(Pascal.Id, result);
            if (result == Pascal.Begin) token.Token.Name = Pascal.Begin;
            else if (result == Pascal.End) token.Token.Name = Pascal.End;
            return token;
        }

        TokenItem CreateToken(string name, string value)
        {
            var token = new TokenItem() { Position = index, Value = value, Token = new Token() {Name = name}};
            return token;
        }
        public IList<TokenItem> Tokenize()
        {
            //rawr
            var items = new List<TokenItem>();
            while (Current != default(char))
            {
                if (char.IsWhiteSpace(Current))
                {
                    Advance();
                }
                else if (char.IsLetter(Current))
                {
                    items.Add(Id());
                }
                else if(Current == ':' && Peek() == '=')
                {
                    Advance();
                    Advance();
                    items.Add(CreateToken(Pascal.Assign, ":="));
                }
                else if (Current == ';')
                {
                    Advance();
                    items.Add(CreateToken(Pascal.Semi, ";"));
                }
                else if(Current == '.')
                {
                    Advance();
                    items.Add(CreateToken(Pascal.Dot, "."));
                }
                else if(char.IsNumber(Current))
                {
                    var num = "";
                    while (char.IsNumber(Current))
                    {
                        num += Current;
                        Advance();
                    }
                    items.Add(CreateToken(SimpleTree.Num, num));
                }
                else
                {
                    throw new Exception($"did not recognize char '{Current}'");
                }
            }

            return items;
        }


    }
}