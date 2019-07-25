using System;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
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