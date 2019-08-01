using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class PascalInterpreter : SuperBasicMathInterpreter
    {
        private ILogger logger;
        public PascalInterpreter(ILogger logger = null)
        {
            this.logger = logger ?? new Logger();
        }
        public override object Interpret(Node node)
        {
            CreateGlobalMemory();
            var result = base.Interpret(node);
            return CurrentScope;
        }

        public void CreateGlobalMemory()
        {
            CurrentScope = new Memory("GLOBAL");
        }

        public object GetVar(string key)
        {
            return CurrentScope.GetValue(key, true);
        }
        private Memory CurrentScope;
        object VisitCompound(CompoundStatement compound)
        {
            foreach (var compoundNode in compound.Nodes)
            {
                VisitNode(compoundNode);
            }

            return compound;
        }

        protected override object VisitNode(Node node)
        {
            if (node is CompoundStatement compound)
            {
                return VisitCompound(compound);
            }

            if (node is AssignNode assign)
            {
                return VisitAssign(assign);
            }

            if (node is Variable var)
            {
                return VisitVariable(var);
            }

            if (node is NoOp no)
            {
                return VisitNoOp(no);
            }

            if (node is PascalProgramNode program)
            {
                return VisitProgram(program);
            }

            if (node is ProcedureCallNode call)
            {
                return VisitProcedureCall(call);
            }
            if (node is ProcedureDeclarationNode procedure)
            {
                return VisitProcedureDeclaration(procedure);
            }

            if (node is BoolNode b)
            {
                return b.Value;
            }

            if (node is IfStatementNode ifNode)
            {
                return VisitIfNode(ifNode);
            }

            if (node is ForLoopNode forLoop)
            {
                return VisitForLoop(forLoop);
            }
            if (node is EqualOperator eop)
            {
                return VisitEqualOperator(eop);
            }
            if (node is FunctionCallNode funcCall)
            {
                return VisitFunctionCall(funcCall);
            }
            if (node is FunctionDeclarationNode funcdec)
            {
                CurrentScope.Add(funcdec.FunctionName, funcdec);
                return null;
            }
            if (node is VarDeclarationNode declaration)
            {
                return VisitVarDeclaration(declaration);
            }
            return base.VisitNode(node);
        }

        private object VisitForLoop(ForLoopNode forLoop)
        {
            var fromName = forLoop.AssignFromNode.Left.VariableName;
            VisitAssign(forLoop.AssignFromNode);
            var toInt = VisitNode(forLoop.ToNode);
            var fromValue = CurrentScope.GetValue(fromName, true);
            CurrentScope = new Memory("_ForLoop_", CurrentScope);
            for (var i =(int)fromValue ; i <= (int)toInt; i++)
            {
                foreach (var forLoopDoStatement in forLoop.DoStatements)
                {
                    VisitNode(forLoopDoStatement);
                }
            }
            CurrentScope = CurrentScope.Parent;
            return null;
        }

        private object VisitIfNode(IfStatementNode ifNode)
        {
            if ((bool)VisitEqualOperator(ifNode.IfCheck))
            {
                foreach (var node in ifNode.IfTrue)
                {
                    VisitNode(node);
                }
            }
            else if (ifNode.IfFalse != null)
            {
                foreach (var node in ifNode.IfFalse)
                {
                    VisitNode(node);
                }
            }

            return null;
        }

        public object VisitEqualOperator(EqualExpression eop)
        {
            var left = VisitNode(eop.Left);
            var right = VisitNode(eop.Right);
            if (eop is EqualOperator)
            {
                return left.Equals(right);
            }
            else if(eop is NotEqualOperator)
            {
                return !left.Equals(right);
            }
            throw new RuntimeException(ErrorCode.UnexpectedToken, eop.TokenItem, $"Did not find an equality operator called {eop.Name}");
        }

        private void VisitCall<T>(CallNode call) where T : DeclarationNode
        {
            try
            {
                var declaration = CurrentScope.GetValue<T>(call.Name, true);
                var values = new List<object>();
                for (var i = 0; i < declaration.Parameters.Count; i++)
                {
                    var value = VisitNode(call.Parameters[i]);
                    values.Add(value);
                }
                var previous = CurrentScope;
                CurrentScope = new Memory(call.Name, previous);
                CurrentScope.Add(call.Name, null);
                for (var i = 0; i < declaration.Parameters.Count; i++)
                {
                    var parameter = declaration.Parameters[i];
                    VisitVarDeclaration(parameter.Declaration);
                    CurrentScope.SetValue(parameter.Declaration.VarNode.VariableName, values[i]);
                }
                VisitBlock(declaration.Block);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RuntimeException(ErrorCode.Runtime, call.Token, "Unexpected Error", e);
            }

        }

        private object VisitFunctionCall(CallNode call)
        {
            try
            {
                VisitCall<FunctionDeclarationNode>(call);
                //var declaration = CurrentScope.GetValue<FunctionDeclarationNode>(call.Name, true);
                //var values = new List<object>();
                //for (var i = 0; i < declaration.Parameters.Count; i++)
                //{
                //    var value = VisitNode(call.Parameters[i]);
                //    values.Add(value);
                //}
                //var previous = CurrentScope;
                //CurrentScope = new Memory(call.Name, previous);
                //CurrentScope.Add(call.Name, null);
                //for (var i = 0; i < declaration.Parameters.Count; i++)
                //{
                //    var parameter = declaration.Parameters[i];
                //    VisitVarDeclaration(parameter.Declaration);
                //    CurrentScope.SetValue(parameter.Declaration.VarNode.VariableName, values[i]);
                //}


                //VisitBlock(declaration.Block);
                var funResult = CurrentScope.GetValue(call.Name);
                CurrentScope = CurrentScope.Parent;
                return funResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RuntimeException(ErrorCode.Runtime,call.Token,"Unexpected error", e);
            }

        }

        private object VisitProcedureCall(ProcedureCallNode call)
        {
            VisitCall<ProcedureDeclarationNode>(call);
            //var declaration = (ProcedureDeclarationNode)CurrentScope.GetValue(call.ProcedureName, true);
            //var previous = CurrentScope;
            //CurrentScope = new Memory(call.ProcedureName, previous);

            //for (var i = 0; i < declaration.Parameters.Count; i++)
            //{
            //    var parameter = declaration.Parameters[i];
            //    VisitVarDeclaration(parameter.Declaration);
            //    var value = VisitNode(call.Parameters[i]);
            //    CurrentScope.SetValue(parameter.Declaration.VarNode.VariableName, value);

            //}

            //VisitBlock(declaration.Block);
            CurrentScope = CurrentScope.Parent;

            return null;
        }

        private object VisitProcedureDeclaration(ProcedureDeclarationNode procedure)
        {
            this.CurrentScope.Add(procedure.ProcedureId, procedure);
            return null;
        }

        private object VisitProgram(PascalProgramNode programNode)
        {
            return VisitBlock(programNode.Block);
        }

        object VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            this.CurrentScope.Add(varDeclaration.VarNode.VariableName, null);
            return null;
        }
        private object VisitBlock(BlockNode block)
        {
            foreach (var blockDeclaration in block.Declarations)
            {
                VisitNode(blockDeclaration);
            }

            return VisitCompound(block.CompoundStatement);
        }

        object VisitAssign(AssignNode node)
        {
            var name = node.Left.VariableName.ToUpper();
            var value = VisitNode(node.Right);
            if (CurrentScope.ContainsKey(name))
            {
                CurrentScope.SetValue(name, value);
            }
            else
            {
                CurrentScope.Add(name, value);
            }

            return value;
        }

        object VisitVariable(Variable var)
        {
            var name = var.VariableName.ToUpper();
            var value = CurrentScope.GetValue(name, true);
            return value;
        }

        static object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}