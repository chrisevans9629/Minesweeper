using System;
using System.Collections.Generic;
using System.Linq;

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

            DefineBuiltIns(levelZero);


            var global = new ScopedSymbolTable("Global", 1, levelZero, _logger);
            CurrentScope = global;
            VisitBlock(programNode.Block);
            CurrentScope = global;
        }

        private static void DefineBuiltIns(ScopedSymbolTable levelZero)
        {
            levelZero.Define(new BuiltInTypeSymbol(Pascal.Int));
            levelZero.Define(new BuiltInTypeSymbol(Pascal.Real));
            levelZero.Define(new BuiltInTypeSymbol(Pascal.Pointer));
            levelZero.Define(new BuiltInTypeSymbol(Pascal.String));
            levelZero.Define(new BuiltInTypeSymbol(Pascal.Char));
            levelZero.Define(new BuiltInTypeSymbol(Pascal.Boolean));
            levelZero.Define(new ProcedureDeclarationSymbol("READ", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "look"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char})))
            }));

            levelZero.Define(new FunctionDeclarationSymbol("UpCase", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "look"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char})))
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()));
            levelZero.Define(new ProcedureDeclarationSymbol("Halt", new List<ParameterNode>()));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x3"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x4"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
            }));

            levelZero.Define(new ProcedureDeclarationSymbol("Write", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("Write", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = Pascal.Char}))),
            }));
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
            else if (node is AssignmentNode ass)
            {
                VisitAssign(ass);
            }
            else if (node is NoOp nope)
            {
                VisitNoOp(nope);
            }
            else if (node is InOperator op)
            {

            }
            else if (node is PointerNode pointer)
            {

            }
            else if (node is IfStatementNode)
            {

            }
            else if (node is RealNode)
            {

            }
            else if (node is IntegerNode)
            {

            }
            else if (node is StringNode str)
            {

            }
            else if (node is VariableOrFunctionCall variable)
            {
                VisitVariable(variable);
            }
            else if (node is BinaryOperator)
            {

            }
            else if (node is UnaryOperator)
            {

            }
            else if (node is VarDeclarationNode declaration)
            {
                VisitVarDeclaration(declaration);
            }
            else if (node is ProcedureDeclarationNode procedureDeclaration)
            {
                VisitProcedureDeclaration(procedureDeclaration);
            }
            else if (node is FunctionDeclarationNode functionDeclaration)
            {
                VisitFunctionDeclaration(functionDeclaration);
            }
            else if (node is FunctionCallNode funCall)
            {
                VisitCall(funCall);
            }
            else if (node is ProcedureCallNode procCall)
            {
                VisitCall(procCall);
            }
            else if (node is ConstantDeclarationNode decNode)
            {
                VisitConstantDeclaration(decNode);
            }
            else if (node is CaseStatementNode caseStatement)
            {
                VisitCaseStatement(caseStatement);
            }
            else if (node is WhileLoopNode whileLoop)
            {
                VisitWhileLoop(whileLoop);
            }
            else
            {
                throw new InvalidOperationException($"did not recognize node {node}");
            }
        }

        private void VisitWhileLoop(WhileLoopNode whileLoop)
        {
            VisitNode(whileLoop.BoolExpression);
            CurrentScope = new ScopedSymbolTable("_while_", CurrentScope.ScopeLevel + 1, CurrentScope);
            VisitNode(whileLoop.DoStatement);
            CurrentScope = CurrentScope.ParentScope;
        }

        private void VisitCaseStatement(CaseStatementNode caseStatement)
        {
            VisitNode(caseStatement.CompareExpression);
            foreach (var caseStatementCaseItemNode in caseStatement.CaseItemNodes)
            {
                VisitCaseItem(caseStatementCaseItemNode);
            }

            if (caseStatement.ElseStatement != null)
            {
                VisitNode(caseStatement.ElseStatement);
            }
        }

        private void VisitCaseItem(CaseItemNode caseItem)
        {
            foreach (var caseItemCase in caseItem.Cases)
            {
                VisitNode(caseItemCase);
            }
            VisitNode(caseItem.Statement);
        }

        private void VisitConstantDeclaration(ConstantDeclarationNode decNode)
        {
            var value = decNode.Value;

            var typeName = $"Constant Error: {value}";

            if (value is PointerNode p)
            {
                typeName = p.TokenItem.Token.Name;
            }

            if (value is StringNode)
            {
                typeName = Pascal.String;
            }

            
            //VisitNode(decNode.Value);
            this.DefineVariableSymbol(decNode.TokenItem, decNode.ConstantName, typeName);
        }

        private void VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            DeclareParameters(procedureDeclaration);
            VisitBlock(procedureDeclaration.Block);
            CurrentScope = CurrentScope.ParentScope;
            var procedure = new ProcedureDeclarationSymbol(procedureDeclaration.Name, procedureDeclaration.Parameters);
            CurrentScope.Define(procedure);
        }

        private void VisitFunctionDeclaration(FunctionDeclarationNode procedureDeclaration)
        {
            DeclareParameters(procedureDeclaration);
            DefineVariableSymbol(procedureDeclaration.Token, procedureDeclaration.FunctionName, procedureDeclaration.ReturnType.TypeValue);
            VisitBlock(procedureDeclaration.Block);

            var returnVariable = CurrentScope.LookupSymbol<VariableSymbol>(procedureDeclaration.FunctionName, false);

            if (returnVariable == null)
            {
                NotFound(procedureDeclaration.Token, procedureDeclaration.ReturnType.TypeValue, procedureDeclaration.FunctionName);
            }

            if (!returnVariable.Initialized)
            {
                throw new SemanticException(ErrorCode.DoesNotReturnValue, procedureDeclaration.Token, $"Function {procedureDeclaration.FunctionName} does not return a value");
            }

            CurrentScope = CurrentScope.ParentScope;
            var procedure = new FunctionDeclarationSymbol(procedureDeclaration.Name, procedureDeclaration.Parameters);
            CurrentScope.Define(procedure);
        }

        private void DeclareParameters(DeclarationNode procedureDeclaration)
        {
            var name = procedureDeclaration.Name;
            var scope = new ScopedSymbolTable(name, CurrentScope.ScopeLevel + 1, CurrentScope, _logger);
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


        }

        private void VisitCall(CallNode funCall)
        {
           var symbol = CurrentScope.LookupSymbols<DeclarationSymbol>(funCall.Name, true);
           if (symbol.Any() != true)
           {
                NotFound(funCall.Token, funCall.Type, funCall.Name);
           }

           foreach (var funCallParameter in funCall.Parameters)
           {
               VisitNode(funCallParameter);
           }

           var callCount = funCall.Parameters.Count;


           if (symbol.All(p => p.Parameters.Count != callCount))
           {
                throw new SemanticException(ErrorCode.ParameterMismatch, funCall.Token, $"Function {funCall.Name} has {symbol.First().Parameters.Count} parameters but {callCount} was used");
           }

        }

        

        private void VisitNoOp(NoOp nope)
        {

        }

        private void VisitAssign(AssignmentNode ass)
        {
            var variable = VisitVariable(ass.Left);
            VisitNode(ass.Right);
            variable.Initialized = true;
            if (variable.Type.Name == Pascal.Int && ass.Right is RealNode r)
            {
                throw new SemanticException(ErrorCode.TypeMismatch, r.TokenItem, $"Cannot assign Real to integer");
            }
        }

        private VariableSymbol VisitVariable(VariableOrFunctionCall assLeft)
        {
            var varName = assLeft.VariableName;
            var symbol = CurrentScope.LookupSymbol<VariableSymbol>(varName, true);
            if (symbol == null)
            {
                NotFound(assLeft.TokenItem, "Variable", varName);
            }

            return symbol;
        }

        private static void NotFound(TokenItem assLeft, string type, string varName)
        {
            throw new SemanticException(ErrorCode.IdNotFound, assLeft, $"{type} '{varName}' was not declared");
        }


        private void VisitBlock(BlockNode programBlock)
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

        private void VisitVarDeclaration(VarDeclarationNode node)
        {
            var typeName = node.TypeNode.TypeValue;
            var varName = node.VarNode.VariableName;
            DefineVariableSymbol(node.VarNode.TokenItem, varName, typeName);
        }

        private void DefineVariableSymbol(TokenItem node, string varName, string typeName)
        {
            var variable = CurrentScope.LookupSymbol(varName, false);
            if (variable != null)
            {
                throw new SemanticException(ErrorCode.DuplicateId, node,
                    $"Variable '{varName}' has already been defined as {variable}");
            }

            var symbol = this.CurrentScope.LookupSymbol(typeName, true);
            if (symbol == null)
            {
                throw new SemanticException(ErrorCode.IdNotFound, node, $"Could not find type {typeName}");
            }

            var varSymbol = new VariableSymbol(varName, symbol);
            CurrentScope.Define(varSymbol);
        }
    }
}