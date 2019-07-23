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
        [TestCase("+ - 3", -3)]
        [TestCase("5 - - 2", 7)]
        [TestCase("5 + - 2", 3)]
        [TestCase("--2", 2)]
        public void Evaluate_Test(string input, double output)
        {
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate();

            var inter = new SuperBasicMathInterpreter();

            var r = inter.Interpret(t);
            r.Should().Be(output);
        }


        [Test]
        public void UnaryOperation()
        {
            var input = "-2";

            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));

            var node = tree.Evaluate();
            node.Should().BeOfType<UnaryOperator>().Which.Name.Should().Be(SimpleTree.Sub);
        }

        [Test]
        public void BasicAst_Should_Add()
        {
            var input = "1+2";
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate();

            var node = t.Should().BeOfType<BinaryOperator>().Which;
            node.Name.Should().Be(SimpleTree.Add);
            var left = node.Left.Should().BeOfType<NumberLeaf>().Which;
            left.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            left.TokenItem.Value.Should().Be("1");
            var right = node.Right.Should().BeOfType<NumberLeaf>().Which;

            right.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            right.TokenItem.Value.Should().Be("2");
        }

        [Test]
        public void AddAndSubtract_Should_HaveSubAtTopOfTree()
        {
            var input = "1+2-3";

            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));

            var t = tree.Evaluate().Should().BeOfType<BinaryOperator>().Which;
            t.Name.Should().Be(SimpleTree.Sub);
        }

        [Test]
        public void BasicAst_Should_Subtract()
        {
            var input = "1-2";
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate().Should().BeOfType<BinaryOperator>().Which;

            t.TokenItem.Token.Name.Should().Be(SimpleTree.Sub);

            var left = t.Left.Should().BeOfType<NumberLeaf>().Which;
            var right = t.Right.Should().BeOfType<NumberLeaf>().Which;

            left.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            left.TokenItem.Value.Should().Be("1");
            right.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            right.TokenItem.Value.Should().Be("2");
        }

        [Test]
        public void TestTree()
        {
            var mulToken = new TokenItem() { Value = "*", Token = new Token() { Name = SimpleTree.Multi } };
            var addToken = new TokenItem() { Value = "+", Token = new Token() { Name = SimpleTree.Add } };
            var mulop = new BinaryOperator(
                new NumberLeaf(new TokenItem() { Value = "2", Token = new Token() { Name = SimpleTree.Num } }),
                new NumberLeaf(new TokenItem() { Value = "7", Token = new Token() { Name = SimpleTree.Num } }),
                mulToken);
            var addop = new BinaryOperator(
                mulop,
                new NumberLeaf(new TokenItem() { Value = "3", Token = new Token() { Name = SimpleTree.Num } }),
                addToken);

            addop.TokenItem.Token.Name.Should().Be(SimpleTree.Add);

        }
    }
}