using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Tests;

namespace Minesweeper.Test
{
    public class PascalLexer
    {
        private readonly ILogger _logger;
        private string _str;
        private int index;
        private int Line;
        private int Column;

        public PascalLexer(ILogger logger = null)
        {
            _logger = logger ?? new Logger();
        }
        public PascalLexer(string str, ILogger logger = null) : this(logger)
        {
            _str = str;
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
            if (Current == '\n')
            {
                Line++;
                Column = 0;
            }

            Column++;
            index++;
        }

        TokenItem Id()
        {
            var result = "";

           
            while (char.IsLetterOrDigit(Current) || Current == '_')
            {
                result += Current;
                Advance();
            }
            //uwu


            var token = CreateToken(Pascal.Id, result);


            var reserved = Pascal.Reservations.FirstOrDefault(p => result.ToUpper() == p.ToUpper());
            if (reserved != null)
            {
                token.Token.Name = reserved;
            }
            return token;
        }

        TokenItem CreateToken(string name, string value)
        {
            var token = new TokenItem()
            {
                Index = index-value.Length,
                Column = Column-value.Length,
                Line = Line,
                Value = value,
                Token = new Token()
                {
                    Name = name
                }
            };
            return token;
        }

        public IList<TokenItem> Tokenize(string str)
        {
            _str = str;
            _logger.Log($"Tokenizing String:\n'{_str}'");
            index = 0;
            Column = 1;
            Line = 1;
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
                        items.Add(CreateToken(Pascal.Num, num));
                    }
                }
               
                else if (Current == '{')
                {
                    Advance();
                    while (Current != '}' && Current != default(char))
                    {
                        Advance();
                    }
                    Advance();
                }
                else if (Pascal.SingleTokens.Contains(Current.ToString()))
                {
                    var item = Pascal.SingleTokens.First(p => p == Current.ToString());

                    Advance();

                    items.Add(CreateToken(item, item));
                }
                else
                {
                    var current = Current;
                    Advance();
                    var token = CreateToken("Error", current.ToString());
                    throw new LexerException(ErrorCode.UnexpectedToken, token, 
                        $"Unexpected token '{current}' at index {token.Index} line {token.Line} column {token.Column}");
                }
            }

            return items;
        }

        public IList<TokenItem> Tokenize()
        {
            return Tokenize(_str);
        }


    }
}