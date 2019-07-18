using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class SuperBasicMathInterpreter_Tests
    {
        [TestCase("1+1", 2)]
        [TestCase("1-1", 0)]
        [TestCase("14 + 2 * 3 - 6 / 2", 17)]
        [TestCase("2 * 2", 4)]
        [TestCase("(1+2)*2", 6)]
        [TestCase("2 * 2 +2", 6)]
        [TestCase("7 + 3 * (10 / (12 / (3 + 1) - 1))", 22)]
        public void Evaluate_Test(string input, double output)
        {
            var tree = new SuperBasicMathInterpreter(input);
            var t = tree.Evaluate();
            t.Should().Be(output);
        }
    }
}