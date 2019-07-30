using System;
using System.Collections.Generic;
using FluentAssertions;
using Minesweeper.Test.Symbols;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{

    public class LoggerMock : Logger
    {
        public List<string> Calls { get; set; } = new List<string>();
        public override void Log(object obj)
        {
            Calls.Add(obj?.ToString());
            base.Log(obj);
        }
    }
    [TestFixture]
    public class PascalSemanticAnalyzerTests
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
        public void PascalProgram_ShouldGetSymbols()
        {
            var input =
                "PROGRAM Part10;\r\nVAR\r\n   number     : INTEGER;\r\n   a, b, c, x : INTEGER;\r\n   y          : REAL;\r\n\r\nBEGIN {Part10}\r\n   BEGIN\r\n      number := 2;\r\n      a := number;\r\n      b := 10 * a + 10 * number DIV 4;\r\n      c := a - - b\r\n   END;\r\n   x := 11;\r\n   y := 20 / 7 + 3.14;\r\n   { writeln('a = ', a); }\r\n   { writeln('b = ', b); }\r\n   { writeln('c = ', c); }\r\n   { writeln('number = ', number); }\r\n   { writeln('x = ', x); }\r\n   { writeln('y = ', y); }\r\nEND.  {Part10}";
            var result = table.CheckSyntax(ast.Evaluate(lexer.Tokenize(input)));
            result.Should().NotBeNull();

            var symbol = result.LookupSymbol("number", true);

            symbol.Type.Name.Should().Be(Pascal.Int);
        }

        [TestCase("program test; begin a := 2; end.")]
        [TestCase("program test; var a: integer; begin a := b; end.")]
        [TestCase("program SymTab5;\r\n    var x : integer;\r\n\r\nbegin\r\n    x := y;\r\nend.")]
        public void PascalVariableNotDelcared_Should_ThrowException(string input)
        {

            var node = ast.Evaluate(lexer.Tokenize(input));

            var tableBuilder = this.table;

            var t = Assert.Throws<SemanticException>(()=> tableBuilder.CheckSyntax(node));
            t.Error.Should().Be(ErrorCode.IdNotFound);
        }

        [TestCase("program SymTab6;\r\n   var x, y : integer;\r\n  y : real;\r\nbegin\r\n   x := x + y;\r\nend.")]
        public void PascalVariableDuplicated_Should_ThrowException(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = Assert.Throws<SemanticException>(() => table.CheckSyntax(node));
            result.Error.Should().Be(ErrorCode.DuplicateId);
        }

        [Test]
        public void LevelZeroScope_Should_Return2Symbols()
        {
            var input = "program SymTab3; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = table.CheckSyntax(node);

            result.Count.Should().Be(2);
        }

       


        [Test]
        public void VariableSymbols_Should_Return4Symbols()
        {
            var input = "program SymTab3;\r\n   var x, y : integer;\r\n\r\nbegin\r\n\r\nend.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = table.CheckSyntax(node);

            result.Count.Should().Be(4);
        }

        [TestCase("PROGRAM Part11;\r\nVAR\r\n   number : INTEGER;\r\n   a, b   : INTEGER;\r\n   y      : REAL;\r\n\r\nBEGIN {Part11}\r\n   number := 2;\r\n   a := number ;\r\n   b := 10 * a + 10 * number DIV 4;\r\n   y := 20 / 7 + 3.14\r\nEND.  {Part11}")]
        public void PascalProgram_ShouldInterpretAndCreateSymbols(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = table.CheckSyntax(node);
            var memory = interpreter.Interpret(node).Should().BeOfType<Memory>().Subject;
             memory.ContainsKey("a").Should().BeTrue();
        }

        [Test]
        public void PascalNoNonPascalExceptionsWhenAddingTokens()
        {
            var file = PascalInterpreterTests.GetFile("PascalFunctionSelfCall.txt");
            var tokens = lexer.Tokenize(file);
            var list = new List<TokenItem>();
            int i = 0;
            while (list.Count < tokens.Count)
            {
                try
                {
                    list.Add(tokens[i]);
                    i++;
                    var node = ast.Evaluate(tokens);
                    var result = table.CheckSyntax(node);
                }
                catch (PascalException e)
                {
                }
               
            }
           
        }

        [Test]
        public void LimitedScopes_GlobalMemory_ShouldNotContainVariablesInProcedure()
        {
            var input =
                "program test; var x : integer; procedure test(a : integer;); var b : integer; begin end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = table.CheckSyntax(node);

            memory.LookupSymbol("test", true).Should().NotBeNull();
            memory.LookupSymbol("x", true).Should().NotBeNull();
            memory.LookupSymbol("a", true).Should().BeNull();
            memory.LookupSymbol("b", true).Should().BeNull();
        }

        [Test]
        public void LimitedScopes_ProcedureScope_Should_CallGlobalVariable()
        {
            var input = "program globalTest; var x : integer; procedure test(a : integer;); var b : integer; begin x := 2; end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = table.CheckSyntax(node);
        }

        [Test]
        public void LimitedScopes_Should_BeAbleToOverrideGlobalValue()
        {
            var input = "program globalTest; var x : integer; procedure test(x : integer;); var b : integer; begin x := 2; end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = table.CheckSyntax(node);
        }

        [Test]
        public void LimitedScopes_OpensAndClosesScope()
        {
            var input ="program globalTest; var x : integer; procedure test(a : integer;); var b : integer; begin a := 2; end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = table.CheckSyntax(node);
            
            
            logger.Calls.Should().Contain("Opened Scope globalTest");
            logger.Calls.Should().Contain("Opened Scope test");
            logger.Calls.Should().Contain("Closed Scope test");
            logger.Calls.Should().Contain("Closed Scope globalTest");
        }
        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithMultipleParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithParameters)]
        public void PascalProgram_WithProcedures_ShouldPass(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = table.CheckSyntax(node);
        }



    }
}