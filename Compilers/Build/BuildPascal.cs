using System.IO;
using Microsoft.Build.Utilities;
using Minesweeper.Test;
using Minesweeper.Test.Symbols;

namespace Compilers.Build
{
    using Microsoft.Build.Utilities;

    public class BuildPascal : Task
    {
        public string FileName { get; set; }
        public override bool Execute()
        {
            //System.Diagnostics.Debugger.Launch();

            var text = File.ReadAllText(FileName);

            var lexer =new PascalLexer();
            var tokens = lexer.Tokenize(text);

            var ast = new PascalAst();

            var node = ast.Evaluate(tokens);

            var syntaxChecker = new PascalSemanticAnalyzer();
            syntaxChecker.CheckSyntax(node);

            return true;
        }
    }
}