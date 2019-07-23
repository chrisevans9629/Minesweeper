using System;
using FluentAssertions;
using Minesweeper.Test.Symbols;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class SymbolTableBuilderTests
    {
        [Test]
        public void PascalProgram_ShouldGetSymbols()
        {
            var input =
                "PROGRAM Part10;\r\nVAR\r\n   number     : INTEGER;\r\n   a, b, c, x : INTEGER;\r\n   y          : REAL;\r\n\r\nBEGIN {Part10}\r\n   BEGIN\r\n      number := 2;\r\n      a := number;\r\n      b := 10 * a + 10 * number DIV 4;\r\n      c := a - - b\r\n   END;\r\n   x := 11;\r\n   y := 20 / 7 + 3.14;\r\n   { writeln('a = ', a); }\r\n   { writeln('b = ', b); }\r\n   { writeln('c = ', c); }\r\n   { writeln('number = ', number); }\r\n   { writeln('x = ', x); }\r\n   { writeln('y = ', y); }\r\nEND.  {Part10}";
            var lexer = new PascalLexer(input).Tokenize();
            var ast = new PascalAst(lexer).Evaluate();
            var table = new SymbolTableBuilder().CreateTable(ast);
            table.Should().NotBeNull();

            var symbol = table.LookupSymbol("number");

            symbol.Type.Should().Be(Pascal.Int);
        }

        [TestCase("program test; begin a := 2; end.")]
        [TestCase("program test; var a: integer; begin a := b; end.")]
        public void PascalVariableNotDelcared_Should_ThrowException(string input)
        {
            var lexer = new PascalLexer(input);

            var ast = new PascalAst(lexer.Tokenize()).Evaluate();

            var tableBuilder = new SymbolTableBuilder();

            var table = Assert.Throws<InvalidOperationException>(()=> tableBuilder.CreateTable(ast));
        }

        [TestCase("PROGRAM Part11;\r\nVAR\r\n   number : INTEGER;\r\n   a, b   : INTEGER;\r\n   y      : REAL;\r\n\r\nBEGIN {Part11}\r\n   number := 2;\r\n   a := number ;\r\n   b := 10 * a + 10 * number DIV 4;\r\n   y := 20 / 7 + 3.14\r\nEND.  {Part11}")]
        public void PascalProgram_ShouldInterpretAndCreateSymbols(string input)
        {
            var lexer = new PascalLexer(input).Tokenize();
            var ast = new PascalAst(lexer).Evaluate();
            var table = new SymbolTableBuilder().CreateTable(ast);
            var memory = new PascalInterpreter().VisitNode(ast);
        }

    }
}