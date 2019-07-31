using FluentAssertions;
using Minesweeper.Test.Symbols;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalToCSharpTests
    {
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer table;
        PascalToCSharp cSharp;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
            table = new PascalSemanticAnalyzer();
            cSharp = new PascalToCSharp();
        }

        [TestCase("program test; var asdf : integer;begin end.", "public static class test\n{\nstatic int asdf;\npublic static void Main()\n{\n}\n}")]
        public void PascalInput_Should_CreateOutput(string input, string output)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            table.CheckSyntax(node);
            cSharp.VisitNode(node).Should().Be(output);
        }
    }
}