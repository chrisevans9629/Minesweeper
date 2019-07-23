using System;

namespace Minesweeper.Test.Symbols
{
    public class SymbolTableBuilder
    {
        SymbolTable table = new SymbolTable();

        public SymbolTable CreateTable(Node rootNode)
        {
            VisitNode(rootNode);
            return table;
        }

        void Error(Node node)
        {
            throw new InvalidOperationException($"did not recognize node {node}");
        }

        private void VisitNode(Node rootNode)
        {
            if (rootNode is PascalProgram program)
            {
                VisitProgram(program);
            }
            else if (rootNode is CompoundStatement compound)
            {
                VisitCompoundStatement(compound);
            }
            else if (rootNode is Assign ass)
            {
                VisitAssign(ass);
            }
            else if (rootNode is NoOp nope)
            {
                VisitNoOp(nope);
            }
            else if (rootNode is NumberLeaf)
            {

            }
            else if (rootNode is Variable variable)
            {
                VisitVariable(variable);
            }
            else if (rootNode is BinaryOperator)
            {
                
            }
            else if (rootNode is UnaryOperator)
            {

            }
            else
            {
                Error(rootNode);
            }
        }

        private void VisitNoOp(NoOp nope)
        {

        }

        private void VisitAssign(Assign ass)
        {
            VisitVariable(ass.Left);
            VisitNode(ass.Right);
        }

        private void VisitVariable(Variable assLeft)
        {
            var varName = assLeft.VariableName;
            var symbol = table.LookupSymbol(varName);
            if (symbol == null)
            {
                throw new InvalidOperationException($"Variable '{varName}' was not declared");
            }
        }

        private void VisitProgram(PascalProgram program)
        {
            VisitBlock(program.Block);
        }

        private void VisitBlock(Block programBlock)
        {
            foreach (var programBlockDeclaration in programBlock.Declarations)
            {
                VisitDeclaration(programBlockDeclaration);
            }
            VisitCompoundStatement(programBlock.CompoundStatement);
        }

        private void VisitCompoundStatement(CompoundStatement node)
        {
            foreach (var nodeNode in node.Nodes)
            {
                VisitNode(nodeNode);
            }
        }

        private void VisitDeclaration(VarDeclaration node)
        {
            var typeName = node.TypeNode.TypeValue;
            var symbol = this.table.LookupSymbol(typeName);
            if (symbol == null)
            {
                throw new InvalidOperationException($"Could not find type {typeName}");
            }
            var varName = node.VarNode.VariableName;

            var varSymbol = new VariableSymbol(varName, symbol);
            table.Define(varSymbol);
        }
    }
}