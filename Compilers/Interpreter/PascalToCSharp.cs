using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Symbols;

namespace Minesweeper.Test
{
    
    public class PascalToCSharp
    {
        private ScopedSymbolTable CurrentScope;
        IList<string> _assembliesCalled = new List<string>();
        public string VisitNode(Node node)
        {
            if (node is PascalProgramNode program)
            {
                return VisitProgram(program).Trim();
            }

            if (node is VarDeclarationNode varDeclaration)
            {
                return VisitVariableDeclaration(varDeclaration);
            }

            if (node is NoOp)
            {
                return "";
            }

            if (node is ProcedureCallNode procedureCall)
            {
                return VisitProcedureCall(procedureCall);
            }

            if (node is StringNode str)
            {
                return "\"" + str.CurrentValue + "\"";
            }
            throw new NotImplementedException($"Not done: {node}");
        }

        private string VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            if (procedureCall.ProcedureName.ToUpper() == "WRITELN")
            {
                var assembly = "using System;";
                if (_assembliesCalled.Contains(assembly) != true)
                {
                    _assembliesCalled.Add(assembly);
                }

                var param = "";
                if (procedureCall.Parameters.Any())
                {
                    foreach (var procedureCallParameter in procedureCall.Parameters)
                    {
                        param += VisitNode(procedureCallParameter);
                        param += ",";
                    }

                    param = param.Remove(param.Length - 1);
                }
               
                
                return $"{AddSpaces(4)}Console.WriteLine({param});\r\n";
            }

            return "";
        }

        private string VisitVariableDeclaration(VarDeclarationNode varDeclaration)
        {
            var typeValue = varDeclaration.TypeNode.TypeValue.ToUpper();
            if (PascalTerms.Int == typeValue)
            {
                typeValue = "int";
            }

            if (PascalTerms.Real == typeValue)
            {
                typeValue = "double";
            }

            if (PascalTerms.Boolean == typeValue)
            {
                typeValue = "bool";
            }
            return $"{AddSpaces()}static {typeValue} {varDeclaration.VarNode.VariableName};\r\n";
        }

        private string VisitProgram(PascalProgramNode program)
        {
            var zero = new ScopedSymbolTable(program.ProgramName, 0);
            PascalSemanticAnalyzer.DefineBuiltIns(zero);
            CurrentScope = zero;
            var block = VisitBlock(program.Block, "Main", "void");
            var assems = "";

            foreach (var s in _assembliesCalled)
            {
                assems += s + "\r\n";
            }

            var str = $"{assems}public static class {program.ProgramName}\r\n{block}\r\n";
            return str;
        }
        string AddSpaces(int add = 0)
        {
            var spaces = "";
            for (int i = 0; i < (CurrentScope.ScopeLevel * 4) + add; i++)
            {
                spaces += " ";
            }

            return spaces;
        }
        private string VisitBlock(BlockNode block, string name, string type)
        {
            var str = AddSpaces() + "{\r\n";
            CurrentScope = new ScopedSymbolTable(name, CurrentScope.ScopeLevel + 1, CurrentScope);
            str += $"{VisitNodes(block.Declarations)}{VisitCompoundStatment(block.CompoundStatement, name, type)}";
            CurrentScope = CurrentScope.ParentScope;
            str += AddSpaces() + "}\r\n";
            return str;
        }

        private string VisitCompoundStatment(CompoundStatement compoundStatement, string name, string type)
        {
            return $"{AddSpaces()}public static {type} {name}()\r\n" + AddSpaces() + "{\r\n" + VisitNodes(compoundStatement.Nodes) + AddSpaces() + "}\r\n";
        }

        private string VisitNodes(IList<Node> blockDeclarations)
        {
            var str = "";
            foreach (var blockDeclaration in blockDeclarations)
            {
                str += VisitNode(blockDeclaration);
            }
            return str;
        }
    }
}