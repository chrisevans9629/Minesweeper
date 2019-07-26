﻿using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Xamarin.Forms.Internals;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalLexerTests
    {
        private PascalLexer lexer;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
        }
        [Test]
        public void VariableLexerTest_ShouldOccurInOrder()
        {
            var input = "VAR\r\n   number     : INTEGER;\r\n   a, b, c, x : INTEGER;\r\n   y          : REAL;";
            var tokens = lexer.Tokenize(input);

            tokens[0].Token.Name.Should().Be(Pascal.Var);
            tokens[1].Token.Name.Should().Be(Pascal.Id);
            tokens[2].Token.Name.Should().Be(Pascal.Colon);
            tokens[3].Token.Name.Should().Be(Pascal.Int);
            tokens[4].Token.Name.Should().Be(Pascal.Semi);
            tokens[5].Token.Name.Should().Be(Pascal.Id);
            tokens[6].Token.Name.Should().Be(Pascal.Comma);
            tokens[17].Token.Name.Should().Be(Pascal.Real);
        }
        [Test]
        public void RealConstTest()
        {
            var input = "10.5";

            lexer.Tokenize(input)[0].Token.Name.Should().Be(Pascal.RealConst);
        }
        [Test]
        public void PascalTokensTest()
        {
            var input = "BEGIN a := 2; END.";

            var result = lexer.Tokenize(input);


            result.Should().HaveCount(7);
            result[0].Token.Name.Should().Be(Pascal.Begin);
        }
        [Test]
        public void PascalLexer()
        {
            var input =
                "BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.";
            var result = lexer.Tokenize(input);

            result.Should().HaveCount(36);
        }


        [Test]
        public void LineTest()
        {
            var input = "program Test;\nbegin end.";
            var tokens = lexer.Tokenize(input);

            tokens.Take(3).ForEach(p => p.Line.Should().Be(1));

            tokens.Skip(3).Take(3).ForEach(p => p.Line.Should().Be(2));
        }

        [TestCase("    .", 5)]
        [TestCase("    test", 5)]
        [TestCase("    asdf", 5)]
        [TestCase("    as", 5)]
        [TestCase("    \nas", 1)]
        public void ColumnTests(string input, int column)
        {
            var tokens = lexer.Tokenize(input);

            tokens[0].Column.Should().Be(column);
        }

        [Test]
        public void ColumnTest()
        {
            var input = "program Test;\nbegin end.";
            var tokens = lexer.Tokenize(input);

            tokens[0].Column.Should().Be(1);
            tokens.Last().Column.Should().Be(10);
        }


        [TestCase("~",1)]
        [TestCase(" ~",2)]
        [TestCase("  ~",3)]
        public void InvalidToken_ThrowsLexerExceptions(string input, int column)
        {
            var ex = Assert.Throws<LexerException>(() => lexer.Tokenize(input));
            ex.Token.Column.Should().Be(column);
        }

        [Test]
        public void InvalidToken_ThrowsLexerException()
        {
            var input = "test test2\nadsf ~";
            input.Length.Should().Be(17);
            var ex = Assert.Throws<LexerException>(() => lexer.Tokenize(input));
            ex.Token.Line.Should().Be(2);
            ex.Token.Column.Should().Be(6);
            ex.Token.Index.Should().Be(16);
            ex.Token.Value.Should().Be("~");
            ex.Error.Should().Be(ErrorCode.UnexpectedToken);
            ex.Message.Should().Be("Unexpected token '~' at index 16 line 2 column 6");
        }

        [Test]
        public void CasingTest()
        {
            var input = "program TEst; begin END.";

            var tokens = lexer.Tokenize(input);

            tokens.Select(p => p.Value).Should().ContainInOrder("program", "TEst", ";", "begin", "END", ".");
        }

        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithMultipleParameters)]
        public void PascalLexer_ShouldContainProcedureToken(string input)
        {
            var tokens = lexer.Tokenize(input);

            tokens.Should().Contain(p => p.Token.Name == Pascal.Procedure);
        }

    }
}