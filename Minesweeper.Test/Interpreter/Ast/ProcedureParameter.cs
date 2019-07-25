namespace Minesweeper.Test
{
    public class ProcedureParameter : Node
    {
        public VarDeclaration Declaration { get; }

        public ProcedureParameter(VarDeclaration declaration)
        {
            Declaration = declaration;
        }
        public override string Display()
        {
            return $"Param({Declaration.Display()})";
        }
    }
}