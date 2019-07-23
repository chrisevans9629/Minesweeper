using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
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

            tokens[0].Token.Name.Should().Be("VAR");
            tokens[1].Token.Name.Should().Be("ID");
            tokens[2].Token.Name.Should().Be("SEMICOLON");
            tokens[3].Token.Name.Should().Be("INTEGER");
            tokens[4].Token.Name.Should().Be("SEMI");
            tokens[5].Token.Name.Should().Be("ID");
            tokens[6].Token.Name.Should().Be("COMMA");
            tokens[17].Token.Name.Should().Be("REAL");
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

        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        public void PascalLexer_ShouldContainProcedureToken(string input)
        {
            var tokens = lexer.Tokenize(input);

            tokens.Should().Contain(p => p.Token.Name == Pascal.Procedure);
        }

    }
}