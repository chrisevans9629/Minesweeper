using System.Collections.Generic;
using Minesweeper.Test.Tests;
using NUnit.Framework.Constraints;

namespace Minesweeper.Test
{
    public class SuperBasicMathAst : AbstractSyntaxTreeBase
    {
        public SuperBasicMathAst(IList<TokenItem> data, ILogger logger = null) : base(logger)
        {
           
            _tokens = data.GetEnumerator();
            //_tokens = GetTokens();
        }

        public static void AddMathTokens(RegexLexer lex)
        {
            lex.Ignore(" ");
            lex.Add(Pascal.LParinth, @"\(");
            lex.Add(Pascal.RParinth, @"\)");
            lex.Add(Pascal.Num, @"\d+");
            lex.Add(Pascal.Add, @"\+");
            lex.Add(Pascal.Sub, @"-");
            lex.Add(Pascal.Multi, @"\*");
            lex.Add(Pascal.FloatDiv, @"/");
        }

        

        public override Node Evaluate()
        {
            Eat(null);
            var result = Expression();
            return result;
        }

       


    }
}