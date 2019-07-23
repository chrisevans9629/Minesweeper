using System;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class PascalAstTests
    {
        private PascalLexer lexer;
        private PascalAst ast;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
        }
        [Test]
        public void FullProgramTestTreeOutput()
        {
            var input = "PROGRAM Part10AST;\r\nVAR\r\n   a, b : INTEGER;\r\n   y    : REAL;\r\n\r\nBEGIN {Part10AST}\r\n   a := 2;\r\n   b := 10 * a + 10 * a DIV 4;\r\n   y := 20 / 7 + 3.14;\r\nEND.  {Part10AST}";

            Console.WriteLine(ast.Evaluate(lexer.Tokenize(input)).Display());
            //var interpreter = new PascalInterpreter();
            //interpreter.Evaluate(ast.Evaluate());
        }
        [Test]
        public void PascalAst_ShouldStartWithProgram()
        {
            var input =
                "Program test; BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var t = node.Display();
            node.Should().BeOfType<PascalProgram>();
        }

    }
}