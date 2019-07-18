using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    public class SuperBasicMathInterpreter
    {
        public double Evaluate(Node node)
        {
            return 0;
        }
    }

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
            var tree = new SuperBasicMathAst(input);
            var t = tree.Evaluate();

            var inter = new SuperBasicMathInterpreter();

            var r = inter.Evaluate(t);
            r.Should().Be(output);
        }

        [Test]
        public void Test()
        {
            var input = "1+2";
            var tree = new SuperBasicMathAst(input);
            var t = tree.Evaluate();
            t.TokenItem.Token.Name.Should().Be(SimpleTree.Add);

            ((BinaryOperator) t).Left.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            ((BinaryOperator) t).Left.TokenItem.Value.Should().Be("1");
            ((BinaryOperator) t).Right.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            ((BinaryOperator) t).Right.TokenItem.Value.Should().Be("2");
        }


        [Test]
        public void TestTree()
        {
            var mulToken = new TokenItem(){Value = "*", Token = new Token(){Name = SimpleTree.Multi}};
            var addToken = new TokenItem(){Value = "+", Token = new Token(){Name = SimpleTree.Add}};
            var mulop = new BinaryOperator(
                new NumberLeaf(new TokenItem(){Value = "2", Token = new Token(){Name = SimpleTree.Num}}),
                new NumberLeaf(new TokenItem(){Value = "7", Token = new Token(){Name = SimpleTree.Num}}), 
                mulToken);
            var addop = new BinaryOperator(
                mulop,
                new NumberLeaf(new TokenItem(){Value = "3", Token = new Token(){Name = SimpleTree.Num}}), 
                addToken);

            addop.TokenItem.Token.Name.Should().Be(SimpleTree.Add);

        }
    }
}