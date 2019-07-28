using System;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalAstTests
    {
        private PascalLexer lexer;
        private PascalAst ast;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
        }
        [Test]
        public void FullProgramTestTreeOutput()
        {
            var input = "PROGRAM Part10AST;\r\nVAR\r\n   a, b : INTEGER;\r\n   y    : REAL;\r\n\r\nBEGIN {Part10AST}\r\n   a := 2;\r\n   b := 10 * a + 10 * a DIV 4;\r\n   y := 20 / 7 + 3.14;\r\nEND.  {Part10AST}";

            var tokens = lexer.Tokenize(input);

            var node = ast.Evaluate(tokens);


            Console.WriteLine(node.Display());
            //var interpreter = new PascalInterpreter();
            //interpreter.Evaluate(ast.Evaluate());
        }
        [Test]
        public void PascalIfTest()
        {
            var input = "if x = 10 then x := 5 else x := 1";

            var tokens = lexer.Tokenize(input);

            ast.CreateIterator(tokens);

           var result = ast.Statement();
           var iff = result.Should().BeOfType<IfStatementNode>().Which;
           iff.IfTrue.Should().BeOfType<AssignNode>();
           iff.IfFalse.Should().BeOfType<AssignNode>();

        }
        [Test]
        public void BooleanParse()
        {
            var file = PascalInterpreterTests.GetFile("PascalBoolean.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);
        }

        [Test]
        public void BooleanSimpleParse()
        {
            var input = "3 = 3";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression();
            var equal = node.Should().BeOfType<EqualOperator>().Which;
            equal.Left.Should().BeOfType<NumberLeaf>();
            equal.Right.Should().BeOfType<NumberLeaf>();
        }

        [TestCase("4 = 5")]
        [TestCase("(1+1) = (2+1)")]
        [TestCase("2*1= (2+1)")]
        [TestCase("(2+1)*2 = (2+1)")]
        public void BoolTheory(string input)
        {
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression();
            node.Should().BeOfType<EqualOperator>();
        }


        [Test]
        public void ThrowParserException()
        {
            var input = "program;";

            var tokens = lexer.Tokenize(input);
            var result = Assert.Throws<ParserException>(() => ast.Evaluate(tokens));

            result.Token.Line.Should().Be(1);
            result.Message.Should().Be("Expected an 'ID' token but was ';' at index 7 column 8 line 1");
        }

        [Test]
        public void PascalAst_ShouldStartWithProgram()
        {
            var input =
                "Program test; BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var t = node.Display();
            node.Should().BeOfType<PascalProgramNode>();
        }

        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithMultipleParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithMultipleVarDeclarations)]
        public void PascalProgram_WithProcedures_ShouldPass(string input)
        {
            var tokens = lexer.Tokenize(input);
            var inter = ast.Evaluate(tokens);

        }

    }
}