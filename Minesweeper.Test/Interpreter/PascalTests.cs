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
        public void PascalLexer()
        {
            var input =
                "BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.";
            var lexer = new PascalLexer(input);
            var result = lexer.Tokenize();

            result.Should().HaveCount(36);
        }

        [Test]
        public void PascalAst()
        {
            var input =
                "BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.";
            var lexer = new PascalLexer(input);
            var tokens = lexer.Tokenize();

            var ast = new PascalAst(tokens);

            var node = ast.Evaluate();
            var t = node.Display();
            node.Should().BeOfType<Compound>();
        }

        [TestCase("BEGIN\r\n\r\n    BEGIN\r\n        number := 2;\r\n        a := NumBer;\r\n        B := 10 * a + 10 * NUMBER / 4;\r\n        c := a - - b\r\n    end;\r\n\r\n    x := 11;\r\nEND.")]
        [TestCase("BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.")]
        public void PascalInterpreter(string input)
        {
            var lexer = new PascalLexer(input);
            var tokens = lexer.Tokenize();

            var ast = new PascalAst(tokens);

            var node = ast.Evaluate();

            var interpreter = new PascalInterpreter();
            interpreter.Evaluate(node);

            interpreter.GetVar("a").Should().Be(2);
            interpreter.GetVar("x").Should().Be(11);
            interpreter.GetVar("c").Should().Be(27);
            interpreter.GetVar("b").Should().Be(25);
            interpreter.GetVar("nuMber").Should().Be(2);
        }

        [Test]
        public void PascalDivTest()
        {
            var input = "Begin begin num := 2 div 2; end; End.";
            var lexer = new PascalLexer(input);
            var tokens = lexer.Tokenize();

            var ast = new PascalAst(tokens);

            var node = ast.Evaluate();

            var interpreter = new PascalInterpreter();
            interpreter.Evaluate(node);

            interpreter.GetVar("num").Should().Be(1);
        }

        [Test]
        public void PascalUnderScoreTest()
        {
            var input = "Begin begin _num := 2 div 2; end; End.";
            var lexer = new PascalLexer(input);
            var tokens = lexer.Tokenize();

            var ast = new PascalAst(tokens);

            var node = ast.Evaluate();

            var interpreter = new PascalInterpreter();
            interpreter.Evaluate(node);

            interpreter.GetVar("_num").Should().Be(1);
        }
    }
}