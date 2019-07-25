using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class ProcedureDeclaration : Node
    {
        public string ProcedureId { get; }
        public Block Block { get; }
        public IList<ProcedureParameter> Parameters { get; }

        public ProcedureDeclaration(string procedureId, Block block, IList<ProcedureParameter> parameters)
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