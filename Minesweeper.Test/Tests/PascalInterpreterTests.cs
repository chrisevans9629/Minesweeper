﻿using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    public class PascalInterpreterTests
    {
        private PascalInterpreter interpreter;
        private PascalLexer lexer;
        private PascalAst ast;
        [SetUp]
        public void Setup()
        {
            interpreter = new PascalInterpreter();
            lexer = new PascalLexer();
            ast = new PascalAst();
        }

        [Test]
        public void PascalUnderScore_ShouldBeRecognized()
        {
            var input = "program test; Begin begin _num := 2 div 2; end; End.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            interpreter.Interpret(node);

            interpreter.GetVar("_num").Should().Be(1);
        }

        [TestCase("20 div 7", 2)]
        [TestCase("20 / 7", 2.95)]
        [TestCase("3.5 + 1", 4.5)]
        public void PascalMath_ShouldReturnResult(string math, double result)
        {
            var input = $"program test; Begin begin num := {math}; end; End.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            interpreter.Interpret(node);

            interpreter.GetVar("num").Should().BeOfType<double>().Which.Should().BeInRange(result - 0.5, result);
        }
        [TestCase("PRogram test; BEGIN\r\n\r\n    BEGIN\r\n        number := 2;\r\n        a := NumBer;\r\n        B := 10 * a + 10 * NUMBER / 4;\r\n        c := a - - b\r\n    end;\r\n\r\n    x := 11;\r\nEND.")]
        [TestCase("Program test; BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.")]
        [TestCase("Program test; BEGIN\r\n    BEGIN\r\n    {THIS IS A COMMENT}number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.")]
        public void PascalInterpreter_ShouldExcludeCommentsAndCasing(string input)
        {
            var tokens = lexer.Tokenize(input);


            var node = ast.Evaluate(tokens);

            interpreter.Interpret(node);

            interpreter.GetVar("a").Should().Be(2);
            interpreter.GetVar("x").Should().Be(11);
            interpreter.GetVar("c").Should().Be(27);
            interpreter.GetVar("b").Should().Be(25);
            interpreter.GetVar("nuMber").Should().Be(2);
        }

        [Test]
        public void FullProgram_ShouldWork()
        {
            var input =
                "PROGRAM Part10;\r\nVAR\r\n   number     : INTEGER;\r\n   a, b, c, x : INTEGER;\r\n   y          : REAL;\r\n\r\nBEGIN {Part10}\r\n   BEGIN\r\n      number := 2;\r\n      a := number;\r\n      b := 10 * a + 10 * number DIV 4;\r\n      c := a - - b\r\n   END;\r\n   x := 11;\r\n   y := 20 / 7 + 3.14;\r\n   { writeln('a = ', a); }\r\n   { writeln('b = ', b); }\r\n   { writeln('c = ', c); }\r\n   { writeln('number = ', number); }\r\n   { writeln('x = ', x); }\r\n   { writeln('y = ', y); }\r\nEND.  {Part10}";
            var node = ast.Evaluate(lexer.Tokenize(input));
            interpreter.Interpret(node);
        }

        [Test]
        public void BasicProgram_ShouldPass()
        {
            var input = "PROGRAM Part10;\r\nBEGIN\r\nEND.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            interpreter.Interpret(node);
        }

        [Test]
        public void PascalProgram_WithProcedures_ShouldPass()
        {
            var input = PascalTestInputs.PascalProgramWithProcedures;
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = interpreter.Interpret(node);
        }
    }
}