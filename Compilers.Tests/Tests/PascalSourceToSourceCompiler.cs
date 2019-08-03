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
            if (node is VarDeclarationNode declaration) return VisitVarDeclaration(declaration);
            if (node is ProcedureDeclarationNode procedureDeclaration)
            {
                return VisitProcedureDec(procedureDeclaration);
            }

            if (node is AssignmentNode assign)
            {
                return $"{AddSpaces()}{VisitVariable(assign.Left)} := {VisitNode(assign.Right)};\r\n";
            }

            if (node is VariableOrFunctionCall varaible)
            {
                return VisitVariable(varaible);
            }
            if (node is BinaryOperator op)
            {
                return $"{VisitNode(op.Left)} {op.TokenItem.Value} {VisitNode(op.Right)}";
            }
            if (node is NoOp no)
            {
                return "";
            }
            //return "";
            throw new NotImplementedException($"no implementation for node {node}");
        }

        private string VisitVariable(VariableOrFunctionCall variable)
        {
            return $"<{variable.VariableName}{CurrentScope.LookupSymbolScope(variable.VariableName)}:{CurrentScope.LookupSymbol(variable.VariableName, true).Type.Name}>";
        }

        private string VisitProcedureDec(ProcedureDeclarationNode procedureDeclaration)
        {
            var prev = CurrentScope;
            var namedec = $"{AddSpaces()}procedure {procedureDeclaration.ProcedureId}{CurrentScope.ScopeLevel}";
            CurrentScope = new ScopedSymbolTable(procedureDeclaration.ProcedureId, prev.ScopeLevel + 1, prev);
            var dec =
                $"{namedec}({VisitProcedureDecParams(procedureDeclaration.Parameters)});\r\n" +
                VisitBlock(procedureDeclaration.Block);
            CurrentScope = prev;
            return dec;
        }

        private string VisitProcedureDecParams(IList<ParameterNode> parameters)
        {
            var str = "";
            foreach (var procedureParameter in parameters)
            {
                var symbol = new VariableSymbol(procedureParameter.Declaration.VarNode.VariableName,
                    CurrentScope.LookupSymbol(procedureParameter.Declaration.TypeNode.TypeValue, true));

                CurrentScope.Define(symbol);
                str +=
                    $"{procedureParameter.Declaration.VarNode.VariableName}{CurrentScope.ScopeLevel} : {procedureParameter.Declaration.TypeNode.TypeValue.ToUpper()};";
            }

            if (str.EndsWith(";"))
            {
                str = str.Remove(str.LastIndexOf(";", StringComparison.InvariantCulture), 1);
            }
            return str;
        }

        string AddSpaces(int add = 0)
        {
            var spaces = "";
            for (int i = 0; i < (CurrentScope.ScopeLevel * 3) + add; i++)
            {
                spaces += " ";
            }

            return spaces;
        }
        private string VisitVarDeclaration(VarDeclarationNode declaration)
        {
            var symbol = new VariableSymbol(declaration.VarNode.VariableName,
                CurrentScope.LookupSymbol(declaration.TypeNode.TypeValue, true));

            CurrentScope.Define(symbol);
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
            var str = $"{AddSpaces(-3)}begin\r\n";
            foreach (var compoundStatementNode in compoundStatement.Nodes)
            {
                str += VisitNode(compoundStatementNode);
            }

            str += AddSpaces(-3) + "end; {END OF " + CurrentScope.ScopeName + "}\r\n";
            return str;
        }

       
        private string VisitProgram(PascalProgramNode program)
        {
            var zero = new ScopedSymbolTable(program.ProgramName, 0);
            zero.Define(new BuiltInTypeSymbol(Pascal.Int));
            zero.Define(new BuiltInTypeSymbol(Pascal.Real));
            CurrentScope = zero;
            var programStr = VisitProgramBlockNode(program);
            return programStr;
        }

        private string VisitProgramBlockNode(PascalProgramNode program)
        {
            var programStr = $"program {program.ProgramName}{CurrentScope.ScopeLevel};\r\n";
            var global = new ScopedSymbolTable("Global", CurrentScope.ScopeLevel + 1, CurrentScope);
            CurrentScope = global;

            foreach (var blockDeclaration in program.Block.Declarations)
            {
                programStr += VisitNode(blockDeclaration);
            }
            programStr += $"{AddSpaces(-3)}begin\r\n";
            foreach (var compoundStatementNode in program.Block.CompoundStatement.Nodes)
            {
                programStr += VisitNode(compoundStatementNode);
            }
            CurrentScope = CurrentScope.ParentScope;
            programStr += AddSpaces(-3) + "end. {END OF " + CurrentScope.ScopeName + "}\r\n";
            return programStr;
        }
    }
}