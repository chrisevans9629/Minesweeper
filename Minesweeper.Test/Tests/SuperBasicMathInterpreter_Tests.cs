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
        [TestCase("--2",2)]
        public void Evaluate_Test(string input, double output)
        {
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate();

            var inter = new SuperBasicMathInterpreter();

            var r = inter.Evaluate(t);
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
            node.Should().BeOfType<UnaryOperator>();
            node.Name.Should().Be(SimpleTree.Sub);
        }

        [Test]
        public void BasicAst_Should_Add()
        {
            var input = "1+2";
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate();

            t.TokenItem.Token.Name.Should().Be(SimpleTree.Add);
            ((BinaryOperator) t).Left.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            ((BinaryOperator) t).Left.TokenItem.Value.Should().Be("1");
            ((BinaryOperator) t).Right.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            ((BinaryOperator) t).Right.TokenItem.Value.Should().Be("2");
        }

        [Test]
        public void AddAndSubtract_Should_HaveSubAtTopOfTree()
        {
            var input = "1+2-3";

            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));

            var t = tree.Evaluate();
            t.Name.Should().Be(SimpleTree.Sub);
        }

        [Test]
        public void BasicAst_Should_Subtract()
        {
            var input = "1-2";
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate();

            t.TokenItem.Token.Name.Should().Be(SimpleTree.Sub);
            ((BinaryOperator)t).Left.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            ((BinaryOperator)t).Left.TokenItem.Value.Should().Be("1");
            ((BinaryOperator)t).Right.TokenItem.Token.Name.Should().Be(SimpleTree.Num);
            ((BinaryOperator)t).Right.TokenItem.Value.Should().Be("2");
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