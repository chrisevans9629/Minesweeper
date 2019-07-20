using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        public char Current => (index < _str.Length - 1) ? _str[index] : default(char);
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
                else
                {
                    throw new Exception($"did not recognize char '{Current}'");
                }
            }

            return items;
        }


    }
    public class Lexer
    {
        readonly IList<Token> tokens = new List<Token>();
        public void Add(string name, string expression)
        {
            tokens.Add(new Token() { Expression = expression, Name = name });
        }
        readonly IList<string> ignoreables = new List<string>();
        public void Ignore(string expression)
        {
            ignoreables.Add(expression);
        }



        public IList<TokenItem> Tokenize(string str)
        {
            var items = new List<TokenItem>();

            var matches = tokens.Select(p => new { match = Regex.Matches(str, (string)p.Expression), p });
            foreach (var match in matches)
            {
                foreach (Match o in match.match)
                {
                    if (o.Success)
                    {
                        items.Add(new TokenItem() { Token = match.p, Position = o.Index, Value = o.Value });
                    }
                }
            }

            //Validation
            {
                var rStr = str;
                foreach (var token in tokens)
                {
                    rStr = Regex.Replace(rStr, token.Expression, "");
                }

                foreach (var ignoreable in ignoreables)
                {
                    rStr = Regex.Replace(rStr, ignoreable, "");
                }

                if (string.IsNullOrEmpty(rStr) != true)
                {
                    var index = str.IndexOf(rStr, StringComparison.InvariantCulture);
                    throw new Exception($"Token '{rStr}' unrecognized at position {index} line 0");
                }
            }

            var resultTokens = items.OrderBy(p => p.Position).ToList();

            return resultTokens;
        }
    }
}