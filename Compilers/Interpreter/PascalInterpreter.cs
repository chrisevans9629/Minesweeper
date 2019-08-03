using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class HaltException : RuntimeException
    {
        public HaltException(ErrorCode error, TokenItem token, string message, Exception ex = null) : base(error, token, message, ex)
        {
        }
    }

    public class PascalInterpreter : SuperBasicMathInterpreter
    {
        private readonly IConsole _console;
        private ILogger _logger;
        public PascalInterpreter(ILogger logger = null, IConsole console = null)
        {
            _console = console ?? new ConsoleModel();
            this._logger = logger ?? new Logger();
        }
        public override object Interpret(Node node)
        {
            CreateGlobalMemory();

            try
            {
                var result = base.Interpret(node);
            }
            catch (HaltException e)
            {
                Console.WriteLine(e);
            }
            return CurrentScope;


        }

        public void CreateGlobalMemory()
        {
            CurrentScope = new Memory("GLOBAL");
            // CurrentScope.Add("Read",null);
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

        public override object VisitNode(Node node)
        {
            if (node is CompoundStatement compound)
            {
                return VisitCompound(compound);
            }

            if (node is AssignNode assign)
            {
                return VisitAssign(assign);
            }

            if (node is VariableOrFunctionCall var)
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

            if (node is ConstantDeclarationNode constantDeclarationNode)
            {
                return VisitConstantDec(constantDeclarationNode);
            }

            if (node is PointerNode pointer)
            {
                return VisitPointer(pointer);
            }

            if (node is StringNode str)
            {
                return str.CurrentValue;
            }

            if (node is InOperator inOperator)
            {
                return VisitInOperator(inOperator);
            }
            return base.VisitNode(node);
        }

        private object VisitInOperator(InOperator inOperator)
        {
            var valueToContain = VisitNode(inOperator.CompareNode).ToString()[0];

            var from = inOperator.ListExpression.FromNode.CurrentValue[0];

            var to = inOperator.ListExpression.ToNode.CurrentValue[0];
            var list = Enumerable.Range(from, to);

            return list.Contains(valueToContain);
        }

        public object VisitPointer2(object pointer)
        {
            if (pointer != null)
            {
                return ".";
            }
            return null;
        }
        public object VisitPointer(PointerNode pointer)
        {
            if (pointer.Value == 'I')
            {
                return "\t";
            }

            if (pointer.Value == 'G')
            {
                return ".";
            }
            throw new NotImplementedException($"{pointer}");
        }

        private object VisitConstantDec(ConstantDeclarationNode constantDeclarationNode)
        {
            CurrentScope.Add(constantDeclarationNode.ConstantName, VisitNode(constantDeclarationNode.Value));
            return null;
        }

        private object VisitForLoop(ForLoopNode forLoop)
        {
            var fromName = forLoop.AssignFromNode.Left.VariableName;
            VisitAssign(forLoop.AssignFromNode);
            var toInt = VisitNode(forLoop.ToNode);
            var fromValue = CurrentScope.GetValue(fromName, true);
            CurrentScope = new Memory("_ForLoop_", CurrentScope);
            for (var i = (int)fromValue; i <= (int)toInt; i++)
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

        public object VisitEqualOperator(Node eop)
        {
            if (eop is EqualExpression exp)
            {
                var left = VisitNode(exp.Left);
                var right = VisitNode(exp.Right);
                if (eop is EqualOperator)
                {
                    return left.Equals(right);
                }
                else if (eop is NotEqualOperator)
                {
                    return !left.Equals(right);
                }
                throw new RuntimeException(ErrorCode.UnexpectedToken, exp.TokenItem, $"Did not find an equality operator called {exp.Name}");
            }

            if (eop is NegationOperator negation)
            {
                var right = VisitNode(negation.Right);
                if (right is bool b)
                {
                    return !b;
                }
                throw new RuntimeException(ErrorCode.UnexpectedToken, negation.TokenItem, $"Did not find an equality operator called {negation.TokenItem.Token.Name}");
            }

            throw new RuntimeException(ErrorCode.UnexpectedToken, null, "Unexpected bool value");
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
            catch (HaltException)
            {
                throw;
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

                var funResult = CurrentScope.GetValue(call.Name);
                CurrentScope = CurrentScope.Parent;
                return funResult;
            }
            catch (HaltException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RuntimeException(ErrorCode.Runtime, call.Token, "Unexpected error", e);
            }

        }

        private object VisitProcedureCall(ProcedureCallNode call)
        {
            if (call.ProcedureName.ToUpper() == "READ")
            {
                var result = _console.Read();
                if (call.Parameters[0] is VariableOrFunctionCall v)
                {
                    CurrentScope.SetValue(v.VariableName, result.ToString());
                }

                return null;
            }

            if (call.ProcedureName.ToUpper() == "WRITE")
            {
                var str = "";
                foreach (var callParameter in call.Parameters)
                {
                    str += VisitNode(callParameter).ToString();
                }
                _console.Write(str);
                return null;
            }
            if (call.ProcedureName.ToUpper() == "WRITELN")
            {
                var str = "";
                foreach (var callParameter in call.Parameters)
                {
                    str += VisitNode(callParameter).ToString();
                }
                _console.WriteLine(str);
                return null;
            }

            if (call.ProcedureName.ToUpper() == "HALT")
            {
                throw new HaltException(ErrorCode.Runtime, call.Token, "Halted");
            }
            VisitCall<ProcedureDeclarationNode>(call);

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

        object VisitVariable(VariableOrFunctionCall var)
        {
            var name = var.VariableName.ToUpper();
            var value = CurrentScope.GetValue(name, true);
            if (value is FunctionDeclarationNode node)
            {
                var call = new FunctionCallNode(node.FunctionName, new List<Node>(), node.Token);
                return VisitNode(call);
            }
            return value;
        }

        static object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}