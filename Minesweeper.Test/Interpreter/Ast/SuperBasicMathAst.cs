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
            lex.Add("LPA", @"\(");
            lex.Add("RPA", @"\)");
            lex.Add("NUM", @"\d+");
            lex.Add("ADD", @"\+");
            lex.Add("SUB", @"-");
            lex.Add("MUL", @"\*");
            lex.Add(SimpleTree.FloatDiv, @"/");
        }

        

        public override Node Evaluate()
        {
            Eat(null);
            var result = Expression();
            return result;
        }

       


    }
}