using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class ProcedureDeclarationNode : Node
    {
        public string ProcedureId { get; }
        public BlockNode Block { get; }
        public IList<ProcedureParameter> Parameters { get; }

        public ProcedureDeclarationNode(string procedureId, BlockNode block, IList<ProcedureParameter> parameters)
        {
            ProcedureId = procedureId;
            Block = block;
            Parameters = parameters;
        }

        public override string Display()
        {
            return $"Procedure({ProcedureId},{Block.Display()}";
        }
    }
}