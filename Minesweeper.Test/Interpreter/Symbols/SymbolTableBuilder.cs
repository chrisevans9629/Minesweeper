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

       

        private void VisitNode(Node node)
        {
            if (node is PascalProgram program)
            {
                VisitProgram(program);
            }
            else if (node is CompoundStatement compound)
            {
                VisitCompoundStatement(compound);
            }
            else if (node is Assign ass)
            {
                VisitAssign(ass);
            }
            else if (node is NoOp nope)
            {
                VisitNoOp(nope);
            }
            else if (node is NumberLeaf)
            {

            }
            else if (node is Variable variable)
            {
                VisitVariable(variable);
            }
            else if (node is BinaryOperator)
            {
                
            }
            else if (node is UnaryOperator)
            {

            }
            else if(node is VarDeclaration declaration)
            {
                VisitDeclaration(declaration);
            }
            else if(node is ProcedureDeclaration procedureDeclaration)
            {
                VisitProcedureDec(procedureDeclaration);
            }
            else
            {
                throw new InvalidOperationException($"did not recognize node {node}");
            }
        }

        private void VisitProcedureDec(ProcedureDeclaration procedureDeclaration)
        {
            
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
                VisitNode(programBlockDeclaration);
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