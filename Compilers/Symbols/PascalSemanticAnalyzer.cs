﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test.Symbols
{
    public class PascalSemanticAnalyzer : IPascalNodeVisitor<object>
    {
        private readonly ILogger _logger;
        private ScopedSymbolTable _currentScope;
        public object VisitProgram(PascalProgramNode programNode)
        {
            var levelZero = CreateCurrentScope(programNode.ProgramName);


            var global = new ScopedSymbolTable("Global", 1, levelZero, _logger);
            CurrentScope = global;
            VisitBlock(programNode.Block);
            CurrentScope = global;
            return null;
        }

        public ScopedSymbolTable CreateCurrentScope(string name)
        {
            var levelZero = new ScopedSymbolTable(name, 0, null, _logger);
            CurrentScope = levelZero;

            DefineBuiltIns(levelZero);
            return levelZero;
        }

        public static void DefineBuiltIns(ScopedSymbolTable levelZero)
        {
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Int, PascalTerms.Real));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Real));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Pointer));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.String));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Char, PascalTerms.String));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Boolean));
            levelZero.Define(new ProcedureDeclarationSymbol("READ", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "look"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char})))
            }));

            levelZero.Define(new FunctionDeclarationSymbol("UpCase", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "look"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char})))
            }, levelZero.LookupSymbol(PascalTerms.Char, true)));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()));
            levelZero.Define(new ProcedureDeclarationSymbol("Halt", new List<ParameterNode>()));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x3"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x4"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));

            levelZero.Define(new ProcedureDeclarationSymbol("Write", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("Write", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
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



        public object VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
            //if (node is PascalProgramNode program)
            //{
            //    VisitProgram(program);
            //}
            //else if (node is CompoundStatementNode compound)
            //{
            //    VisitCompoundStatement(compound);
            //}
            //else if (node is AssignmentNode ass)
            //{
            //    VisitAssign(ass);
            //}
            //else if (node is NoOp nope)
            //{
            //    VisitNoOp(nope);
            //}
            //else if (node is EqualExpression equal)
            //{
            //    VisitEqualExpression(equal);
            //}
            //else if (node is InOperator op)
            //{

            //}
            //else if (node is PointerNode pointer)
            //{

            //}
            //else if (node is IfStatementNode ifStatement)
            //{
            //    VisitIfStatement(ifStatement);
            //}
            //else if (node is RealNode)
            //{

            //}
            //else if (node is IntegerNode)
            //{

            //}
            //else if (node is StringNode str)
            //{

            //}
            //else if (node is VariableOrFunctionCall variable)
            //{
            //    VisitVariable(variable);
            //}
            //else if (node is BinaryOperator)
            //{

            //}
            //else if (node is UnaryOperator)
            //{

            //}
            //else if (node is VarDeclarationNode declaration)
            //{
            //    VisitVarDeclaration(declaration);
            //}
            //else if (node is ProcedureDeclarationNode procedureDeclaration)
            //{
            //    VisitProcedureDeclaration(procedureDeclaration);
            //}
            //else if (node is FunctionDeclarationNode functionDeclaration)
            //{
            //    VisitFunctionDeclaration(functionDeclaration);
            //}
            //else if (node is FunctionCallNode funCall)
            //{
            //    VisitCall(funCall);
            //}
            //else if (node is ProcedureCallNode procCall)
            //{
            //    VisitCall(procCall);
            //}
            //else if (node is ConstantDeclarationNode decNode)
            //{
            //    VisitConstantDeclaration(decNode);
            //}
            //else if (node is CaseStatementNode caseStatement)
            //{
            //    VisitCaseStatement(caseStatement);
            //}
            //else if (node is WhileLoopNode whileLoop)
            //{
            //    VisitWhileLoop(whileLoop);
            //}
            //else if (node is NegationOperator negate)
            //{
            //    VisitNegationOperator(negate);
            //}
            //else
            //{
            //    throw new InvalidOperationException($"did not recognize node {node}");
            //}
        }

        public object VisitNegationOperator(NegationOperator negate)
        {
            VisitNode(negate.Right);
            return null;
        }

        public object VisitIfStatement(IfStatementNode ifStatement)
        {
            VisitNode(ifStatement.IfCheck);
            VisitNode(ifStatement.IfTrue);
            if (ifStatement.IfFalse != null)
            {
                VisitNode(ifStatement.IfFalse);
            }

            return null;
        }

        public object VisitForLoop(ForLoopNode forLoop)
        {
            VisitAssignment(forLoop.AssignFromNode);
            VisitNode(forLoop.ToNode);
            VisitNode(forLoop.DoStatements);
            return null;
        }

        public object VisitFunctionCall(CallNode functionCall)
        {
            return VisitCall(functionCall);
        }

        public object VisitEqualExpression(EqualExpression equal)
        {
            var left = VisitNode(equal.Left);
            var right = VisitNode(equal.Right);
            return CheckTypeMatch(equal.TokenItem, left, right, equal);
        }

        BuiltInTypeSymbol GetBuiltInType(object sym)
        {
            if (sym is Symbol symbol)
            {
                if (symbol is BuiltInTypeSymbol t)
                {
                    return t;
                }
                if (symbol.Type != null)
                {
                    return GetBuiltInType(symbol.Type);
                }
            }
            
           
            return null;
        }
        private object CheckTypeMatch(TokenItem tokenItem, object left, object right, Node nodeForLog, bool twoWayConversion = true)
        {
            if (GetBuiltInType(left) is BuiltInTypeSymbol a && GetBuiltInType(right) is BuiltInTypeSymbol b)
            {
                if (a.Name == b.Name)
                {
                    return a;
                }

                if (a.Conversions.Contains(b.Name) && twoWayConversion)
                {
                    return b;
                }
                if (b.Conversions.Contains(a.Name))
                {
                    return a;
                }


                TypeMismatch(tokenItem, a, b);
            }

            throw new NotImplementedException($"{nodeForLog}");
        }

        private static void TypeMismatch(TokenItem tokenItem, BuiltInTypeSymbol a, BuiltInTypeSymbol b)
        {
            throw new SemanticException(ErrorCode.TypeMismatch, tokenItem, $"type {a} is not assignable to {b}");
        }

        public object VisitWhileLoop(WhileLoopNode whileLoop)
        {
            VisitNode(whileLoop.BoolExpression);
            CurrentScope = new ScopedSymbolTable("_while_", CurrentScope.ScopeLevel + 1, CurrentScope);
            VisitNode(whileLoop.DoStatement);
            CurrentScope = CurrentScope.ParentScope;
            return null;
        }

        public object VisitReal(RealNode real)
        {
            //return real.Value;
            return CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Real, true);

        }

        public object VisitInteger(IntegerNode integer)
        {
            return CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Int, true);
        }

        public object VisitBinaryOperator(BinaryOperator binary)
        {
            var left = VisitNode(binary.Left);
            var right = VisitNode(binary.Right);
            return CheckTypeMatch(binary.TokenItem, left, right, binary);
        }

        public object VisitUnary(UnaryOperator unary)
        {
            var value = VisitNode(unary.Value);
            return value;
        }

        public object Fail(Node node)
        {
            return this.FailModel(node);
        }

        public object VisitString(StringNode str)
        {
            if (str.CurrentValue.Length == 1)
            {
                return CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Char, true);
            }
            else
            {
                return CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.String, true);
            }
        }

        public object VisitInOperator(InOperator inOperator)
        {
            var compare = VisitNode(inOperator.CompareNode);

            var list = VisitNode(inOperator.ListExpression);
            if (list is CollectionTypeSymbol collection)
            {
                var comType = GetBuiltInType(compare);
                if (collection.ItemType.Name != comType.Name && comType.Conversions.Contains(collection.ItemType.Name) != true)
                {
                    TypeMismatch(inOperator.TokenItem, collection, comType);
                }
            }

            return CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Boolean, true);
        }

        public object VisitCaseStatement(CaseStatementNode caseStatement)
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

            return null;
        }

        private void VisitCaseItem(CaseItemNode caseItem)
        {
            foreach (var caseItemCase in caseItem.Cases)
            {
                VisitNode(caseItemCase);
            }
            VisitNode(caseItem.Statement);
        }

        public object VisitConstantDeclaration(ConstantDeclarationNode decNode)
        {
            var value = decNode.Value;

            var typeName = $"Constant Error: {value}";

            if (value is PointerNode p)
            {
                typeName = p.TokenItem.Token.Name;
            }

            if (value is StringNode)
            {
                typeName = PascalTerms.String;
            }


            //VisitNode(decNode.Value);
            this.DefineVariableSymbol(decNode.TokenItem, decNode.ConstantName, typeName);
            return null;
        }

        public object VisitPointer(PointerNode pointer)
        {
            return pointer.Value;
        }

        public object VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            DeclareParameters(procedureDeclaration);
            VisitBlock(procedureDeclaration.Block);
            CurrentScope = CurrentScope.ParentScope;
            var procedure = new ProcedureDeclarationSymbol(procedureDeclaration.Name, procedureDeclaration.Parameters);
            CurrentScope.Define(procedure);
            return null;
        }

        public object VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            return VisitCall(procedureCall);
        }

        public object VisitFunctionDeclaration(FunctionDeclarationNode procedureDeclaration)
        {
            var procedure = new FunctionDeclarationSymbol(procedureDeclaration.Name, procedureDeclaration.Parameters, CurrentScope.LookupSymbol(procedureDeclaration.ReturnType.TypeValue, true));
            CurrentScope.Define(procedure);
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

            return procedure;
        }

        public object VisitBool(BoolNode boolNode)
        {
            return boolNode.Value;
        }

        public object VisitRangeExpression(ListRangeExpressionNode listRange)
        {

            CheckTypeMatch(listRange.TokenItem, VisitNode(listRange.FromNode), VisitNode(listRange.ToNode), listRange);
            
            return new CollectionTypeSymbol(CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.String, true));
        }

        public object VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode)
        {
            Node prevNode = null;
            foreach (var stringNode in itemsExpressionNode.Items)
            {
                if (prevNode == null)
                {
                    prevNode = stringNode;
                }

                CheckTypeMatch(itemsExpressionNode.TokenItem, VisitNode(prevNode), VisitNode(stringNode), itemsExpressionNode);
                prevNode = stringNode;
            }
            return new CollectionTypeSymbol(CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.String, true));
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

        private object VisitCall(CallNode funCall)
        {
            var symbols = CurrentScope.LookupSymbols<DeclarationSymbol>(funCall.Name, true);
            if (symbols.Any() != true)
            {
                NotFound(funCall.Token, funCall.Type, funCall.Name);
            }

            foreach (var funCallParameter in funCall.Parameters)
            {
                VisitNode(funCallParameter);
            }

            var callCount = funCall.Parameters.Count;

            

            if (symbols.All(p => p.Parameters.Count != callCount))
            {
                throw new SemanticException(ErrorCode.ParameterMismatch, funCall.Token, $"Function {funCall.Name} has {symbols.First().Parameters.Count} parameters but {callCount} was used");
            }

            var symbol = symbols.First(p => p.Parameters.Count == callCount);

            return symbol.Type;
        }


        public object VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            return VisitVariable(call);
        }

        public object VisitNoOp(NoOp nope)
        {
            return null;
        }

        public object VisitAssignment(AssignmentNode ass)
        {
            var variable = VisitVariable(ass.Left) as VariableSymbol;
            var assignmentType = VisitNode(ass.Right);
            variable.Initialized = true;

            //CheckType(ass.TokenItem, assignmentType, variable);


            //if (variable.Type.Name == PascalTerms.Int && ass.Right is RealNode r)
            //{
            //    throw new SemanticException(ErrorCode.TypeMismatch, r.TokenItem, $"Cannot assign Real to integer");
            //}

            return CheckTypeMatch(ass.TokenItem, variable, assignmentType, ass, false);
        }

        //private  object CheckType(TokenItem token, object assignmentType, Symbol variable)
        //{
        //    if (GetBuiltInType(assignmentType) is BuiltInTypeSymbol symbol)
        //    {
        //        var varTypeName = variable.Type.Name.ToUpper();
        //        var assTypeName = symbol.Name.ToUpper();
        //        if (varTypeName != assTypeName && symbol.Conversions.Select(p=>p.ToUpper()).Contains(varTypeName.ToUpper()) != true)
        //        {
        //            throw new SemanticException(ErrorCode.TypeMismatch, token,
        //                $"Cannot assign type '{assTypeName}' to '{variable.Name}' with type {varTypeName}");
        //        }

        //        return symbol;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException($"assignment not returning type symbol '{assignmentType}'");
        //    }
        //}

        private object VisitVariable(VariableOrFunctionCall assLeft)
        {
            var varName = assLeft.VariableName;
            Symbol symbol = CurrentScope.LookupSymbol<VariableSymbol>(varName, true);
            if (symbol == null)
            {
                symbol = CurrentScope.LookupSymbol<FunctionDeclarationSymbol>(varName, true);
            }
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


        public object VisitBlock(BlockNode programBlock)
        {
            foreach (var programBlockDeclaration in programBlock.Declarations)
            {
                VisitNode(programBlockDeclaration);
            }
            VisitCompoundStatement(programBlock.CompoundStatement);
            return null;
        }

        public object VisitCompoundStatement(CompoundStatementNode node)
        {
            foreach (var nodeNode in node.Nodes)
            {
                VisitNode(nodeNode);
            }

            return null;
        }



        public object VisitVarDeclaration(VarDeclarationNode node)
        {
            var typeName = node.TypeNode.TypeValue;
            var varName = node.VarNode.VariableName;
            return DefineVariableSymbol(node.VarNode.TokenItem, varName, typeName);
        }

        private VariableSymbol DefineVariableSymbol(TokenItem node, string varName, string typeName)
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
            return varSymbol;
        }
    }
}