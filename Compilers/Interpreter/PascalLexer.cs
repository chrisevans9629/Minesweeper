﻿using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test;

namespace Minesweeper.Test
{
    public class PascalLexer
    {
        private readonly ILogger _logger;
        private string _str;
       
        private LexerIterator iterator;
        public PascalLexer(ILogger logger = null)
        {
            _logger = logger ?? new Logger();
        }



        public char Current => iterator.Current;
        private int index => iterator.index;
        private int Column => iterator.Column;
        private int Line => iterator.Line;
        void Advance()
        {
            iterator.Advance();
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


            if (Pascal.Reservations.ContainsKey(result.ToUpper()))
            {
                token.Token = Pascal.Reservations[result.ToUpper()];
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

        public char Peek()
        {
            return iterator.Peek();
        }
        public IList<TokenItem> Tokenize(string str)
        {
            _str = str;
            _logger.Log($"Tokenizing String:\n'{_str}'");
            iterator = new LexerIterator(str.ToCharArray());
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
                else if(Current == '^' && char.IsLetter(Peek()))
                {
                    Advance();
                    items.Add(CreateToken(Pascal.Pointer,Current.ToString()));
                    Advance();
                }
                else if (Current == '<' && Peek() == '>')
                {
                    Advance();
                    Advance();
                    items.Add(CreateToken(Pascal.NotEqual, "<>"));
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
                        items.Add(CreateToken(Pascal.IntegerConst, num));
                    }
                }
                else if (Current == '\'')
                {
                    Advance();
                    var quote = "";
                    while (Current != '\'' && Current != default(char))
                    {
                        quote += Current;
                        Advance();
                    }
                    Advance();
                    items.Add(CreateToken(Pascal.StringConst, quote));
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
                        $"Unexpected token '{current}'");
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