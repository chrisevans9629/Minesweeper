using System;
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
        [TestCase("BEGIN\r\n    BEGIN\r\n    {THIS IS A COMMENT}number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.")]
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
        public void FullProgramTEst()
        {
            var input =
                "PROGRAM Part10;\r\nVAR\r\n   number     : INTEGER;\r\n   a, b, c, x : INTEGER;\r\n   y          : REAL;\r\n\r\nBEGIN {Part10}\r\n   BEGIN\r\n      number := 2;\r\n      a := number;\r\n      b := 10 * a + 10 * number DIV 4;\r\n      c := a - - b\r\n   END;\r\n   x := 11;\r\n   y := 20 / 7 + 3.14;\r\n   { writeln('a = ', a); }\r\n   { writeln('b = ', b); }\r\n   { writeln('c = ', c); }\r\n   { writeln('number = ', number); }\r\n   { writeln('x = ', x); }\r\n   { writeln('y = ', y); }\r\nEND.  {Part10}";
            var lexer = new PascalLexer(input);
            var ast = new PascalAst(lexer.Tokenize());
            var interpreter = new PascalInterpreter();
            interpreter.Evaluate(ast.Evaluate());
        }


        [Test]
        public void FullProgramTestTreeOutput()
        {
            var input = "PROGRAM Part10AST;\r\nVAR\r\n   a, b : INTEGER;\r\n   y    : REAL;\r\n\r\nBEGIN {Part10AST}\r\n   a := 2;\r\n   b := 10 * a + 10 * a DIV 4;\r\n   y := 20 / 7 + 3.14;\r\nEND.  {Part10AST}";
            var lexer = new PascalLexer(input);
            var ast = new PascalAst(lexer.Tokenize());

            Console.WriteLine(ast.Evaluate().Display());
            //var interpreter = new PascalInterpreter();
            //interpreter.Evaluate(ast.Evaluate());
        }


        [Test]
        public void RealConstTest()
        {
            var input = "10.5";
            var lexer = new PascalLexer(input);
            lexer.Tokenize()[0].Token.Name.Should().Be(Pascal.RealConst);
        }

        [TestCase("20 div 7", 2)]
        [TestCase("20 / 7", 2.95)]
        [TestCase("3.5 + 1", 4.5)]
        public void PascalDivTest(string math, double result)
        {
            var input = $"Begin begin num := {math}; end; End.";
            var lexer = new PascalLexer(input);
            var tokens = lexer.Tokenize();

            var ast = new PascalAst(tokens);

            var node = ast.Evaluate();

            var interpreter = new PascalInterpreter();
            interpreter.Evaluate(node);

            interpreter.GetVar("num").Should().BeOfType<double>().Which.Should().BeInRange(result - 0.5, result);
        }

        [Test]
        public void BasicProgram()
        {
            var input = "PROGRAM Part10;\r\nBEGIN\r\nEND.";
            var lexer = new PascalLexer(input);
            var tokens = lexer.Tokenize();

            var ast = new PascalAst(tokens);

            var node = ast.Evaluate();

            var interpreter = new PascalInterpreter();
            interpreter.Evaluate(node);
        }

        

        [Test]
        public void VariableLexerTest()
        {
            var input = "VAR\r\n   number     : INTEGER;\r\n   a, b, c, x : INTEGER;\r\n   y          : REAL;";
            var lexer = new PascalLexer(input);
            var tokens = lexer.Tokenize();

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