using System;
using System.Linq;
using Minesweeper.Test.Tests;

namespace Minesweeper.Test.Symbols
{
    public class PascalSemanticAnalyzer
    {
        private readonly ILogger _logger;
        private ScopedSymbolTable _currentScope;
        private void VisitProgram(PascalProgramNode programNode)
        {
            var levelZero = new ScopedSymbolTable(programNode.ProgramName, 0, null, _logger);
            CurrentScope = levelZero;

            levelZero.Define(new BuiltInTypeSymbol(Pascal.Int));
            levelZero.Define(new BuiltInTypeSymbol(Pascal.Real));

            var global = new ScopedSymbolTable("Global", 1, levelZero, _logger);
            CurrentScope = global;
            VisitBlock(programNode.Block);
            CurrentScope = global;
        }
        public ScopedSymbolTable CurrentScope
        {
            get => _currentScope;
            private set
            {
                if (_currentScope != null)
                {
                    _logger.Log($"Closed Scope {_currentScope.ScopeName}");
                }
                _currentScope = value;
                if (value != null)
                {
                    _logger.Log($"Opened Scope {value.ScopeName}");
                }
            }
        }

        public PascalSemanticAnalyzer(ILogger logger = null)
        {
            _logger = logger ?? new Logger();
        }
        public ScopedSymbolTable CheckSyntax(Node rootNode)
        {
            VisitNode(rootNode);
            return CurrentScope;
        }



        private void VisitNode(Node node)
        {
            if (node is PascalProgramNode program)
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
            else if (node is VarDeclaration declaration)
            {
                VisitVarDeclaration(declaration);
            }
            else if (node is ProcedureDeclaration procedureDeclaration)
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
            var previous = CurrentScope;
            var name = procedureDeclaration.ProcedureId;
            var scope = new ScopedSymbolTable(name, CurrentScope.ScopeLevel + 1, previous, _logger);
            CurrentScope = scope;
            var param = procedureDeclaration.Parameters;
            foreach (var varDeclaration in param.Select(p => p.Declaration))
            {
                //VisitVarDeclaration(varDeclaration);
                var typeName = varDeclaration.TypeNode.TypeValue;
                var varName = varDeclaration.VarNode.VariableName;
                var symbol = this.CurrentScope.LookupSymbol(typeName, true);
                var varSymbol = new VariableSymbol(varName, symbol);
                CurrentScope.Define(varSymbol);
            }
            VisitBlock(procedureDeclaration.Block);
            CurrentScope = previous;
            var procedure = new ProcedureSymbol(name, param);
            CurrentScope.Define(procedure);
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
            var symbol = CurrentScope.LookupSymbol(varName, true);
            if (symbol == null)
            {
                throw new InvalidOperationException($"Variable '{varName}' was not declared");
            }
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

        private void VisitVarDeclaration(VarDeclaration node)
        {
            var typeName = node.TypeNode.TypeValue;
            var varName = node.VarNode.VariableName;
            var variable = CurrentScope.LookupSymbol(varName, false);
            if (variable != null)
            {
                throw new InvalidOperationException($"Variable '{varName}' has already been defined as {variable}");
            }

            var symbol = this.CurrentScope.LookupSymbol(typeName, true);
            if (symbol == null)
            {
                throw new InvalidOperationException($"Could not find type {typeName}");
            }

            var varSymbol = new VariableSymbol(varName, symbol);
            CurrentScope.Define(varSymbol);
        }
    }
}