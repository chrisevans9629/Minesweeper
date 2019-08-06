using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class PascalToCSharp
    {
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
            throw new NotImplementedException($"Not done: {node}");
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
            return $"static {typeValue} {varDeclaration.VarNode.VariableName};\n";
        }

        private string VisitProgram(PascalProgramNode program)
        {
            return $"public static class {program.ProgramName}\n{VisitBlock(program.Block, "Main", "void")}\n";
        }

        private string VisitBlock(BlockNode block, string name, string type)
        {
            return "{\n" + $"{VisitNodes(block.Declarations)}{VisitCompoundStatment(block.CompoundStatement, name, type)}" + "}\n";
        }

        private string VisitCompoundStatment(CompoundStatement compoundStatement, string name, string type)
        {
            return $"public static {type} {name}()\n"+"{\n" + VisitNodes(compoundStatement.Nodes) + "}\n";
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