using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class PascalLexer
    {
        private readonly string _str;
        private int index = 0;

        public PascalLexer(string str)
        {
            _str = str.ToLower();
        }


        public char Peek()
        {
            if (index + 1 < _str.Length - 1)
                return _str[index + 1];
            return default(char);
        }

        public char Current => (index <= _str.Length - 1) ? _str[index] : default(char);
        public void Advance()
        {
            index++;
        }

        TokenItem Id()
        {
            var result = "";

            var reservations = new List<string>()
            {
                SimpleTree.IntDiv,
                Pascal.Begin,
                Pascal.End,
                Pascal.Var,
                Pascal.Int,
                Pascal.Real,
            };
            while (char.IsLetterOrDigit(Current) || Current == '_')
            {
                result += Current;
                Advance();
            }
            //uwu


            var token = CreateToken(Pascal.Id, result);


            var reserved = reservations.FirstOrDefault(p => result.ToUpper() == p);
            if (reserved != null)
            {
                token.Token.Name = reserved;
            }
            return token;
        }

        TokenItem CreateToken(string name, string value)
        {
            var token = new TokenItem() { Position = index, Value = value, Token = new Token() { Name = name } };
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
                else if (char.IsLetter(Current) || Current == '_')
                {
                    items.Add(Id());
                }
                else if (Current == ':' && Peek() == '=')
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

                else if (char.IsNumber(Current) || (Current == '.' && char.IsNumber(Peek())))
                {
                    //10.10 valid
                    //10.10.10 invalid
                    //10. 10 invalid
                    var num = "";
                    bool hasFloat = false;
                    while (char.IsNumber(Current) || Current == '.')
                    {
                        if (Current == '.' && hasFloat) throw new InvalidOperationException($"Number {num} cannot have two periods");
                        if (Current == '.')
                        {
                            hasFloat = true;
                        }
                        num += Current;
                        Advance();
                    }

                    if (hasFloat)
                    {
                        items.Add(CreateToken(Pascal.RealConst, num));
                    }
                    else
                    {
                        items.Add(CreateToken(SimpleTree.Num, num));
                    }
                }
                else if (Current == '.')
                {
                    Advance();
                    items.Add(CreateToken(Pascal.Dot, "."));
                }
                else if (Current == '+')
                {
                    Advance();

                    items.Add(CreateToken(SimpleTree.Add, "+"));
                }
                else if (Current == '-')
                {
                    Advance();

                    items.Add(CreateToken(SimpleTree.Sub, "-"));
                }
                else if (Current == '*')
                {
                    Advance();

                    items.Add(CreateToken(SimpleTree.Multi, "*"));
                }
                else if (Current == '/')
                {
                    Advance();

                    items.Add(CreateToken(SimpleTree.FloatDiv, "/"));
                }
                else if (Current == '(')
                {
                    Advance();

                    items.Add(CreateToken(SimpleTree.LParinth, "("));
                }
                else if (Current == ')')
                {
                    Advance();

                    items.Add(CreateToken(SimpleTree.RParinth, ")"));
                }
                else if (Current == ':')
                {
                    Advance();
                    items.Add(CreateToken(Pascal.Colon, ":"));
                }
                else if (Current == ',')
                {
                    Advance();
                    items.Add(CreateToken(Pascal.Comma, ","));
                }
                else if(Current == '{')
                {
                    Advance();
                    while (Current != '}' && Current != default(char))
                    {
                        Advance();
                    }
                    Advance();
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