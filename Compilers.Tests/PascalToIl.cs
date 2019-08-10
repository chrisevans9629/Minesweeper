using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Minesweeper.Test;
using Minesweeper.Test.Symbols;

namespace Compilers.Tests
{

    public class PascalToIl : IPascalNodeVisitor<object>
    {
        public object VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        public object VisitCompoundStatement(CompoundStatementNode statement)
        {
            foreach (var statementNode in statement.Nodes)
            {
                VisitNode(statementNode);
            }

            return null;
        }

        private ScopedSymbolTable symbolTable;
        public object VisitAssignment(AssignmentNode assignment)
        {
            VisitNode(assignment.Right);

            var varName = assignment.Left.VariableName;

            //var typeName = symbolTable.LookupSymbol(varName, true);
            //if (typeName.Type.Name == PascalTerms.Int)
            //{
            //    ilGenerator.Emit(OpCodes.Ldc_I4_S, assignment);
            //}
            //assignment.Left.
            ilGenerator.Emit(OpCodes.Stfld, varName);
            return null;
        }

        public object VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            throw new NotImplementedException();
        }

        public object VisitNoOp(NoOp noOp)
        {
            return null;
        }

        public object VisitBlock(BlockNode block)
        {
            throw new NotImplementedException();
        }

        public object VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            var name = varDeclaration.TypeNode.TypeValue;
            if (name == PascalTerms.Int)
            {
               var t  =this.Type.DefineField(varDeclaration.VarNode.VariableName, typeof(int), FieldAttributes.Public | FieldAttributes.Static);
            }

            if (name == PascalTerms.Real)
            {
                this.Type.DefineField(varDeclaration.VarNode.VariableName, typeof(double), FieldAttributes.Public | FieldAttributes.Static);
            }

            if (name == PascalTerms.Char)
            {
                this.Type.DefineField(varDeclaration.VarNode.VariableName, typeof(char), FieldAttributes.Public | FieldAttributes.Static);
            }
            if (name == PascalTerms.String)
            {
                this.Type.DefineField(varDeclaration.VarNode.VariableName, typeof(string), FieldAttributes.Public | FieldAttributes.Static);
            }
            if (name == PascalTerms.Boolean)
            {
                this.Type.DefineField(varDeclaration.VarNode.VariableName, typeof(bool), FieldAttributes.Public | FieldAttributes.Static);
            }
            return null;
        }



        public TypeBuilder Type;
        public AssemblyBuilder Assembly;

        public object VisitProgram(PascalProgramNode program)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName name = new AssemblyName();
            name.Name = "CalculatorExpression";
            // make a run-only assembly (can't save as .exe)
            Assembly = domain.DefineDynamicAssembly(
                name, AssemblyBuilderAccess.Run);
            ModuleBuilder module = Assembly.DefineDynamicModule(
                "CalculatorExpressionModule");
            Type = module.DefineType(
                "ExpressionExecutor", TypeAttributes.Public);

            this.symbolTable = new ScopedSymbolTable("Main", 0);
            PascalSemanticAnalyzer.DefineBuiltIns(symbolTable);
            VisitProgramBlock(program.Block);
            

            var result = Type.CreateType();

            return result;
        }

        MethodBuilder method;
        private ILGenerator ilGenerator;
        
        public object VisitProgramBlock(BlockNode block)
        {
            foreach (var blockDeclaration in block.Declarations)
            {
                VisitNode(blockDeclaration);
            }

            method = Type.DefineMethod("Main",
                MethodAttributes.Public | MethodAttributes.Static);
            ilGenerator = method.GetILGenerator();
            VisitCompoundStatement(block.CompoundStatement);
            return null;
        }

        public object VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            throw new NotImplementedException();
        }

        public object VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            throw new NotImplementedException();
        }

        public object VisitFunctionCall(CallNode functionCall)
        {
            throw new NotImplementedException();
        }

        public object VisitEqualExpression(EqualExpression equalExpression)
        {
            throw new NotImplementedException();
        }

        public object VisitNegationOperator(NegationOperator negationOperator)
        {
            throw new NotImplementedException();
        }

        public object VisitIfStatement(IfStatementNode ifStatement)
        {
            throw new NotImplementedException();
        }

        public object VisitForLoop(ForLoopNode forLoop)
        {
            throw new NotImplementedException();
        }

        public object VisitConstantDeclaration(ConstantDeclarationNode constantDeclaration)
        {
            throw new NotImplementedException();
        }

        public object VisitPointer(PointerNode pointer)
        {
            throw new NotImplementedException();
        }

        public object VisitInOperator(InOperator inOperator)
        {
            throw new NotImplementedException();
        }

        public object VisitCaseStatement(CaseStatementNode caseStatement)
        {
            throw new NotImplementedException();
        }

        public object VisitWhileLoop(WhileLoopNode whileLoop)
        {
            throw new NotImplementedException();
        }

        public object VisitReal(RealNode real)
        {
            throw new NotImplementedException();
        }

        public object VisitInteger(IntegerNode integer)
        {
            ilGenerator.Emit(OpCodes.Ldc_I4_S, integer.Value);
            return null;
        }

        public object VisitBinaryOperator(BinaryOperator binary)
        {
            throw new NotImplementedException();
        }

        public object VisitUnary(UnaryOperator unary)
        {
            throw new NotImplementedException();
        }

        public object Fail(Node node)
        {
            return this.FailModel(node);
        }

        public object VisitString(StringNode str)
        {
            throw new NotImplementedException();
        }

        public object VisitFunctionDeclaration(FunctionDeclarationNode faDeclarationNode)
        {
            throw new NotImplementedException();
        }

        public object VisitBool(BoolNode boolNode)
        {
            throw new NotImplementedException();
        }

        public object VisitRangeExpression(ListRangeExpressionNode listRange)
        {
            throw new NotImplementedException();
        }

        public object VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode)
        {
            throw new NotImplementedException();
        }
    }
}