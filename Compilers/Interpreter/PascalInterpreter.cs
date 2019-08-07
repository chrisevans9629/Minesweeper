using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class PascalInterpreter : SuperBasicMathInterpreter, IPascalNodeVisitor<object>
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
        public object VisitCompoundStatement(CompoundStatementNode compound)
        {
            foreach (var compoundNode in compound.Nodes)
            {
                VisitNode(compoundNode);
            }

            return compound;
        }

        public override object VisitNode(Node node)
        {
            if (node is CompoundStatementNode compound)
            {
                return VisitCompoundStatement(compound);
            }

            if (node is AssignmentNode assign)
            {
                return VisitAssignment(assign);
            }

            if (node is VariableOrFunctionCall var)
            {
                return VisitVariableOrFunctionCall(var);
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
                return VisitIfStatement(ifNode);
            }

            if (node is WhileLoopNode whileLoop)
            {
                return VisitWhileLoop(whileLoop);
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
                return VisitConstantDeclaration(constantDeclarationNode);
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

            if (node is CaseStatementNode caseStatement)
            {
                return VisitCaseStatement(caseStatement);
            }
            return base.VisitNode(node);
        }

        public object VisitWhileLoop(WhileLoopNode whileLoop)
        {
            while ((bool)VisitNode(whileLoop.BoolExpression))
            {
                VisitNode(whileLoop.DoStatement);
            }
            return null;
        }

        public object VisitCaseStatement(CaseStatementNode caseStatement)
        {
            var comparer = VisitNode(caseStatement.CompareExpression);

            foreach (var caseItem in caseStatement.CaseItemNodes)
            {
                foreach (var caseItemCase in caseItem.Cases)
                {
                    var value = VisitNode(caseItemCase);
                    if (value.Equals(comparer))
                    {
                        return VisitNode(caseItem.Statement);
                    }
                }
            }
            if (caseStatement.ElseStatement != null)
                return VisitNode(caseStatement.ElseStatement);
            return null;
        }

        public object VisitInOperator(InOperator inOperator)
        {
            var valueToContain = VisitNode(inOperator.CompareNode).ToString()[0];

            if (inOperator.ListExpression is ListRangeExpressionNode listRange)
            {
                var from = listRange.FromNode.CurrentValue[0];

                var to = listRange.ToNode.CurrentValue[0];
                var list = Enumerable.Range(from, to);

                return list.Contains(valueToContain);
            }

            if (inOperator.ListExpression is ListItemsExpressionNode listItems)
            {
                var items = listItems.Items.Select(p => p.CurrentValue[0]);

                return items.Contains(valueToContain);
            }
            throw new NotImplementedException();

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

        public object VisitConstantDeclaration(ConstantDeclarationNode constantDeclarationNode)
        {
            CurrentScope.Add(constantDeclarationNode.ConstantName, VisitNode(constantDeclarationNode.Value));
            return null;
        }

        public object VisitForLoop(ForLoopNode forLoop)
        {
            var fromName = forLoop.AssignFromNode.Left.VariableName;
            VisitAssignment(forLoop.AssignFromNode);
            var toInt = VisitNode(forLoop.ToNode);
            var fromValue = CurrentScope.GetValue(fromName, true);
            CurrentScope = new Memory("_ForLoop_", CurrentScope);
            for (var i = (int)fromValue; i <= (int)toInt; i++)
            {
                VisitNode(forLoop.DoStatements);
            }
            CurrentScope = CurrentScope.Parent;
            return null;
        }

        public object VisitIfStatement(IfStatementNode ifNode)
        {
            if ((bool)VisitEqualOperator(ifNode.IfCheck))
            {
                var node = ifNode.IfTrue;
                VisitNode(node);
            }
            else if (ifNode.IfFalse != null)
            {
                var node = ifNode.IfFalse;
                VisitNode(node);
            }

            return null;
        }

        public object VisitEqualOperator(Node eop)
        {
            if (eop is EqualExpression exp)
            {
                return VisitEqualExpression(exp);
            }

            if (eop is NegationOperator negation)
            {
                return VisitNegationOperator(negation);
            }

            throw new RuntimeException(ErrorCode.UnexpectedToken, null, "Unexpected bool value");
        }

        public object VisitNegationOperator(NegationOperator negation)
        {
            var right = VisitNode(negation.Right);
            if (right is bool b)
            {
                return !b;
            }

            throw new RuntimeException(ErrorCode.UnexpectedToken, negation.TokenItem,
                $"Did not find an equality operator called {negation.TokenItem.Token.Name}");
        }

        public object VisitEqualExpression(EqualExpression exp)
        {
            var left = VisitNode(exp.Left);
            var right = VisitNode(exp.Right);
            if (exp is EqualOperator)
            {
                return left.Equals(right);
            }
            else if (exp is NotEqualOperator)
            {
                return !left.Equals(right);
            }

            throw new RuntimeException(ErrorCode.UnexpectedToken, exp.TokenItem,
                $"Did not find an equality operator called {exp.Name}");
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

        public object VisitFunctionCall(CallNode call)
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

        public object VisitProcedureCall(ProcedureCallNode call)
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

        public object VisitProcedureDeclaration(ProcedureDeclarationNode procedure)
        {
            this.CurrentScope.Add(procedure.ProcedureId, procedure);
            return null;
        }

        public object VisitProgram(PascalProgramNode programNode)
        {
            return VisitBlock(programNode.Block);
        }

        public object VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            this.CurrentScope.Add(varDeclaration.VarNode.VariableName, null);
            return null;
        }
        public object VisitBlock(BlockNode block)
        {
            foreach (var blockDeclaration in block.Declarations)
            {
                VisitNode(blockDeclaration);
            }

            return VisitCompoundStatement(block.CompoundStatement);
        }

        public object VisitAssignment(AssignmentNode node)
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

        public object VisitVariableOrFunctionCall(VariableOrFunctionCall var)
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

        public object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}