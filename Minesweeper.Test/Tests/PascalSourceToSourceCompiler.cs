using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Symbols;

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

        private ScopedSymbolTable CurrentScope;
        public string Convert(string pascalInput)
        {
            var tokens = lexer.Tokenize(pascalInput);
            var tree = ast.Evaluate(tokens);

            table.CheckSyntax(tree);

            return VisitNode(tree);
        }

        private string VisitNode(Node node)
        {
            if (node is PascalProgramNode program) return VisitProgram(program);
            if (node is BlockNode block) return VisitBlock(block);
            if (node is VarDeclaration declaration) return VisitVarDeclaration(declaration);
            if (node is ProcedureDeclaration procedureDeclaration)
            {
                return VisitProcedureDec(procedureDeclaration);
            }

            if (node is Assign assign)
            {

            }
            //return "";
            throw new NotImplementedException($"no implementation for node {node}");
        }

        private string VisitProcedureDec(ProcedureDeclaration procedureDeclaration)
        {
            var prev = CurrentScope;
            var namedec = $"{AddSpaces()}procedure {procedureDeclaration.ProcedureId}";
            CurrentScope = new ScopedSymbolTable(procedureDeclaration.ProcedureId, prev.ScopeLevel + 1, prev);
            var dec =
                $"{namedec}({VisitProcedureDecParams(procedureDeclaration.Parameters)});\r\n" +
                VisitBlock(procedureDeclaration.Block);
            CurrentScope = prev;
            return dec;
        }

        private string VisitProcedureDecParams(IList<ProcedureParameter> parameters)
        {
            var str = "";
            foreach (var procedureParameter in parameters)
            {
                str +=
                    $"{procedureParameter.Declaration.VarNode.VariableName}{CurrentScope.ScopeLevel} : {procedureParameter.Declaration.TypeNode.TypeValue.ToUpper()};";
            }

            if (str.EndsWith(";"))
            {
                str = str.Remove(str.LastIndexOf(";", StringComparison.InvariantCulture), 1);
            }
            return str;
        }

        string AddSpaces()
        {
            var spaces = "";
            for (int i = 0; i < CurrentScope.ScopeLevel * 3; i++)
            {
                spaces += " ";
            }

            return spaces;
        }
        private string VisitVarDeclaration(VarDeclaration declaration)
        {
            
            return $"{AddSpaces()}var {declaration.VarNode.VariableName}{CurrentScope.ScopeLevel} : {declaration.TypeNode.TypeValue.ToUpper()};\r\n";
        }

        private string VisitBlock(BlockNode block)
        {
            var str = "";
            foreach (var blockDeclaration in block.Declarations)
            {
                str += VisitNode(blockDeclaration);
            }
            str += VisitCompoundStatement(block.CompoundStatement);
            return str;
        }

        private string VisitCompoundStatement(CompoundStatement compoundStatement)
        {
            var str = "";
            foreach (var compoundStatementNode in compoundStatement.Nodes)
            {
                str += VisitNode(compoundStatementNode);
            }
            return str;
        }

        private string VisitProgram(PascalProgramNode program)
        {
            var zero = new ScopedSymbolTable(program.ProgramName, 0);
            zero.Define(new BuiltInTypeSymbol(Pascal.Int));
            zero.Define(new BuiltInTypeSymbol(Pascal.Real));

            var global = new ScopedSymbolTable("Global", zero.ScopeLevel + 1, zero);
            CurrentScope = global;
            return $"program {program.ProgramName}{zero.ScopeLevel};\r\n" + VisitNode(program.Block);
        }
    }
}