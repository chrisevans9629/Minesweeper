using System;
using FluentAssertions;
using Minesweeper.Test.Symbols;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    public class PascalSourceToSourceCompiler
    {
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer table;
        public PascalSourceToSourceCompiler(ILogger logger)
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
            table = new PascalSemanticAnalyzer(logger);
        }

        public string Convert(string pascalInput)
        {
            var tokens = lexer.Tokenize(pascalInput);
            var tree = ast.Evaluate(tokens);

            var check = table.CheckSyntax(tree);

            return VisitNode(tree);
        }

        private string VisitNode(Node node)
        {
            if (node is PascalProgramNode program) return VisitProgram(program);
            throw new NotImplementedException($"no implementation for node {node}");
        }

        private string VisitProgram(PascalProgramNode program)
        {
            return $"program {program.ProgramName}0";
        }
    }

    [TestFixture]
    public class PascalSourceToSourceTests
    {
        private LoggerMock logger;
        private PascalSourceToSourceCompiler compiler;
        [SetUp]
        public void Setup()
        {
            logger = new LoggerMock();
            compiler = new PascalSourceToSourceCompiler(logger);
        }

        [Test]
        public void PascalSourceToSourceTest()
        {
            var input = PascalTestInputs.PascalSourceToSource;

            var output = compiler.Convert(input);
            var result = PascalTestInputs.PascalSourceToSourceResult;

            output.Should().Be(result);
        }
    }
}