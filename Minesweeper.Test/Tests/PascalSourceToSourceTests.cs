using Minesweeper.Test.Symbols;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalSourceToSourceTests
    {
        private PascalInterpreter interpreter;
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer table;
        private LoggerMock logger;
        [SetUp]
        public void Setup()
        {
            logger = new LoggerMock();
            interpreter = new PascalInterpreter(logger);
            lexer = new PascalLexer();
            ast = new PascalAst();
            table = new PascalSemanticAnalyzer(logger);
        }

        [Test]
        public void PascalSourceToSourceTest()
        {
            var input = PascalTestInputs.PascalSourceToSource;

            var tokens = lexer.Tokenize(input);
            var tree = ast.Evaluate(tokens);

            var check = table.CheckSyntax(tree);

            var result = PascalTestInputs.PascalSourceToSourceResult;
        }
    }
}