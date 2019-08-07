using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Minesweeper.Test;
using Minesweeper.Test.Symbols;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{

    public class CompileCSharp
    {
        public static bool CompileExecutable(string sourceName, string appName)
        {
            //FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;
            bool compileOk = false;

            // Select the code provider based on the input file extension.
            //if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
            //{
            provider = CodeDomProvider.CreateProvider("CSharp");
            //}
            //else if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".VB")
            //{
            //    provider = CodeDomProvider.CreateProvider("VisualBasic");
            //}
            //else
            //{
            //    Console.WriteLine("Source file must have a .cs or .vb extension");
            //}

            if (provider != null)
            {

                // Format the executable file name.
                // Build the output assembly path using the current directory
                // and <source>_cs.exe or <source>_vb.exe.

                String exeName = $@"{System.Environment.CurrentDirectory}\{appName}.exe";

                CompilerParameters cp = new CompilerParameters();

                // Generate an executable instead of 
                // a class library.
                cp.GenerateExecutable = true;

                // Specify the assembly file name to generate.
                cp.OutputAssembly = exeName;

                // Save the assembly as a physical file.
                cp.GenerateInMemory = false;

                // Set whether to treat all warnings as errors.
                cp.TreatWarningsAsErrors = false;

                // Invoke compilation of the source file.

                CompilerResults cr = provider.CompileAssemblyFromSource(cp,
                    sourceName);

                if (cr.Errors.Count > 0)
                {
                    // Display compilation errors.
                    Console.WriteLine("Errors building {0} into {1}",
                        sourceName, cr.PathToAssembly);
                    foreach (CompilerError ce in cr.Errors)
                    {
                        Console.WriteLine("  {0}", ce.ToString());
                        Console.WriteLine();
                    }
                }
                else
                {
                    // Display a successful compilation message.
                    Console.WriteLine("Source {0} built into {1} successfully.",
                        sourceName, cr.PathToAssembly);
                }

                // Return the results of the compilation.
                if (cr.Errors.Count > 0)
                {
                    compileOk = false;
                }
                else
                {
                    compileOk = true;
                }
            }
            return compileOk;
        }
    }

    [TestFixture]
    public class PascalToCSharpTests
    {
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer table;
        PascalToCSharp cSharp;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
            table = new PascalSemanticAnalyzer();
            cSharp = new PascalToCSharp();
        }

        private string CscPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";

        [Test]
        public void CscPath_ShouldFind()
        {
            File.Exists(CscPath).Should().BeTrue("the path to the csc is broken.  may break other tests");
        }

        [TestCase("program test; var asdf : integer;begin end.",
            @"public static class test
{
    static int asdf;
    public static void Main()
    {
    }
}")]
        [TestCase("program test; var asdf : integer;begin WriteLn('hello world!'); end.",
            @"using System;
public static class test
{
    static int asdf;
    public static void Main()
    {
        Console.WriteLine(" + "\"hello world!\"" + @");
    }
}")]
        public void PascalInput_Should_CreateOutput(string input, string output)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            table.CheckSyntax(node);
            cSharp.VisitNode(node).Should().Be(output);

            CompileCSharp.CompileExecutable(output, "test").Should().BeTrue();
        }
    }
}