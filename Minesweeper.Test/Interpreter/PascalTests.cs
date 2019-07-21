using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class PascalTests
    {


        [Test]
        public void PascalTokensTest()
        {
            var input = "BEGIN a := 2; END.";

            var lexer = new PascalLexer(input);
            var result = lexer.Tokenize();


            result.Should().HaveCount(7);
            result[0].Token.Name.Should().Be(Pascal.Begin);
        }
        [Test]
        public void PascalAst()
        {
            var input =
                "BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.";
            var lexer = new PascalLexer(input);
            var result = lexer.Tokenize();

            result.Should().HaveCount(36);
        }
    }
}