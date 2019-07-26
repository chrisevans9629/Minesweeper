using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Tests;
using NUnit.Framework.Constraints;

namespace Minesweeper.Test
{
    public class SuperBasicMathAst : AbstractSyntaxTreeBase
    {
        public SuperBasicMathAst(IList<TokenItem> data, ILogger logger = null) : base(logger)
        {
           
            _tokens = new Iterator<TokenItem>(data.ToArray());
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
            var result = Expression();
            return result;
        }

       


    }
}