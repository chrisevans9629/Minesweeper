using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class MathSimpleTreeTests
    {
        private SimpleTree tree;

        [SetUp]
        public void Setup()
        {
            tree = new SimpleTree();
        }
        [TestCase("14 + 2 * 3 - 6 / 2", 17)]
        [TestCase("2 * 2", 4)]
        [TestCase("(1+2)*2", 6)]
        [TestCase("2 * 2 +2", 6)]
        [TestCase("7 + 3 * (10 / (12 / (3 + 1) - 1))", 22)]
        [TestCase("14 + 2 * 3 - 6 / 2", 17)]
        public void Evaluate_Test(string input, double output)
        {
            var lex = new RegexLexer();
            lex.Ignore(" ");
            lex.Add("LPA", @"\(");
            lex.Add("RPA", @"\)");
            lex.Add("NUM", @"\d+");
            lex.Add("ADD", @"\+");
            lex.Add("SUB", @"-");
            lex.Add("MUL", @"\*");
            lex.Add(SimpleTree.FloatDiv, @"/");
            var tokens = lex.Tokenize(input);
            var t = tree.Evaluate(tokens);

            t.Should().Be(output);
        }
    }
}