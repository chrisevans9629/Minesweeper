using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Minesweeper.Test
{
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

        IList<TokenItem> GetToken(string str)
        {
            var items = new List<TokenItem>();

            if (string.IsNullOrEmpty(str)) return new List<TokenItem>();
            foreach (var token in tokens)
            {
                var match = Regex.Match(str, "^" + token.Expression);
                if (match.Success)
                {
                    var newStr = str.Remove(match.Index, match.Length);
                    var t = new TokenItem() { Token = token, Value = match.Value, Position = match.Index };
                    items.Add(t);
                    items.AddRange(GetToken(newStr));
                    return items;
                }
            }

            return items;
        }

        public IList<TokenItem> Tokenize(string str)
        {
            return GetToken(str);
            var items = new List<TokenItem>();

            var matches = tokens.Select(p => new { match = Regex.Matches(str, (string) p.Expression), p });
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