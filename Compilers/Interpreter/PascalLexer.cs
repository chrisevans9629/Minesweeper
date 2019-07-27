using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test;

namespace Minesweeper.Test
{

    public class Iterator<T>
    {
        private T[] _str;
        public int index;

        public Iterator(T[] str)
        {
            index = 0;
            _str = str;
        }
        public T Peek()
        {
            if (index + 1 < _str.Length - 1)
                return _str[index + 1];
            return default(T);
        }
         public T Current => (index <= _str.Length - 1) ? _str[index] : default(T);
        public virtual void Advance()
        {
            index++;
        }
    }

    public class LexerIterator : Iterator<char>
    {
        public int Line;
        public int Column;
        public override void Advance()
        {
            if (Current == '\n')
            {
                Line++;
                Column = 0;
            }
            Column++;
            base.Advance();
        }

        public LexerIterator(char[] str) : base(str)
        {
            Line = 1;
            Column = 1;
        }
    }
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