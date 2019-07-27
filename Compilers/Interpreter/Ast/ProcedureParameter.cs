namespace Minesweeper.Test
{
    public class ProcedureParameter : Node
    {
        public VarDeclarationNode Declaration { get; }

        public ProcedureParameter(VarDeclarationNode declaration)
        {
            Declaration = declaration;
        }
        public override string Display()
        {
            return $"Param({Declaration.Display()})";
        }
    }
}