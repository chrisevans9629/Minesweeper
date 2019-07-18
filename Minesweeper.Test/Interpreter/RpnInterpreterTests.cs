﻿using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class InterpreterTests
    {
        [Test]
        public void RpnTest()
        {
            var input = "(5 + 3) * 12 / 3";
            var tree = new SuperBasicMathAst(input);

            var t = tree.Evaluate();

            var inter = new SuperBasicRpnMathInterpreter();

            var result = inter.Evaluate(t);

            result.Should().Be("5 3 + 12 * 3 /");
        }

        [TestCase("2 + 3", "(+ 2 3)")]
        [TestCase("(2 + 3 * 5)", "(+ 2 (* 3 5))")]
        public void LispTest(string input, string output)
        {
            var tree = new SuperBasicMathAst(input);

            var t = tree.Evaluate();

            var inter = new SuperBasicMathLispInterpreter();

            var result = inter.Evaluate(t);

            result.Should().Be(output);
        }
    }
}