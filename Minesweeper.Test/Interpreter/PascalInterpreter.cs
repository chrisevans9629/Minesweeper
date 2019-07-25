using System.Collections.Generic;
using Minesweeper.Test.Tests;

namespace Minesweeper.Test
{

    public class GlobalMemory : Dictionary<string, object>
    {

    }

    public class PascalInterpreter : SuperBasicMathInterpreter
    {
        private ILogger logger;
        public PascalInterpreter(ILogger logger = null)
        {
            this.logger = logger ?? new Logger();
        }
        public override object Interpret(Node node)
        {
            _scope = new GlobalMemory();
            var result = base.Interpret(node);
            return _scope;
        }

        public object GetVar(string key)
        {
            return _scope[key.ToUpper()];
        }
        private GlobalMemory _scope = new GlobalMemory();
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

            if (node is ProcedureDeclarationNode procedure)
            {
                return VisitProcedureDeclaration(procedure);
            }
            if (node is VarDeclarationNode declaration)
            {
                return VisitVarDeclaration(declaration);
            }
            return base.VisitNode(node);
        }

        private object VisitProcedureDeclaration(ProcedureDeclarationNode procedure)
        {
            return null;
        }

        private object VisitProgram(PascalProgramNode programNode)
        {
            return VisitBlock(programNode.Block);
        }

        object VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            this._scope.Add(varDeclaration.VarNode.VariableName, null);
            return null;
        }
        private object VisitBlock(BlockNode block)
        {
            foreach (var blockDeclaration in block.Declarations)
            {
                VisitNode(blockDeclaration);
            }

            return VisitNode(block.CompoundStatement);
        }

        object VisitAssign(AssignNode node)
        {
            var name = node.Left.VariableName.ToUpper();
            var value = VisitNode(node.Right);
            if (_scope.ContainsKey(name))
            {
                _scope[name] = value;
            }
            else
            {
                _scope.Add(name, value);
            }

            return value;
        }

        object VisitVariable(Variable var)
        {
            var name = var.VariableName.ToUpper();
            var value = _scope[name];
            return value;
        }

        static object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}