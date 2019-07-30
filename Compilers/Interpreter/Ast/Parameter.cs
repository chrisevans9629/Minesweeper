namespace Minesweeper.Test
{
    public class Parameter : Node
    {
        public VarDeclarationNode Declaration { get; }

        public Parameter(VarDeclarationNode declaration)
        {
            Declaration = declaration;
        }
        public override string Display()
        {
            return $"Param({Declaration.Display()})";
        }
    }
}